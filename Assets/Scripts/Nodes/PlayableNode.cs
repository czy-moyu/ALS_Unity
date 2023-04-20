using UnityEngine;
using UnityEngine.Playables;

public abstract class PlayableNode : ScriptableObject
{
    #if UNITY_EDITOR
    [SerializeField]
    [ReadOnly]
    private Rect graphPosition;
    
    public Rect GraphPosition
    {
        get => graphPosition;
        set => graphPosition = value;
    }
    #endif
    
    public abstract Playable GetPlayable(PlayableGraph playableGraph, AnimController animController);

    public abstract void UpdatePlayable(float delta, PlayableGraph playableGraph,
        AnimController animController);
}
