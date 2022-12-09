using Unity.Entities;

public class TankAuthoring : UnityEngine.MonoBehaviour
{
}

public class TankBake : Baker<TankAuthoring>
{
    public override void Bake(TankAuthoring authoring)
    {
        AddComponent<Tank>();
    }
}