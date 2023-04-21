using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moyu.Anim
{
    [CreateAssetMenu(fileName = "AnimGraph", menuName = "PlayableNode/AnimGraph", order = 0)]
    public class AnimGraph : ScriptableObject
    {
        [SerializeField]
        private AnimParams animParams;
    
        [SerializeField]
        private RootPlayableNode rootNode;

        public RootPlayableNode GetRootNode()
        {
            return rootNode;
        }
    
        public AnimParams GetAnimParams()
        {
            return animParams;
        }
    }
}