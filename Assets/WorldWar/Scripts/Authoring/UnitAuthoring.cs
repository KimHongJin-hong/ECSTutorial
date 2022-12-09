using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public float radius;
    public float speed;

    public class UnitBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            //AddComponent(new SphereCollider
            //{
            //    bound = new BoundingSphere(Unity.Mathematics.float3.zero, authoring.radius)
            //});
            AddComponent(new Flocking
            {
                moveTarget = new Unity.Mathematics.float3(100, 0, 0),
                speed = authoring.speed
            });
            AddComponent(new Trigger());
        }
    }
}