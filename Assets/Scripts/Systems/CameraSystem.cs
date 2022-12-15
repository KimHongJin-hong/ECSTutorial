using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class CameraSystem : SystemBase
{
    private Entity target;
    private EntityQuery characterQuery;

    protected override void OnCreate()
    {
        characterQuery = GetEntityQuery(typeof(CharacterMove));
        RequireForUpdate(characterQuery);
    }

    protected override void OnStartRunning()
    {
        var character = characterQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
        var targetLookup = GetComponentLookup<CharacterMove>();
        target = character[0];
        SetSingleton(targetLookup[target]);
    }

    protected override void OnUpdate()
    {
        var cameraTrasnform = CameraSingleton.Instance.transform;
        var characterTransform = GetComponent<LocalToWorld>(target);
        cameraTrasnform.position = characterTransform.Position + new float3(0.0f, 10.0f, -10.0f);
        cameraTrasnform.LookAt(characterTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}
