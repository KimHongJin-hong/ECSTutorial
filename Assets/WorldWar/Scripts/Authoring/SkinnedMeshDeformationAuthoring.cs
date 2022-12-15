using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

internal struct BoneEntity : IBufferElementData
{
    public Entity Value;
}

internal struct RootEntity : IComponentData
{
    public Entity Value;
}

internal struct BindPose : IBufferElementData
{
    public float4x4 Value;
}

/// <summary>
/// bone
/// </summary>
public class SkinnedMeshDeformationAuthoring : MonoBehaviour
{

    public class AnimatorBaker : Baker<SkinnedMeshDeformationAuthoring>
    {
        public override void Bake(SkinnedMeshDeformationAuthoring authoring)
        {
            var skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>(authoring);
            if (skinnedMeshRenderer == null)
                return;

            DependsOn(skinnedMeshRenderer.sharedMesh);
            var hasSkinning = skinnedMeshRenderer.bones.Length > 0 && skinnedMeshRenderer.sharedMesh.bindposes.Length > 0;
            if (hasSkinning)
            {
                // Setup reference to the root bone
                var rootTransform = skinnedMeshRenderer.rootBone ? skinnedMeshRenderer.rootBone : skinnedMeshRenderer.transform;
                var rootEntity = GetEntity(rootTransform);
                AddComponent(new RootEntity { Value = rootEntity });

                // Setup reference to the other bones
                var boneEntityArray = AddBuffer<BoneEntity>();
                boneEntityArray.ResizeUninitialized(skinnedMeshRenderer.bones.Length);

                for (int boneIndex = 0; boneIndex < skinnedMeshRenderer.bones.Length; ++boneIndex)
                {
                    var bone = skinnedMeshRenderer.bones[boneIndex];
                    var boneEntity = GetEntity(bone);
                    boneEntityArray[boneIndex] = new BoneEntity { Value = boneEntity };
                }

                // Store the bindpose for each bone
                var bindPoseArray = AddBuffer<BindPose>();
                bindPoseArray.ResizeUninitialized(skinnedMeshRenderer.bones.Length);

                for (int boneIndex = 0; boneIndex != skinnedMeshRenderer.bones.Length; ++boneIndex)
                {
                    var bindPose = skinnedMeshRenderer.sharedMesh.bindposes[boneIndex];
                    bindPoseArray[boneIndex] = new BindPose { Value = bindPose };
                }
            }
        }
    }
}
