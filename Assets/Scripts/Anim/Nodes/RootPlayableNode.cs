using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    // [CreateAssetMenu(fileName = "RootNode", menuName = "PlayableNode/Root", order = 1)]
    [Serializable]
    public class RootPlayableNode : PlayableNode
    {
        [PlayableInput]
        [SerializeReference]
        [HideInInspector]
        private PlayableNode inputNode;
    
        public override Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
        {
            return inputNode.GetPlayable(playableGraph, animController);
        }

        public override void UpdatePlayable(float delta, PlayableGraph playableGraph, 
            AnimController animController)
        {
            inputNode.UpdatePlayable(delta, playableGraph, animController);
        }
    }
}