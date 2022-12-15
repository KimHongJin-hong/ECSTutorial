using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;
using Unity.Transforms;

[RequireMatchingQueriesForUpdate] //EntityQuery가 비어있으면 OnUpdate을 실행하지 않게
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct TriggerSystem : ISystem
{
    private ComponentDataHandles handles;

    private struct ComponentDataHandles
    {
        public TransformAspect.Lookup transformLookup;
        public ComponentLookup<Push> pushLookup;
        public ComponentLookup<Hit> hitLookup;
        public ComponentLookup<OnHit> onHitLookup;


        public ComponentDataHandles(ref SystemState state)
        {
            transformLookup = new TransformAspect.Lookup(ref state, true);
            pushLookup = state.GetComponentLookup<Push>();
            hitLookup = state.GetComponentLookup<Hit>();
            onHitLookup = state.GetComponentLookup<OnHit>();
        }

        public void Update(ref SystemState state)
        {
            transformLookup.Update(ref state);
            pushLookup.Update(ref state);
            hitLookup.Update(ref state);
            onHitLookup.Update(ref state);
        }

    }
    public void OnCreate(ref SystemState state)
    {
        handles = new ComponentDataHandles(ref state);

    }
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        handles.Update(ref state);
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        state.Dependency = new TriggerJob
        {
            transformLookup = handles.transformLookup,
            pushLookup = handles.pushLookup,
            deltatTime = SystemAPI.Time.DeltaTime,
            hitLookup = handles.hitLookup,
            onHitLookup = handles.onHitLookup,
            commandBuffer = ecb,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct TriggerJob : ITriggerEventsJob
    {
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public TransformAspect.Lookup transformLookup;
        public float deltatTime;
        public ComponentLookup<Push> pushLookup;
        public ComponentLookup<Hit> hitLookup;
        public ComponentLookup<OnHit> onHitLookup;
        //[ReadOnly] public TransformAspect 
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            // 둘다 push를 가지고 있을 때 서로 밀 수 있게.
            if (pushLookup.HasComponent(entityA) && pushLookup.HasComponent(entityB))
            {
                var transformA = transformLookup[entityA];
                var transformB = transformLookup[entityB];

                float3 positionDiff = transformA.Position - transformB.Position;
                float distance = math.lengthsq(positionDiff);

                if (distance == 0)
                {
                    positionDiff = new float3(1, 0, 0);
                }

                float power = 5;
                float3 result = math.normalize(positionDiff) * power * deltatTime;
                result.y = 0;
                transformLookup[entityA].TranslateLocal(result);
                transformLookup[entityB].TranslateLocal(-result);
            }

            bool hasHitA = hitLookup.HasComponent(entityA);
            bool hasHitB = hitLookup.HasComponent(entityB);
            bool hasOnHitA = onHitLookup.HasComponent(entityA);
            bool hasOnHitB = onHitLookup.HasComponent(entityB);

            if (hasHitA && hasOnHitB)
            {
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
            if (hasHitB && hasOnHitA)
            {
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
        }
    }
}