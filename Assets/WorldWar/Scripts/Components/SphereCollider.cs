using Unity.Entities;

/// <summary>
/// 구형 충돌체
/// </summary>
public struct SphereCollider : IComponentData
{
    public BoundingSphere bound;
}
