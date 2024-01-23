using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UDebug = UnityEngine.Debug;

namespace GBG.AnimationGraph.Component
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Skeleton : MonoBehaviour
    {
        [Tooltip("Bones under Animator.")]
        [SerializeField]
        [NonReorderable]
        private Transform[] _bones = Array.Empty<Transform>();

        private Animator _animator;

        private NativeArray<BoneInfo> _boneInfos;


        private void Reset()
        {
            // CollectBonesFromHierarchy(this, true);
            CollectBonesFromAvatar(this, true);
        }

        private void Start()
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                if (!_bones[i])
                {
                    UDebug.LogError($"[Puppeteer::Skeleton] Skeleton bone at index '{i}' is null.");
                }
            }

            // GetOrAllocateBoneInfos();
        }
        
        public BoneInfo GetBoneInfo(string boneName)
        {
            var boneInfos = GetOrAllocateBoneInfos();
            for (int i = 0; i < boneInfos.Length; i++)
            {
                if (boneInfos[i].BoneNameHash == Animator.StringToHash(boneName))
                {
                    return boneInfos[i];
                }
            }

            throw new Exception("Bone not found.");
        }

        private void OnDestroy()
        {
            if (_boneInfos.IsCreated)
            {
                _boneInfos.Dispose();
            }
        }

        /// <summary>
        /// Get or allocate a instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.
        /// </summary>
        /// <param name="nameToHash">Method for calculate hash from name.</param>
        /// <returns>The instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.</returns>
        public NativeArray<BoneInfo> GetOrAllocateBoneInfos()
        {
            if (!_boneInfos.IsCreated)
            {
                if (!_animator) _animator = GetComponent<Animator>();
                _boneInfos = BoneInfo.AllocateBoneInfos(_animator, _bones);
            }

            return _boneInfos;
        }

        public static bool CollectBonesFromHierarchy(Skeleton skeleton, bool noRenderer)
        {
            var hierarchy = new List<Transform>(200);
            if (!noRenderer || !skeleton.GetComponent<Renderer>())
            {
                hierarchy.Add(skeleton.transform);
            }

            GetChildren(skeleton.transform, hierarchy, noRenderer);
            skeleton._bones = hierarchy.ToArray();

            return true;

            static void GetChildren(Transform node, List<Transform> result, bool noRenderer)
            {
                foreach (Transform child in node)
                {
                    if (child == node)
                    {
                        continue;
                    }

                    if (!noRenderer || !child.GetComponent<Renderer>())
                    {
                        result.Add(child);
                    }

                    GetChildren(child, result, noRenderer);
                }
            }
        }

        public static bool CollectBonesFromAvatar(Skeleton skeleton, bool noRenderer)
        {
            var avatar = skeleton.GetComponent<Animator>().avatar;
            if (!avatar)
            {
                return false;
            }

            var boneDict = new Dictionary<string, Transform>(skeleton._bones.Length);
            GetChildren(skeleton.transform, boneDict, noRenderer);

            var skeletonBones = avatar.humanDescription.skeleton;
            var bones = new List<Transform>(skeletonBones.Length);
            for (int i = 0; i < skeletonBones.Length; i++)
            {
                if (!boneDict.TryGetValue(skeletonBones[i].name, out var bone))
                {
                    UDebug.LogError($"[Puppeteer::Skeleton] Bone '{skeletonBones[i].name}' not found.");
                    continue;
                }

                bones.Add(bone);
            }

            skeleton._bones = bones.ToArray();

            return true;

            static void GetChildren(Transform node, Dictionary<string, Transform> result, bool noRenderer)
            {
                foreach (Transform child in node)
                {
                    if (child == node)
                    {
                        continue;
                    }

                    if (!noRenderer || !child.GetComponent<Renderer>())
                    {
                        result.Add(child.name, child);
                    }

                    GetChildren(child, result, noRenderer);
                }
            }
        }
    }
}

public readonly struct BoneInfo
    {
        public TransformStreamHandle BoneHandle { get; }

        public float BoneWeight { get; }

        public int BoneNameHash { get; }

        public int ParentIndex { get; }

        public bool IsValid { get; }

        public BoneInfo(TransformStreamHandle boneHandle, float boneWeight, int boneNameHash, 
            int parentIndex)
        {
            BoneHandle = boneHandle;
            BoneWeight = boneWeight;
            BoneNameHash = boneNameHash;
            ParentIndex = parentIndex;
            IsValid = BoneNameHash != 0;
        }

        /// <summary>
        /// Allocate a instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="bones">Bone transforms under the <see cref="Animator"/> component.</param>
        /// <param name="nameToHash">Method for calculate hash from name. Default is <see cref="Animator.StringToHash"/>.</param>
        /// <returns>The instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.</returns>
        public static NativeArray<BoneInfo> AllocateBoneInfos(Animator animator, Transform[] bones)
        {
            NativeArray<BoneInfo> boneInfos = new(bones.Length,
                Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < bones.Length; i++)
            {
                // Editor only assertions
                Assert.IsTrue(bones[i], $"Bone transform is null. Index = {i}.");
                Assert.IsTrue(IsInHierarchy(animator.transform, bones[i]),
                    $"Bone transform is not a child of Animator. Index = {i}.");

                if (bones[i])
                {
                    // TODO: BoneWeight is always 1
                    boneInfos[i] = new BoneInfo(animator.BindStreamTransform(bones[i]), 1,
                        Animator.StringToHash(bones[i].name),
                        FindParentIndex(bones, bones[i].parent, i));
                    // Debug.Log($"index {i} name {bones[i].name}");
                }
            }

            return boneInfos;

            static int FindParentIndex(Transform[] bones, Transform parent, int fromIndex)
            {
                if (!parent) return -1;
                for (int i = fromIndex - 1; i >= 0; i--)
                {
                    if (bones[i] == parent) return i;
                }

                return -1;
            }

            static bool IsInHierarchy(Transform root, Transform other)
            {
                while (other)
                {
                    if (other == root) return true;
                    other = other.parent;
                    if (other == root.parent) return false;
                }

                return false;
            }
        }

        /// <summary>
        /// Try to find bone node info.
        /// </summary>
        /// <param name="bones">NativeArray of BoneInfo. Will find target info from this NativeArray.</param>
        /// <param name="boneNameHash">Hash of the target bone.</param>
        /// <param name="index">The index of the target bone.</param>
        /// <returns>Whether it is successful or not to find target info.</returns>
        public static bool TryFindBone(NativeArray<BoneInfo> bones, int boneNameHash, out int index)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i].BoneNameHash == boneNameHash)
                {
                    index = i;
                    return true;
                }
            }

            index = default;
            return false;
        }
    }