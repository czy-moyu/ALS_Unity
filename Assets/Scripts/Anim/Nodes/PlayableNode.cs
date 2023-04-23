using System;
using UnityEngine;
using UnityEngine.Playables;
using SGuid = System.Guid;

namespace Moyu.Anim
{
    [Serializable]
    public class PlayableNode
    {
        [SerializeField]
        [ReadOnly]
        private string guid = SGuid.NewGuid().ToString();
        
        public string Guid => guid;
        
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

        // public override int GetHashCode()
        // {
        //     return guid.GetHashCode();
        // }
        //
        // public override bool Equals(object obj)
        // {
        //     if (obj is PlayableNode node)
        //     {
        //         return node.guid == guid;
        //     }
        //     return false;
        // }
    }
}