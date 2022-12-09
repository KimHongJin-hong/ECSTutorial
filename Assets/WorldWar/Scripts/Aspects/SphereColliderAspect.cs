using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct SphereColliderAspect : IAspect
{
    public readonly Entity entity;

    private readonly TransformAspect transform;
    private readonly RefRO<SphereCollider> sphereCollider;

    public BoundingSphere Bound
    {
        get => sphereCollider.ValueRO.bound;
    }
    public float Radius
    {
        get { return sphereCollider.ValueRO.bound.radius; }
    }

    public float3 Position
    {
        get { return transform.Position; }
        set { transform.Position = value; }
    }
}
