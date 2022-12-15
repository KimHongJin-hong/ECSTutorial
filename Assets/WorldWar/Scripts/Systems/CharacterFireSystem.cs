using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
partial struct CharacterFireSystem : ISystem
{
    public TransformAspect.Lookup transformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        transformLookup = new TransformAspect.Lookup(ref state, true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        transformLookup.Update(ref state);

        var commandBufferSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var fire = UnityEngine.Input.GetAxis("Fire1");
        float3 lookAtPoint = float3.zero;
        if (fire > 0)
        {
            var unityRay = CameraSingleton.Instance.ScreenPointToRay(UnityEngine.Input.mousePosition);

            RaycastInput ray = new RaycastInput
            {
                Start = unityRay.origin,
                End = unityRay.origin + unityRay.direction * 1000.0f,
                Filter = CollisionFilter.Default
            };

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            if (physicsWorld.CastRay(ray, out Unity.Physics.RaycastHit hit))
            {
                lookAtPoint = hit.Position;
            }
        }

        var job = new CharacterFireJob
        { 
            commandBuffer = commandBuffer,
            transformLookup = transformLookup,
            fireAxis = fire,
            deltaTime = SystemAPI.Time.DeltaTime,
            lookAtPoint = lookAtPoint,
        };

        job.Schedule();
    }

    [WithAll(typeof(CharacterFire))]
    [BurstCompile]
    partial struct CharacterFireJob : IJobEntity
    {
        public EntityCommandBuffer commandBuffer;
        [NativeDisableContainerSafetyRestriction]
        public TransformAspect.Lookup transformLookup;
        public float fireAxis;
        public float deltaTime;
        
        public float3 lookAtPoint;

        public void Execute(ref CharacterAspect fireAspect)
        {
            float spawnTime = fireAspect.SpawnTime;
            if (spawnTime == 0)
            {
                return;
            }

            if (fireAspect.Timer < spawnTime)
            {
                fireAspect.Timer += deltaTime;
            }

            if (fireAspect.Timer >= spawnTime)
            {
                if (fireAxis > 0)
                {
                    var instance = commandBuffer.Instantiate(fireAspect.ProjectilePrefab);
                    var spawnTransform = transformLookup[fireAspect.ProjectileSpawn];

                    spawnTransform.LookAt(new float3(lookAtPoint.x, spawnTransform.Position.y, lookAtPoint.z));
                    commandBuffer.SetComponent(instance, new Translation
                    {
                        Value = spawnTransform.Position
                    });

                    commandBuffer.SetComponent(instance, new Rotation
                    {
                        Value = spawnTransform.Rotation
                    });

                    fireAspect.Timer = 0;
                }
            }
        }
    }
}
