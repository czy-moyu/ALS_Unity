using System.Collections.Generic;
using UnityEngine;

namespace Moyu.Anim
{
    [CreateAssetMenu(fileName = "AnimGraph", menuName = "PlayableNode/AnimGraph", order = 0)]
    public class AnimInstance : ScriptableObject
    {
        [SerializeField]
        private List<AnimControllerParamsPair<AnimGraph>> animGraphs;
        
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
                AnimControllerParamsPair<AnimGraph> animGraph = animGraphs[index];
                if (animGraph.name == graphName)
                {
                    return animGraph.value;
                }
            }

            return null;
        }
        
        public void AddAnimGraph(string graphName, AnimGraph animGraph)
        {
            animGraphs.Add(new AnimControllerParamsPair<AnimGraph>()
            {
                name = graphName,
                value = animGraph
            });
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