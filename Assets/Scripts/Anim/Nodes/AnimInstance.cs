using System.Collections.Generic;
using UnityEngine;

namespace Moyu.Anim
{
    [CreateAssetMenu(fileName = "AnimGraph", menuName = "PlayableNode/AnimGraph", order = 0)]
    public class AnimInstance : ScriptableObject
    {
        [SerializeField]
        private List<AnimGraph> animGraphs = new ();
        
        [SerializeField]
        private AnimParams animParams;
    
        public AnimParams GetAnimParams()
        {
            return animParams;
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
        
        public RootPlayableNode GetRootNodeOfRootGraph()
        {
            AnimGraph animGraph = GetAnimGraph("RootGraph");
            if (animGraph != null) return animGraph.GetRootNode();
            AnimGraph graph = new AnimGraph();
            AddAnimGraph("RootGraph", graph);
            animGraph = graph;
            return animGraph.GetRootNode();
        }
    }
}