using Unity.Entities;
using Unity.Mathematics;

public struct Tracking : IComponentData
{
    /// <summary>
    /// 格利瘤
    /// </summary>
    public float3 moveTarget;
    /// <summary>
    /// 加仿
    /// </summary>
    public float speed;
    /// <summary>
    /// 加档
    /// </summary>
    public float3 velocity;
    public float3 avoidVector;
    public float avoidRange;

    //public float3 destination;
    //public float speed;
    //public bool isTracking;
}
