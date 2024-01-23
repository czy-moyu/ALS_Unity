using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;


[BindAnimNode(typeof(RootPlayableNode))]
public class RootNodeView : NodeView<RootPlayableNode>
{
    // reflection call
    public RootNodeView(RootPlayableNode node, List<FieldInfo> nodeInputFieldInfos, NodeGraphView graphView) 
        : base(node, nodeInputFieldInfos, graphView)
    {
    }
}
