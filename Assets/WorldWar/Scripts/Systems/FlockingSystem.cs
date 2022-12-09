using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


[RequireMatchingQueriesForUpdate] //EntityQuery가 비어있으면 OnUpdate을 실행하지 않게
[BurstCompile]
public partial struct FlockingSystem : ISystem
{
    public TransformAspect.Lookup transformLookup;
    public ComponentLookup<Flocking> flockingLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Flocking>();
        transformLookup = new TransformAspect.Lookup(ref state, true);
        flockingLookup = state.GetComponentLookup<Flocking>();
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
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var trackingJob = new TrackingJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            transformLookup = transformLookup,
            physicsWorldSingleton = physicsWorldSingleton,
            flockingLookup = flockingLookup
        };

        trackingJob.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Flocking))]
    [BurstCompile]
    [UpdateBefore(typeof(TriggerSystem))]
    private partial struct TrackingJob : IJobEntity
    {
        public float deltaTime;
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public TransformAspect.Lookup transformLookup;
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<Flocking> flockingLookup;
        [NativeDisableContainerSafetyRestriction]
        public PhysicsWorldSingleton physicsWorldSingleton;
        public const float NeighborDistance = 5f;
        public const float AligmentWeight = 1;
        public const float CohesionWeight = 2;
        public const float SeparationWeight = 3;
        private void Execute(ref TrackingAspect flockingAspect)
        {
            var flockingValue = flockingAspect.Value;
            if (math.lengthsq(flockingAspect.Position - flockingValue.moveTarget) > 1)
            {
                var world = physicsWorldSingleton.PhysicsWorld;
                //나와 인접한 엔티티 찾기
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
                    //정렬 계산 (각 엔티티는 주변의 엔티티와 같은 방향으로 향하려는 특성
                    float3 aligment = new float3();
                    //응집 계산 (각 엔터티는 자기 주변의 엔터티의 곁으로 가려는 성질
                    float3 cohesion = new float3();
                    //분리 계산 (각 엔터티는 너무 가까워 지지 않으려고함
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


            //if (tracking.IsTracking)
            //{
            //    //tracking.Destination = new float3(0, 0, 0);
            //    var distance = math.lengthsq(tracking.Destination - tracking.Position);

            //    if (distance > 0)
            //    {
            //        var dir = math.normalize(tracking.Destination - tracking.Position);
            //        transformLookup[tracking.entity].LookAt(tracking.Destination);

            //        transformLookup[tracking.entity].TranslateWorld(dir * tracking.Speed * deltaTime);
            //    }
            //}
            //translation[tracking.entity].Value = translation[tracking.entity].Value + dir;

            //var dir = math.normalize(tracking.Destination - tracking.Position);
            //tracking.LookAt = tracking.Destination;
            //tracking.Position += dir * tracking.Speed * deltaTime;
        }
    }
}
