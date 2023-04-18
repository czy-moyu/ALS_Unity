using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "AnimBlendNode", menuName = "PlayableNode/AnimBlend", order = 3)]
public class AnimBlendNode : PlayableNode
{
    [SerializeField]
    private PlayableNode input1;
    
    [SerializeField]
    private PlayableNode input2;

    [SerializeField]
    private bool isBindParam;
    
    [SerializeField]
    private string paramName;
    
    [SerializeField]
    [Range(0, 1f)]
    private float alpha;

    private AnimationMixerPlayable animationMixerPlayable;
    
    public override Playable GetPlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        animationMixerPlayable = 
            AnimationMixerPlayable.Create(playableGraph, 2);
        
        Playable playable1 = input1.GetPlayable(playableGraph, animControllerParams);
        Playable playable2 = input2.GetPlayable(playableGraph, animControllerParams);
        
        playableGraph.Connect(playable1, 0, animationMixerPlayable, 0);
        playableGraph.Connect(playable2, 0, animationMixerPlayable, 1);

        UpdateWeight(animControllerParams);
        
        return animationMixerPlayable;
    }
    
    private void UpdateWeight(AnimControllerParams animControllerParams)
    {
        if (isBindParam)
        {
            float weight = animControllerParams.GetParam<float>(paramName);
            weight = Mathf.Clamp01(weight);
            animationMixerPlayable.SetInputWeight(0, 1 - weight);
            animationMixerPlayable.SetInputWeight(1, weight);
        }
        else
        {
            animationMixerPlayable.SetInputWeight(0, 1 - alpha);
            animationMixerPlayable.SetInputWeight(1, alpha);
        }
    }

    public override void UpdatePlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        input1.UpdatePlayable(playableGraph, animControllerParams);
        input2.UpdatePlayable(playableGraph, animControllerParams);
        // Debug.Log("AnimBlendNode Update " + alpha);

        UpdateWeight(animControllerParams);
    }
}
