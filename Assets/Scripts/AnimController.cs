using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class AnimController : MonoBehaviour
{
    private Animator animator;
    private PlayableGraph playableGraph;
    [SerializeField]
    private AnimControllerParams animControllerParams;
    [SerializeField]
    private RootPlayableNode rootPlayableNode;

    private void Start()
    {
        // 创建一个动画播放器
        animator = GetComponent<Animator>();
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        animControllerParams.Init();
        SetSourcePlayable(rootPlayableNode.GetPlayable(playableGraph, animControllerParams));
    }
    
    public void SetParam<T>(string paramName, T value)
    {
        animControllerParams.SetParam(paramName, value);
    }

    private void SetSourcePlayable(Playable playable)
    {
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
        // 将动画 playable 添加到 playableGraph 中
        playableOutput.SetSourcePlayable(playable);
    }
    
    private void Update()
    {
        rootPlayableNode.UpdatePlayable(playableGraph, animControllerParams);
        
        // 播放动画
        playableGraph.Evaluate(Time.deltaTime);
    }
}
