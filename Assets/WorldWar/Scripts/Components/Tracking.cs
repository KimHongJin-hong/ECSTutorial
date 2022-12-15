using Unity.Entities;
using Unity.Mathematics;

public struct Tracking : IComponentData
{
    /// <summary>
    /// ������
    /// </summary>
    public float3 moveTarget;
    /// <summary>
    /// �ӷ�
    /// </summary>
    public float speed;
    /// <summary>
    /// �ӵ�
    /// </summary>
    public float3 velocity;
    public float3 avoidVector;
    public float avoidRange;

    //public float3 destination;
    //public float speed;
    //public bool isTracking;
}
