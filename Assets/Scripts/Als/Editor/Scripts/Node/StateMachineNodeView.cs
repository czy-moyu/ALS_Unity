using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;
using Plugins.Als.Editor.Scripts.Inspector;
using UnityEngine.UIElements;

[BindAnimNode(typeof(StateMachineNode))]
public class StateMachineNodeView : NodeView<StateMachineNode>
{
    public StateMachineNodeView(StateMachineNode node, List<FieldInfo> nodeInputFieldInfos, NodeGraphView graphView) 
        : base(node, nodeInputFieldInfos, graphView)
    {
        AddOutputPort();
    }
    
    public override VisualElement GetNodeInspector()
    {
        return new StateMachineNodeInspector(this);
    }
}
