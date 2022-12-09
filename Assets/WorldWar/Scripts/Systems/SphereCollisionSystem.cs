using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
public partial class SphereCollisionSystem : SystemBase
{
    private BVHNode root;

    public void Insert(BoundingSphere bound, SphereColliderAspect aspect)
    {
        if (root == null)
        {
            root = new BVHNode(null, bound, aspect);
        }
        else
        {
            root.Insert(bound, aspect);
        }
    }

    public void UpdateTree()
    {
        if (root != null)
        {
            root.Update();
        }
    }

    protected override void OnUpdate()
    {

        //foreach (var collider in SystemAPI.Query<SphereColliderAspect>())
        //{
        //    Insert(collider.Bound, collider);
        //    //BVHTree.instance.UpdateTree();
        //}
        //UpdateTree();
        //root.Remove();
        //root = null;
        //NativeMultiHashMap<int, int> collisions = new NativeMultiHashMap<int, int>(0, Allocator.Temp);
        //NativeList<SphereColliderAspect> colliders = new NativeList<SphereColliderAspect>(0, Allocator.Temp);//= new List<SphereColliderAspect>();
        //foreach (var collider in SystemAPI.Query<SphereColliderAspect>())
        //{
        //    collisions.Clear();
        //    colliders.Add(collider);

        //}
        //var size = colliders.Length;
        //for (int i = 0; i < size; i++)
        //{
        //    var aCollider = colliders[i];
        //    for (int j = i + 1; j < size; j++)
        //    {
        //        var iter = collisions.GetValuesForKey(i);
        //        bool isCollision = false;
        //        while (iter.MoveNext())
        //        {
        //            if (iter.Current == j)
        //            {
        //                isCollision = true;
        //                break;
        //            }
        //        }
        //        if (isCollision)
        //        {
        //            continue;
        //        }


        //        var bCollider = colliders[j];
        //        float3 aPosition = aCollider.Position;
        //        float3 bPosition = bCollider.Position;

        //        float aRadius = aCollider.Radius;
        //        float bRadius = bCollider.Radius;
        //        float radius = aRadius + bRadius;

        //        float3 positionDiff = aPosition - bPosition;
        //        float distance = math.length(positionDiff);

        //        if (distance == 0)
        //        {
        //            positionDiff = new float3(radius, 0, 0);
        //        }

        //        if (distance < radius)
        //        {
        //            collisions.Add(i, j);
        //            collisions.Add(j, i);
        //            float power = (radius - distance) * 0.5f;
        //            float3 dir = math.normalize(positionDiff) * power;
        //            aCollider.Position = aPosition + dir;
        //            bCollider.Position = bPosition - dir;
        //        }
        //    }
        //}
    }
}
