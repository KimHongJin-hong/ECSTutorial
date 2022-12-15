using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CharacterControllerAuthoring : MonoBehaviour
{
    public float speed = 0;

    public GameObject projectilePrefab;
    public Transform spawnTransform;

    public class CharacterControllerBaker : Baker<CharacterControllerAuthoring>
    {
        public override void Bake(CharacterControllerAuthoring authoring)
        {
            AddComponent(new CharacterMove { speed = authoring.speed });
            AddComponent(new CharacterFire
            {
                projectilePrefab = GetEntity(authoring.projectilePrefab),
                projectileSpawn = GetEntity(authoring.spawnTransform),
                spawnTime = 0.5f,
                timer = 0,
            });
        }
    }
}
