using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    [Serializable]
    public class PlayableNode
    {
        [SerializeField]
        [ReadOnly]
        private string id = Guid.NewGuid().ToString();
        
        public string Id => id;
        
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

        public virtual Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
        {
            return default;
        }

        public virtual void UpdatePlayable(float delta, PlayableGraph playableGraph,
            AnimController animController)
        {
            
        }
    }
}