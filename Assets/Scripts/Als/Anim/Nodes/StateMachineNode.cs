using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Moyu.Anim
{
    [Serializable]
    public class StateMachineNode : PlayableNode
    {
        [SerializeReference]
        private StateMachine stateMachine;
        
        public StateMachineNode(StateMachine machine)
        {
            stateMachine = machine;
        }
        
        public StateMachine GetStateMachine()
        {
            return stateMachine;
        }
        
        public override Playable GetPlayable(PlayableGraph playableGraph, AnimController animController)
        {
            return stateMachine.EntryNode.GetPlayable(playableGraph, animController);
        }
    }
}