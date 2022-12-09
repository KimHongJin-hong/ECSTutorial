using Unity.Entities;

public struct Turret : IComponentData
{
    public Entity cannonBallSpawn;

    public Entity cannonBallPrefab;
}
