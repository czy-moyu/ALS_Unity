using System.Collections.Generic;
using GBG.AnimationGraph.Component;
using Moyu.Anim;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Skeleton))]
[DisallowMultipleComponent]
public class AnimController : MonoBehaviour
{
    private Animator animator;
    private PlayableGraph playableGraph;
    private Skeleton _skeleton;
    private Dictionary<string, float> curveParams = new();
    [SerializeField]
    private AnimInstance animGraph;
    private RootPlayableNode rootNodeOfRootGraph;

    private void Start()
    {
        // 创建一个动画播放器
        animator = GetComponent<Animator>();
        _skeleton = GetComponent<Skeleton>();
        _skeleton.GetOrAllocateBoneInfos();
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        animGraph.GetAnimParams().Init();
        rootNodeOfRootGraph = animGraph.GetRootNodeOfRootGraph();
        SetSourcePlayable(rootNodeOfRootGraph.GetPlayable(playableGraph, this));
    }
    
    public T GetAnimParam<T>(string paramName)
    {
        return animGraph.GetAnimParams().GetParam<T>(paramName);
    }
    
    public Skeleton GetSkeleton()
    {
        return _skeleton;
    }
    
    public void SetAnimParam<T>(string paramName, T value)
    {
        animGraph.GetAnimParams().SetParam(paramName, value);
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
        rootNodeOfRootGraph?.UpdatePlayable(Time.deltaTime, playableGraph, this);
        
        // 播放动画
        playableGraph.Evaluate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        playableGraph.Destroy();
    }
}
