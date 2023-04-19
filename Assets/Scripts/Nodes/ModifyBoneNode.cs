using System;
using System.Collections;
using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "ModifyBoneNode", menuName = "PlayableNode/ModifyBone", order = 3)]
public class ModifyBoneNode : PlayableNode
{
    [SerializeField]
    [PlayableInput]
    private PlayableNode inputNode;
    
    [SerializeField]
    private string[] boneNames;
    
    [SerializeField]
    private BlendSpace blendSpace;

    [SerializeField]
    private Vector3 positionOffset;
    
    public override Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
    {
        int[] boneNameHashs = new int[boneNames.Length];
        for (int i = 0; i < boneNames.Length; ++i)
        {
            boneNameHashs[i] = Animator.StringToHash(boneNames[i]);
        }
        ModifyBoneJob animationJob = new(this, animController.GetSkeleton(), boneNameHashs);
        AnimationScriptPlayable playable = AnimationScriptPlayable.Create(playableGraph, animationJob);
        if (inputNode == null) return playable;
        // playable.SetInputCount(1);
        playable.SetProcessInputs(false);
        Playable inputPlayable = inputNode.GetPlayable(playableGraph, animController);
        playable.AddInput(inputPlayable, 0, 1f);
        // playableGraph.Connect(inputPlayable, 0, playable, 0);
        return playable;
    }
    
    public Vector3 GetPositionOffset()
    {
        return positionOffset;
    }

    public override void UpdatePlayable(float delta, PlayableGraph playableGraph, AnimController animController)
    {
        if (inputNode != null)
            inputNode.UpdatePlayable(delta, playableGraph, animController);
    }
}

public readonly struct ModifyBoneJob : IAnimationJob
{
    private readonly ModifyBoneNode node;
    private readonly Skeleton skeleton;
    private readonly int[] boneNameHashs;

    public ModifyBoneJob(ModifyBoneNode node, Skeleton skeleton, int[] boneNameHashs)
    {
        this.node = node;
        this.skeleton = skeleton;
        this.boneNameHashs = boneNameHashs;
    }
    
    public void ProcessRootMotion(AnimationStream stream)
    {
        AnimationStream inputStream = stream.GetInputStream(0);
        stream.velocity = inputStream.velocity;
        stream.angularVelocity = inputStream.angularVelocity;
    }

    public void ProcessAnimation(AnimationStream stream)
    {
        AnimationStream inputStream = stream.GetInputStream(0);
        // inputStream.AsHuman().GetMuscle()
        var handles = skeleton.GetOrAllocateBoneInfos();
        for (int i = 0; i < handles.Length; ++i)
        {
            BoneInfo handle = handles[i];
            if (Array.IndexOf(boneNameHashs, handle.BoneNameHash) > -1)
            {
                Vector3 pos = handle.BoneHandle.GetLocalPosition(inputStream);
                Vector3 positionOffset = pos + node.GetPositionOffset();
                handle.BoneHandle.SetLocalPosition(stream, positionOffset);
            }
            else
            {
                Vector3 pos = handle.BoneHandle.GetLocalPosition(inputStream);
                handle.BoneHandle.SetLocalPosition(stream, pos);
            }
            
            Quaternion localRotation = handle.BoneHandle.GetLocalRotation(inputStream);
            handle.BoneHandle.SetLocalRotation(stream, localRotation);
            
            handle.BoneHandle.SetLocalScale(stream, handle.BoneHandle.GetLocalScale(inputStream));
        }
    }
}

public enum BlendSpace
{
    LocalSpace
}