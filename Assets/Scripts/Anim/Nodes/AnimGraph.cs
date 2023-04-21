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
        
#if UNITY_EDITOR
        [SerializeField]
        private List<PlayableNode> _nodeViewsWithoutOutputNode = new();
#endif

        public RootPlayableNode GetRootNode()
        {
            return rootNode;
        }
    
        public AnimParams GetAnimParams()
        {
            return animParams;
        }

#if UNITY_EDITOR
        public List<PlayableNode> GetNodeViewsWithoutOutputNode()
        {
            return _nodeViewsWithoutOutputNode;
        }
#endif
    }
}