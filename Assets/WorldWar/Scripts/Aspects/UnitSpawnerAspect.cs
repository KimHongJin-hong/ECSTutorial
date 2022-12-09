using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// 유닛 스포너 데이터 컨트롤
/// </summary>
public readonly partial struct UnitSpawnerAspect : IAspect
{
    //public readonly Entity entity;

    private readonly RefRW<UnitSpawner> unitSpawner;

    public Entity UnitPrefab
    {
        get { return unitSpawner.ValueRO.unitPrefab; }
    }

    public float SpawnTime
    {
        get { return unitSpawner.ValueRO.spawnTime; }
    }

    public float Timer
    {
        get { return unitSpawner.ValueRO.timer; }
        set { unitSpawner.ValueRW.timer = value; }
    }

    public uint Seed { get => unitSpawner.ValueRO.seed; }

    public float3 RandomPosition
    {
        get 
        {
            unitSpawner.ValueRW.seed += 1;
            unitSpawner.ValueRW.random = Random.CreateFromIndex(unitSpawner.ValueRO.seed);
            return unitSpawner.ValueRO.random.NextFloat3(new float3(-1.0f, 0, -1.0f), new float3(1.0f, 0, 1.0f)); 
        }
    }

    public uint MaxSpawnUnit
    {
        get { return unitSpawner.ValueRO.maxSpawnUnit; }
    }
}
