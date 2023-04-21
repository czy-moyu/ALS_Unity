using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    [CreateAssetMenu(fileName = "RootNode", menuName = "PlayableNode/Root", order = 1)]
    public class RootPlayableNode : PlayableNode
    {
        [SerializeField]
        [PlayableInput]
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