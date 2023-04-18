using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "RootNode", menuName = "PlayableNode/Root", order = 1)]
public class RootPlayableNode : PlayableNode
{
    [SerializeField]
    private PlayableNode inputNode;
    
    public override Playable GetPlayable(PlayableGraph playableGraph, AnimControllerParams animControllerParams)
    {
        return inputNode.GetPlayable(playableGraph, animControllerParams);
    }

    public override void UpdatePlayable(float delta, PlayableGraph playableGraph, 
        AnimControllerParams animControllerParams)
    {
        inputNode.UpdatePlayable(delta, playableGraph, animControllerParams);
    }
}
