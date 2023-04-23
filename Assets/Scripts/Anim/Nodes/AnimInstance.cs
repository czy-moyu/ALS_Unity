using System.Collections.Generic;
using UnityEngine;

namespace Moyu.Anim
{
    [CreateAssetMenu(fileName = "AnimInstance", menuName = "PlayableNode/AnimInstance", order = 0)]
    public class AnimInstance : ScriptableObject
    {
        [SerializeField]
        [ReadOnly]
        private List<AnimGraph> animGraphs = new ();
        
        [SerializeField]
        private AnimParams animParams;

        public AnimParams GetAnimParams()
        {
            return animParams;
        }

        private void OnEnable()
        {
        }

        public AnimGraph GetAnimGraph(string graphName)
        {
            for (var index = 0; index < animGraphs.Count; index++)
            {
                AnimGraph animGraph = animGraphs[index];
                if (animGraph.Name == graphName)
                {
                    return animGraph;
                }
            }
            return null;
        }
        
        public void AddAnimGraph(string graphName, AnimGraph animGraph)
        {
            animGraph.Name = graphName;
            animGraphs.Add(animGraph);
        }
        
        public const string ROOT_GRAPH_NAME = "RootGraph";
        public RootPlayableNode GetRootNodeOfRootGraph()
        {
            AnimGraph animGraph = GetAnimGraph(ROOT_GRAPH_NAME);
            if (animGraph != null) return animGraph.GetRootNode();
            AnimGraph graph = new();
            AddAnimGraph("RootGraph", graph);
            animGraph = graph;
            return animGraph.GetRootNode();
        }
    }
}