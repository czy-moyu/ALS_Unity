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
    
    public override Playable GetPlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        return AnimationClipPlayable.Create(playableGraph, _clip);
    }

    public override void UpdatePlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        
    }
}
