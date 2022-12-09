using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct TurretShootingSystem : ISystem
{
    //ComponentLookup<LocalToWorldTransform> m_localToWorldTransformFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //m_localToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorldTransform>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //m_localToWorldTransformFromEntity.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var turretShootJob = new TurretShoot
        {
            //m_localToWorldTransformFromEntity = m_localToWorldTransformFromEntity,
            ECB = ecb,
        };

        //turretShootJob.Schedule();
    }
}

[WithAll(typeof(Shooting))]
[BurstCompile]
partial struct TurretShoot : IJobEntity
{
    //[ReadOnly] public ComponentLookup<LocalToWorldTransform> m_localToWorldTransformFromEntity;
    public EntityCommandBuffer ECB;

    public void Execute(in TurretAspect turret)
    {
        var instance = ECB.Instantiate(turret.CannonBallPrefab);
        //var spawnLocalToWorld = m_localToWorldTransformFromEntity[turret.CannonBallSpawn];
        //var cannonBallTransform = UniformScaleTransform.FromPosition(spawnLocalToWorld.Value.Position);

        //cannonBallTransform.Scale = m_localToWorldTransformFromEntity[turret.CannonBallPrefab].Value.Scale;

        //ECB.SetComponent(instance, new LocalToWorldTransform { Value = cannonBallTransform });
        //ECB.SetComponent(instance, new CannonBall { speed = spawnLocalToWorld.Value.Forward() * 20.0f });
        ECB.SetComponent(instance, new URPMaterialPropertyBaseColor { Value = turret.Color });
    }
}