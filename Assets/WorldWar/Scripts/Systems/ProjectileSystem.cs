using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
partial struct ProjectileSystem : ISystem
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
        var job = new Proejctilejob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        job.ScheduleParallel();
    }

    [WithAll(typeof(Projectile))]
    [BurstCompile]
    partial struct Proejctilejob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref TransformAspect transform, in Projectile projectile)
        {
            transform.Position += projectile.speed * deltaTime * transform.Forward;
        }
    }
}
