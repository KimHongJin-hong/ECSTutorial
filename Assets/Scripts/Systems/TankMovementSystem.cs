using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial class TankMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltatTime = SystemAPI.Time.DeltaTime;

        Entities.WithAll<Tank>().ForEach((Entity entity, TransformAspect transform) =>
        {
            var position = transform.Position;
            //https://www.bit-101.com/blog/2021/07/mapping-perlin-noise-to-angles/ 펄른노이즈 각도 매핑
            position.y = entity.Index;
            var angle = (0.5f + noise.cnoise(position * 0.1f)) * 4.0f * math.PI;
            var dir = float3.zero;
            math.sincos(angle, out dir.x, out dir.z);
            transform.Position += dir * deltatTime * 5.0f;
            transform.Rotation = quaternion.RotateY(angle);
        }).ScheduleParallel();
    }
}