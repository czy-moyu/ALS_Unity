using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;


[BindAnimNode(typeof(AnimBlendNode))]
public class BlendNodeView : NodeView<AnimBlendNode>
{
    public BlendNodeView(AnimBlendNode node, List<FieldInfo> nodeInputFieldInfos, NodeGraphView graphView) 
        : base(node, nodeInputFieldInfos, graphView)
    {
        AddOutputPort();
    }
}
