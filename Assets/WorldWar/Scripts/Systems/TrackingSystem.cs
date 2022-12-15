using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


[RequireMatchingQueriesForUpdate] //EntityQuery�� ��������� OnUpdate�� �������� �ʰ�
[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct TrackingSystem : ISystem
{
    public TransformAspect.Lookup transformLookup;
    public ComponentLookup<Tracking> flockingLookup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Tracking>();
        transformLookup = new TransformAspect.Lookup(ref state, true);
        flockingLookup = state.GetComponentLookup<Tracking>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        transformLookup.Update(ref state);
        flockingLookup.Update(ref state);
        var target = SystemAPI.GetSingletonEntity<CharacterMove>();
        
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var trackingJob = new TrackingJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            transformLookup = transformLookup,
            physicsWorldSingleton = physicsWorldSingleton,
            flockingLookup = flockingLookup,
            target = target,
        };

        trackingJob.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Tracking))]
    [BurstCompile]
    private partial struct TrackingJob : IJobEntity
    {
        public float deltaTime;
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public TransformAspect.Lookup transformLookup;
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<Tracking> flockingLookup;
        [NativeDisableContainerSafetyRestriction]
        public PhysicsWorldSingleton physicsWorldSingleton;
        public Entity target;

        public const float NeighborDistance = 5f;
        public const float AligmentWeight = 1;
        public const float CohesionWeight = 2;
        public const float SeparationWeight = 3;

        private void Execute(ref TrackingAspect flockingAspect)
        {
            var flockingValue = flockingAspect.Value;
            flockingValue.moveTarget = transformLookup[target].Position;
            if (math.lengthsq(flockingAspect.Position - flockingValue.moveTarget) > 1)
            {
                var world = physicsWorldSingleton.PhysicsWorld;
                //���� ������ ��ƼƼ ã��
                NativeList<Entity> neighbors = new NativeList<Entity>(0, Allocator.Temp);
                NativeList<DistanceHit> hits = new NativeList<DistanceHit>(0, Allocator.Temp);
                if (world.OverlapSphere(flockingAspect.Position, NeighborDistance, ref hits, CollisionFilter.Default))
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hitEntity = hits[i].Entity;
                        if (flockingLookup.HasComponent(hitEntity))
                        {
                            if (hitEntity.Index != flockingAspect.entity.Index)
                            {
                                neighbors.Add(hitEntity);
                            }
                        }
                    }
                }
                var neighborsLength = neighbors.Length;
                flockingAspect.Velocity = math.normalizesafe(flockingValue.moveTarget - flockingAspect.Position);
                var velocityCalculator = flockingAspect.Velocity * flockingValue.speed;
                if (neighborsLength > 0)
                {
                    //���� ��� (�� ��ƼƼ�� �ֺ��� ��ƼƼ�� ���� �������� ���Ϸ��� Ư��
                    float3 aligment = new float3();
                    //���� ��� (�� ����Ƽ�� �ڱ� �ֺ��� ����Ƽ�� ������ ������ ����
                    float3 cohesion = new float3();
                    //�и� ��� (�� ����Ƽ�� �ʹ� ����� ���� ����������
                    float3 separation = new float3();

                    for (int i = 0; i < neighborsLength; i++)
                    {
                        var neighbor = neighbors[i];
                        var entity = flockingLookup[neighbor];

                        var neighborVelocity = entity.velocity;
                        aligment += neighborVelocity;
                        var neighborPosition = transformLookup[neighbor].Position;
                        cohesion += neighborPosition;

                        separation += math.normalizesafe(neighborPosition -flockingAspect.Position);
                    }

                    float insverseLength = 1 / (float)neighborsLength;
                    if (neighborsLength != 0)
                    {
                        aligment = math.normalizesafe(aligment * insverseLength);

                        cohesion = cohesion * insverseLength;
                        cohesion = math.normalizesafe(cohesion - flockingAspect.Position);

                        separation = math.normalizesafe(separation * -insverseLength);
                    }

                    velocityCalculator += aligment * AligmentWeight;
                    velocityCalculator += cohesion * CohesionWeight;
                    velocityCalculator += separation * SeparationWeight;
                    velocityCalculator.y = 0;
                }
                flockingAspect.Position += velocityCalculator * deltaTime;
                flockingAspect.LookAt = flockingValue.moveTarget;
            }
        }
    }
}
