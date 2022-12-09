using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WithAll(typeof(Turret))]
[BurstCompile]
partial struct SafeZoneJob : IJobEntity
{
    // 병렬 안전 검사 하지 않음 (공유자원관리)
    [NativeDisableParallelForRestriction] public ComponentLookup<Shooting> TurretActiveFromEntity;

    public float SquaredRadius;

    private void Execute(Entity entity, TransformAspect transform)
    {
        TurretActiveFromEntity.SetComponentEnabled(entity, math.lengthsq(transform.Position) > SquaredRadius);
    }
}

[BurstCompile]
partial struct SafeZoneSystem : ISystem
{
    private ComponentLookup<Shooting> m_TurretActiveFromEntity;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        m_TurretActiveFromEntity = state.GetComponentLookup<Shooting>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
        const float debugRenderStepInDegree = 20;

        for (float angle = 0; angle < 360; angle += debugRenderStepInDegree)
        {
            var a = float3.zero;
            var b = float3.zero;
            math.sincos(math.radians(angle), out a.x, out a.z);
            math.sincos(math.radians(angle + debugRenderStepInDegree), out b.x, out b.z);
            UnityEngine.Debug.DrawLine(a * radius, b * radius);
        }

        m_TurretActiveFromEntity.Update(ref state);
        var safeZoneJob = new SafeZoneJob
        {
            TurretActiveFromEntity = m_TurretActiveFromEntity,
            SquaredRadius = radius * radius
        };

        safeZoneJob.ScheduleParallel();
    }
}
