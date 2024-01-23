using Moyu.Anim;

namespace Plugins.Als.Editor.Scripts.Inspector
{
    public class StateMachineNodeInspector : NodeInspector<StateMachineNodeView>
    {
        public StateMachineNodeInspector(StateMachineNodeView nodeView) : base(nodeView)
        {
            StateMachineNode stateMachineNode = nodeView.GetActualNode();
            AddTextField("State Machine", stateMachineNode.GetStateMachine().Name, false);
        }
    }
}