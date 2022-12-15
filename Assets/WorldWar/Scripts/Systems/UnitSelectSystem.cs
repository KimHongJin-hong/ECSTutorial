//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Rendering;
//using UnityEngine;

//public partial class UnitSelectSystem : SystemBase
//{
//    private Vector3 startPosition;
//    private bool isDragStart;
//    public const float MaxDistance = 1000.0f;
//    private NativeList<Entity> selectedEntity;

//    protected override void OnCreate()
//    {
//        selectedEntity = new NativeList<Entity>(0, Allocator.Persistent);
//    }

//    protected override void OnUpdate()
//    {
//        // 마우스 시작점과 끝점으로 박스 오버랩 하여 박스안의 엔티티들을 선택하게 한다.
//        var mainCamera = Camera.main;
//        if (Input.GetMouseButtonDown(0))
//        {
//            startPosition = Input.mousePosition;

//            var unityRay = mainCamera.ScreenPointToRay(startPosition);
//            RaycastInput ray = new RaycastInput
//            {
//                Start = unityRay.origin,
//                End = unityRay.origin + unityRay.direction * MaxDistance,
//                Filter = CollisionFilter.Default
//            };
//            var world = GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
//            isDragStart = world.CastRay(ray, out Unity.Physics.RaycastHit hit);
//            if (isDragStart)
//            {
//                startPosition = hit.Position;
//            }
//        }
//        if (Input.GetMouseButtonUp(0))
//        {
//            if (isDragStart)
//            {
//                Vector2 endMousePosition = Input.mousePosition;
//                var unityRay = mainCamera.ScreenPointToRay(endMousePosition);
//                RaycastInput ray = new RaycastInput
//                {
//                    Start = unityRay.origin,
//                    End = unityRay.origin + unityRay.direction * MaxDistance,
//                    Filter = CollisionFilter.Default
//                };
//                var world = GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
//                if (world.CastRay(ray, out Unity.Physics.RaycastHit hit))
//                {
//                    var endPosition = hit.Position;
//                    float3 upper = math.max(startPosition, endPosition);
//                    float3 lower = math.min(startPosition, endPosition);

//                    float3 extents = upper - lower;
//                    float3 halfExtents = extents * 0.5f;
//                    halfExtents.y = 1;
//                    float3 center = new float3(lower) + halfExtents;

//                    NativeList<DistanceHit> hits = new NativeList<DistanceHit>(0, Allocator.Temp);
//                    if (world.OverlapBox(center, quaternion.identity, halfExtents, ref hits, CollisionFilter.Default))
//                    {
//                        if (selectedEntity.Length > 0)
//                        {
//                            for (int i = 0; i < selectedEntity.Length; ++i)
//                            {
//                                SystemAPI.SetComponent(selectedEntity[i], new URPMaterialPropertyBaseColor { Value = new float4(1, 1, 1, 1) });
//                            }
//                            selectedEntity.Clear();
//                        }
//                        for (int i = 0; i < hits.Length; ++i)
//                        {
//                            if (SystemAPI.HasComponent<Flocking>(hits[i].Entity))
//                            {
//                                selectedEntity.Add(hits[i].Entity);
//                                SystemAPI.SetComponent(hits[i].Entity, new URPMaterialPropertyBaseColor { Value = new float4(1, 0, 0, 1) });
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        if (Input.GetMouseButtonDown(1))
//        {
//            if (selectedEntity.Length > 0)
//            {
//                Vector2 endMousePosition = Input.mousePosition;
//                var unityRay = mainCamera.ScreenPointToRay(endMousePosition);
//                RaycastInput ray = new RaycastInput
//                {
//                    Start = unityRay.origin,
//                    End = unityRay.origin + unityRay.direction * MaxDistance,
//                    Filter = CollisionFilter.Default
//                };
//                var world = GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
//                if (world.CastRay(ray, out Unity.Physics.RaycastHit hit))
//                {
//                    for (int i = 0; i < selectedEntity.Length; ++i)
//                    {
//                        SystemAPI.SetComponent(selectedEntity[i], new Flocking
//                        {
//                            moveTarget = hit.Position,
//                            speed = 10,
//                        });
//                    }
//                }
//            }
//        }
//    }
//}
