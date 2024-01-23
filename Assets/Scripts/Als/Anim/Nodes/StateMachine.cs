using System.Collections.Generic;
using UnityEngine;

namespace Moyu.Anim {
    public class StateMachine : BaseGraph
    {
        [SerializeReference]
        [ReadOnly]
        private EntryStateNode entryNode;
        
        [SerializeReference]
        [ReadOnly]
        private List<StateNode> stateNodes = new List<StateNode>();

        public EntryStateNode EntryNode => entryNode;
        
        public StateMachine()
        {
            entryNode = new EntryStateNode();
        }
        
        public List<StateNode> StateNodes => stateNodes;
        
        // public void RemoveStateNode(StateNode node)
        // {
        //     if (!stateNodes.Contains(node))
        //     {
        //         return;
        //     }
        //     stateNodes.Remove(node);
        // }
    }
}
