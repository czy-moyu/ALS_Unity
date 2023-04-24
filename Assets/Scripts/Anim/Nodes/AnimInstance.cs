using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Moyu.Anim
{
    [CreateAssetMenu(fileName = "AnimInstance", menuName = "PlayableNode/AnimInstance", order = 0)]
    public class AnimInstance : ScriptableObject
    {
        [FormerlySerializedAs("animGraphs")]
        [SerializeReference]
        [ReadOnly]
        private List<BaseGraph> graphs = new ();
        
        [SerializeField]
        private AnimParams animParams;

        public AnimParams GetAnimParams()
        {
            return animParams;
        }

        private void OnEnable()
        {
        }
        
        public List<BaseGraph> GetGraphs()
        {
            return graphs;
        }

        public BaseGraph GetGraph(string graphName)
        {
            BaseGraph result = null;
            for (int index = 0; index < graphs.Count; index++)
            {
                BaseGraph graph = graphs[index];
                if (graph.Name == graphName)
                {
                    result = graph;
                }
            }

            if (graphName == ROOT_GRAPH_NAME && result == null)
            {
                result = new AnimGraph();
                result.Name = ROOT_GRAPH_NAME;
                graphs.Add(result);
            }
            return result;
        }
        
        public void AddAnimGraph(string graphName, AnimGraph animGraph)
        {
            animGraph.Name = graphName;
            graphs.Add(animGraph);
        }
        
        public const string ROOT_GRAPH_NAME = "RootGraph";
        public RootPlayableNode GetRootNodeOfRootGraph()
        {
            if (GetGraph(ROOT_GRAPH_NAME) is AnimGraph animGraph) 
                return animGraph.GetRootNode();
            AnimGraph graph = new();
            AddAnimGraph("RootGraph", graph);
            return graph.GetRootNode();
        }
    }
}