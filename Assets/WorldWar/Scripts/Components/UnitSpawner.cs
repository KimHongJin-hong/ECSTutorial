using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

/// <summary>
/// 유닛 스폰 컴포넌트
/// </summary>
public struct UnitSpawner : IComponentData
{
    public Entity unitPrefab;
    public float spawnTime;
    public float timer;
    public uint seed;
    public uint maxSpawnUnit;
    public Random random;
}
