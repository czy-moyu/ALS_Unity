using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    [Serializable]
    public class StateNode : PlayableNode
    {
        [SerializeField]
        private string guid = Guid.NewGuid().ToString();
        
        [SerializeField]
        private string nodeName;
        
        [SerializeField]
        private string graphGuid;
        
        [SerializeField]
        [ReadOnly]
        private List<StateNode> outputNodes = new List<StateNode>();
        
        public string NodeName
        {
            get => nodeName;
            set => nodeName = value;
        }
        
        public string GetGuid => guid;
        
        public string GraphGuid
        {
            get => graphGuid;
            set => graphGuid = value;
        }
        
        public StateNode(string nodeName)
        {
            this.nodeName = nodeName;
        }

        // private StateAnimGraph _graph;

        public override Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
        {
            var _graph = animController.GetStateAnimGraph(graphGuid);
            return _graph.GetRootNode().GetPlayable(playableGraph, animController);
        }

        public void AddOutputNode(StateNode node)
        {
            foreach (var stateNode in outputNodes)
            {
                if (stateNode.GetGuid == node.GetGuid)
                {
                    return;
                }
            }
            outputNodes.Add(node);
        }
        
        public void RemoveOutputNode(StateNode node)
        {
            foreach (var stateNode in new HashSet<StateNode>(outputNodes))
            {
                if (stateNode.GetGuid == node.GetGuid)
                {
                    outputNodes.Remove(stateNode);
                }
            }
        }
        
        public void ForEachOutputNode(Action<StateNode> action)
        {
            var temp = new List<StateNode>(outputNodes);
            foreach (StateNode outputNode in temp)
            {
                action(outputNode);
            }
        }

        public void Save()
        {
            
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }
    }
}