using Unity.Entities;
using Unity.Rendering;

public class CannonBallAuthoring : UnityEngine.MonoBehaviour
{
}

public class ConnonBallBaker : Baker<CannonBallAuthoring>
{
    public override void Bake(CannonBallAuthoring authoring)
    {
        AddComponent<CannonBall>();
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}
