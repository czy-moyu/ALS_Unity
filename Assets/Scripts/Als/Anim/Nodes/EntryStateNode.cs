using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    [Serializable]
    public class EntryStateNode : PlayableNode
    {
        [SerializeField]
        private string guid = Guid.NewGuid().ToString();
        
        [SerializeField]
        private string nodeName = "Entry";
        
        [SerializeField]
        [ReadOnly]
        private List<StateNode> outputNodes = new List<StateNode>();
        
        public string NodeName => nodeName;

        public string GetGuid => guid;
        
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
            foreach (var node in outputNodes)
            {
                action(node);
            }
        }

        public override Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
        {
            return outputNodes[0].GetPlayable(playableGraph, animController);
        }
    }
}