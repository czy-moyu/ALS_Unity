using UnityEngine;
using UnityEngine.Playables;

public abstract class PlayableNode : ScriptableObject
{
    public abstract Playable GetPlayable(PlayableGraph playableGraph, AnimController animController);

    public abstract void UpdatePlayable(float delta, PlayableGraph playableGraph,
        AnimController animController);
}
