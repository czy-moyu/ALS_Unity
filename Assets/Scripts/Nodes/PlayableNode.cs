using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;

public abstract class PlayableNode : ScriptableObject
{
    public abstract Playable GetPlayable(PlayableGraph playableGraph, 
        AnimControllerParams animControllerParams);

    public abstract void UpdatePlayable(PlayableGraph playableGraph,
        AnimControllerParams animControllerParams);
}
