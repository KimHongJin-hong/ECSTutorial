using UnityEngine;
using Unity.Entities;
using Unity.Collections;

/// <summary>
/// 유닛 스포너 베이크
/// </summary>
public class UnitSpawnerAuthoring : MonoBehaviour
{
    public GameObject unitPrefab;
    public float spawnTime;
    public uint maxSpawnUnit;

    public class UnitSpawnerBaker : Baker<UnitSpawnerAuthoring>
    {
        public override void Bake(UnitSpawnerAuthoring authoring)
        {
            AddComponent(new UnitSpawner
            {
                unitPrefab = GetEntity(authoring.unitPrefab),
                spawnTime = authoring.spawnTime,
                maxSpawnUnit = authoring.maxSpawnUnit,
                //spawnedEntities = new Unity.Collections.NativeList<Entity>(0, Allocator.Persistent),
                random = Unity.Mathematics.Random.CreateFromIndex(1234)
            });
        }
    }
}
