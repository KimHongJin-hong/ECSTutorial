using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[WithAll(typeof(CharacterMove))]
[UpdateBefore(typeof(TransformSystemGroup))]
partial struct CharacterMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var horizontal = UnityEngine.Input.GetAxis("Horizontal");
        var vertical = UnityEngine.Input.GetAxis("Vertical");
        var mouseViewport = CameraSingleton.ScreenToViewportPoint(UnityEngine.Input.mousePosition);
        var forward = (mouseViewport - (UnityEngine.Vector3.one * 0.5f)).normalized;

        //var rotation = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);
        CharacterMoveJob job = new CharacterMoveJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            axis = new float3(horizontal, 0, vertical),
            forward = forward,
        };

        job.Schedule();
    }


    [BurstCompile]
    [WithAll(typeof(CharacterMove))]
    partial struct CharacterMoveJob : IJobEntity
    {
        public float deltaTime;
        public float3 axis;
        public float3 forward;

        public void Execute(ref TransformAspect transform, ref CharacterMove characterController)
        {
            transform.LookAt(transform.Position + new float3(forward.x, 0, forward.y));
            //transform.TranslateLocal(math.mul(transform.Rotation, axis * characterController.speed * deltaTime));
            transform.TranslateLocal(axis * characterController.speed * deltaTime);
        }
    }
}

