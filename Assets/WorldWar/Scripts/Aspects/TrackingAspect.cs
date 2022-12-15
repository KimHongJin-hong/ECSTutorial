using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct TrackingAspect : IAspect
{
    public readonly Entity entity;

    private readonly TransformAspect transform;
    private readonly RefRW<Tracking> tracking;

    public Tracking Value
    {
        get => tracking.ValueRO;
    }

    public float3 Velocity
    {
        get => tracking.ValueRO.velocity;
        set => tracking.ValueRW.velocity = value;
    }

    public float3 LookAt
    {
        set { transform.LookAt(value); }
    }

    public float3 Position
    {
        get { return transform.Position; }
        set { transform.Position = value; }
    }

    //public float3 Destination
    //{
    //    get { return tracking.ValueRO.destination; }
    //    set { tracking.ValueRW.destination = value; }
    //}

    //public float Speed
    //{
    //    get { return tracking.ValueRO.speed; }
    //}

    //public bool IsTracking
    //{
    //    get => tracking.ValueRO.isTracking;
    //    set => tracking.ValueRW.isTracking = value;
    //}
}
