

using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;
using UnityEngine.UIElements;

[BindAnimNode(typeof(AnimClipNode))]
public class ClipNodeView : NodeView<AnimClipNode>
{
    public ClipNodeView(AnimClipNode node, List<FieldInfo> nodeInputFieldInfos, NodeGraphView graphView) 
        : base(node, nodeInputFieldInfos, graphView)
    {
        AddOutputPort();
    }

    public override VisualElement GetNodeInspector()
    {
        return new AnimClipInspector(this);
    }
}
