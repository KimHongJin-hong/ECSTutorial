using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct CharacterAspect : IAspect
{
    public readonly Entity entity;
    private readonly RefRW<CharacterFire> characterFire;

    public float SpawnTime
    {
        get => characterFire.ValueRO.spawnTime;
    }

    public float Timer
    {
        get => characterFire.ValueRO.timer;
        set => characterFire.ValueRW.timer = value;
    }

    public Entity ProjectilePrefab
    {
        get => characterFire.ValueRO.projectilePrefab;
    }

    public Entity ProjectileSpawn
    {
        get => characterFire.ValueRO.projectileSpawn;
    }
}
