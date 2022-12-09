using Unity.Mathematics;

public struct BoundingSphere
{
    private float3 position;

    public float3 GetPosition
    {
        get => position;
    }
    public float radius;

    public BoundingSphere(float3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }

    public BoundingSphere(BoundingSphere a, BoundingSphere b)
    {
        float3 centerOffset = b.GetPosition - a.GetPosition;
        float distance = math.lengthsq(centerOffset);
        float radiusDiff = b.radius - a.radius;

        if (radiusDiff * radiusDiff >= distance)
        {
            //A 구가 B 구안에 정확히 포함되어있을 때
            if (a.radius > b.radius)
            {
                position = a.GetPosition;
                radius = a.radius;
            }
            else
            {
                position = b.GetPosition;
                radius = b.radius;
            }
        }
        //아닐경우 두 구를 포함하는 새로운 구를 만든다.
        else
        {
            distance = (float)System.Math.Sqrt(distance);
            radius = (distance + a.radius + b.radius) * 0.5f;

            position = a.GetPosition;

            if (distance > 0)
            {
                position += centerOffset * ((radius - a.radius) / distance);
            }
        }
    }

    public bool Overlaps(BoundingSphere sphere)
    {
        float distance = math.lengthsq(GetPosition - sphere.GetPosition);
        return distance < (radius + sphere.radius) * (radius + sphere.radius);
    }

    public bool OverlapsFat(BoundingSphere sphere, float fat)
    {
        float distance = math.lengthsq(GetPosition - sphere.GetPosition);
        return (distance) < (radius + sphere.radius - fat) * (radius + sphere.radius - fat);
    }

    public float GetGrowth(BoundingSphere other)
    {
        BoundingSphere newSphere = new BoundingSphere(this, other);
        return (newSphere.radius * newSphere.radius) - (radius * radius);
    }

    public float GetSize()
    {
        return 1.333333f * (float)math.PI * radius * radius * radius;
    }
}

public class BVHNode
{
    public BVHNode parent;
    public BVHNode left;
    public BVHNode right;

    public BoundingSphere sphere;
    public SphereColliderAspect body;
    public bool isLeaf;

    public BVHNode()
    {

    }

    public BVHNode(BVHNode parent, BoundingSphere boundingSphere, SphereColliderAspect body)
    {
        this.parent = parent;
        this.sphere = boundingSphere;
        this.body = body;
        isLeaf = true;
    }

    public bool IsLeaf()
    {
        return isLeaf;
    }

    public bool Overlaps(BVHNode other)
    {
        return sphere.Overlaps(other.sphere);
    }

    public BVHNode Insert(BoundingSphere newBoundingSphere, SphereColliderAspect newBody)
    {
        if (IsLeaf())
        {
            left = new BVHNode(this, sphere, body);
            right = new BVHNode(this, newBoundingSphere, newBody);
            isLeaf = false;
            //RecalculateSphere();
            return right;
        }
        else
        {
            //더 가까운 공간 찾아라..
            if (left.sphere.GetGrowth(newBoundingSphere) < right.sphere.GetGrowth(newBoundingSphere))
            {
                return left.Insert(newBoundingSphere, newBody);
            }
            else
            {
                return right.Insert(newBoundingSphere, newBody);
            }
        }
    }

    public void Remove()
    {
        if (parent != null)
        {
            BVHNode sibling = GetSibling();

            if (sibling != null)
            {
                parent.sphere = sibling.sphere;
                parent.body = sibling.body;
                parent.left = sibling.left;
                parent.right = sibling.right;

                if (parent.left != null)
                {
                    parent.left.parent = parent;
                }
                if (parent.right != null)
                {
                    parent.right.parent = parent;
                }


                sibling.parent = null;
                sibling.left = null;
                sibling.right = null;
                sibling.Remove();
                parent.RecalculateSphere();
            }
        }
        if (left != null)
        {
            left.parent = null;
            left.Remove();
        }

        if (right != null)
        {
            right.parent = null;
            right.Remove();
        }
    }

    public void RecalculateSphere()
    {
        if (IsLeaf()) return;

        sphere = new BoundingSphere(left.sphere, right.sphere);

        if (parent != null) parent.RecalculateSphere();
    }

    public void RecalulateSphereDown()
    {
        if (IsLeaf()) return;

        sphere = new BoundingSphere(left.sphere, right.sphere);

        if (left != null) left.RecalulateSphereDown();
        if (right != null) right.RecalulateSphereDown();
    }

    public BVHNode GetSibling()
    {
        if (parent == null)
        {
            return null;
        }
        return this == parent.left ? parent.right : parent.left;
    }

    public void Update()
    {
        FindPotentialCollision();
        if (IsLeaf())
        {
            if (parent != null && !parent.sphere.OverlapsFat(sphere, sphere.radius * 2f))
            //if (parent != null && !parent.sphere.Overlaps(sphere))
            {
                Remove();
                var root = GetRoot();
                root.Insert(sphere, body);
            }
            else
            {
                //FindCollision(this);
            }
        }
        else
        {
            if (left != null)
                left.Update();
            if (right != null)
                right.Update();
        }
    }

    public BVHNode GetRoot()
    {
        if (parent != null)
            return parent.GetRoot();
        return this;
    }

    public void FindCollision(BVHNode node)
    {
        var sibling = GetSibling();

        if (sibling == null)
        {
            return;
        }

        if (node.sphere.Overlaps(sibling.sphere))
        {
            if (sibling.IsLeaf() == false)
            {
                if (sibling.left.sphere.GetGrowth(node.sphere) < sibling.right.sphere.GetGrowth(node.sphere))
                {
                    sibling.right.FindCollision(node);
                }
                else
                {
                    sibling.left.FindCollision(node);
                }
            }
            else
            {
                float3 dir = math.lengthsq(node.body.Position - sibling.body.Position);
                float power = node.sphere.GetGrowth(sibling.sphere) * 0.5f;
                float3 deltaPosition = dir * power;
                node.body.Position += deltaPosition;
                sibling.body.Position -= deltaPosition;
            }
        }
    }

    public void FindPotentialCollision()
    {
        if (IsLeaf())
        {
            FindCollision(this);
            return;
        }

        left.FindPotentialCollisionWith(right);
    }

    public void FindPotentialCollisionWith(BVHNode other)
    {
        if (!Overlaps(other))
            return;

        if (IsLeaf() && other.IsLeaf())
        {
            float3 dir = math.normalize(body.Position - other.body.Position);
            float power = sphere.GetGrowth(other.sphere) * 0.5f;
            float3 deltaPosition = dir * power;
            body.Position += deltaPosition;
            other.body.Position -= deltaPosition;

            return;
        }

        if (other.IsLeaf() || (!IsLeaf() && sphere.GetSize() >= other.sphere.GetSize()))
        {
            if (left != null) left.FindPotentialCollisionWith(other);
            if (right != null) right.FindPotentialCollisionWith(other);
        }
        else
        {
            if (other.left != null) FindPotentialCollisionWith(other.left);
            if (other.right != null) FindPotentialCollisionWith(other.right);
        }
    }
}