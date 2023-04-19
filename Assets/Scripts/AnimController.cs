using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Component;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Skeleton))]
[DisallowMultipleComponent]
public class AnimController : MonoBehaviour
{
    private Animator animator;
    private PlayableGraph playableGraph;
    private Skeleton _skeleton;
    [SerializeField]
    private AnimControllerParams animControllerParams;
    private Dictionary<string, float> curveParams = new();
    [SerializeField]
    private RootPlayableNode rootPlayableNode;

    private void Start()
    {
        // 创建一个动画播放器
        animator = GetComponent<Animator>();
        _skeleton = GetComponent<Skeleton>();
        _skeleton.GetOrAllocateBoneInfos();
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        animControllerParams.Init();
        SetSourcePlayable(rootPlayableNode.GetPlayable(playableGraph, this));
    }
    
    public T GetParam<T>(string paramName)
    {
        return animControllerParams.GetParam<T>(paramName);
    }
    
    public Skeleton GetSkeleton()
    {
        return _skeleton;
    }
    
    public void SetParam<T>(string paramName, T value)
    {
        animControllerParams.SetParam(paramName, value);
    }

    private void SetSourcePlayable(Playable playable)
    {
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
        // 将动画 playable 添加到 playableGraph 中
        // playableOutput.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);
        playableOutput.SetSourcePlayable(playable);
    }
    
    private void Update()
    {
        rootPlayableNode.UpdatePlayable(Time.deltaTime, playableGraph, this);
        
        // 播放动画
        playableGraph.Evaluate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        playableGraph.Destroy();
    }
}
