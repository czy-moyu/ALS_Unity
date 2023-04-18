using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "AnimClipNode", menuName = "PlayableNode/AnimClip", order = 2)]
public class AnimClipNode : PlayableNode
{
    [SerializeField]
    private AnimationClip _clip;
    
    [SerializeField]
    [Range(0, 10f)]
    private float speed = 1f;

    private AnimationClipPlayable animationClipPlayable;
    
    public override Playable GetPlayable(PlayableGraph playableGraph, 
        AnimControllerParams animControllerParams)
    {
        animationClipPlayable = AnimationClipPlayable.Create(playableGraph, _clip);
        animationClipPlayable.SetSpeed(speed);
        return animationClipPlayable;
    }

    public override void UpdatePlayable(float delta, PlayableGraph playableGraph, 
        AnimControllerParams animControllerParams)
    {
        animationClipPlayable.SetSpeed(speed);
    }
}
