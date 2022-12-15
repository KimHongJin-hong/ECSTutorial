using Unity.Entities;

public struct CharacterFire : IComponentData
{
    public Entity projectilePrefab;
    public Entity projectileSpawn;
    public float spawnTime;
    public float timer;
}