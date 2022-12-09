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
        public ComponentLookup<PhysicsCollider> ColliderGroup;
        public TransformAspect.Lookup transformLookup;

        public ComponentDataHandles(ref SystemState state)
        {
            ColliderGroup = state.GetComponentLookup<PhysicsCollider>(true);
            transformLookup = new TransformAspect.Lookup(ref state, true);
        }

        public void Update(ref SystemState state)
        {
            ColliderGroup.Update(ref state);
            transformLookup.Update(ref state);
        }

    }
    public void OnCreate(ref SystemState state)
    {
        // 시스템이 실행되려면 Trigger컴포넌트를 가진 엔티티가 하나 이상 존재해야함.
        state.RequireForUpdate(state.GetEntityQuery(ComponentType.ReadOnly<Trigger>()));
        handles = new ComponentDataHandles(ref state);

    }
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        handles.Update(ref state);
        state.Dependency = new TriggerJob
        {
            ColliderGroup = handles.ColliderGroup,
            transformLookup = handles.transformLookup,
            deltatTime = SystemAPI.Time.DeltaTime,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct TriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<PhysicsCollider> ColliderGroup;
        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] public TransformAspect.Lookup transformLookup;
        public float deltatTime;
        //[ReadOnly] public TransformAspect 

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            var transformA = transformLookup[entityA];
            var transformB = transformLookup[entityB];
            //ColliderGroup[entityA].MassProperties.Volume;

            float aVolume = ColliderGroup[entityA].MassProperties.Volume;
            float bVolume = ColliderGroup[entityB].MassProperties.Volume;
            float sumVolume = aVolume + bVolume;

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
    }
}