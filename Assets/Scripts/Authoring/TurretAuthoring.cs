using Unity.Entities;

public class TurretAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject cannonBallPrefab;
    public UnityEngine.Transform cannonBallSpawn;
}

class TurretBaker : Baker<TurretAuthoring>
{
    public override void Bake(TurretAuthoring authoring)
    {
        AddComponent(new Turret()
        {
            cannonBallPrefab = GetEntity(authoring.cannonBallPrefab),
            cannonBallSpawn = GetEntity(authoring.cannonBallSpawn)
        });

        AddComponent<Shooting>();
    }
}