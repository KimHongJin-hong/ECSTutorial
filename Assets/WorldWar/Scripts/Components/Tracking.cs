using Unity.Entities;
using Unity.Mathematics;

public struct Tracking : IComponentData
{
    /// <summary>
    /// 목적지
    /// </summary>
    public float3 moveTarget;
    /// <summary>
    /// 속력
    /// </summary>
    public float speed;
    /// <summary>
    /// 속도
    /// </summary>
    public float3 velocity;
    public float3 avoidVector;
    public float avoidRange;

    //public float3 destination;
    //public float speed;
    //public bool isTracking;
}
