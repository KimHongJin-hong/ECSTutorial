using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// 유닛을 스폰시키는 시스템
/// </summary>
[WithAll(typeof(UnitSpawner))]
[BurstCompile]
public partial struct UnitSpawnJob : IJobEntity
{
    public EntityCommandBuffer entityCommandBuffer;
    public float deltaTime;
    public NativeList<int> spawnedEntities;

    private void Execute(ref UnitSpawnerAspect unitSpawner)
    {
        if (spawnedEntities.Length >= unitSpawner.MaxSpawnUnit)
        {
            unitSpawner.Timer = 0;
            return;
        }
        if (unitSpawner.SpawnTime == 0)
        {
            return;
        }


        unitSpawner.Timer += deltaTime;

        if (unitSpawner.Timer >= unitSpawner.SpawnTime)
        {
            unitSpawner.Timer -= unitSpawner.SpawnTime;
            var instance = entityCommandBuffer.Instantiate(unitSpawner.UnitPrefab);
            
            var position = unitSpawner.RandomPosition;
            var transform = UniformScaleTransform.FromPosition(position);
            //entityCommandBuffer.SetComponent(instance, new LocalToWorldTransform { Value = transform });
            spawnedEntities.Add(instance.Index);
        }
    }
}

[BurstCompile]
public partial struct UnitSpawningSystem : ISystem
{
    private NativeList<int> spawnedEntities;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        spawnedEntities = new NativeList<int>(0, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        spawnedEntities.Clear();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //BeginSimulationEntityCommandBufferSystem에 스폰 시킬 커맨드버퍼를 추가한다.
        var entityCommandBufferSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var entityCommandBuffer = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var unitSpawnJob = new UnitSpawnJob
        {
            entityCommandBuffer = entityCommandBuffer,
            deltaTime = SystemAPI.Time.DeltaTime,
            spawnedEntities = spawnedEntities,
        };

        unitSpawnJob.Schedule();
    }
}
