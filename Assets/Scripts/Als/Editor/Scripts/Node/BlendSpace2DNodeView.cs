using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;
using UnityEngine;

[BindAnimNode(typeof(BlendSpace2DNode))]
public class BlendSpace2DNodeView : NodeView<BlendSpace2DNode>
{
    public BlendSpace2DNodeView(BlendSpace2DNode node, List<FieldInfo> nodeInputFieldInfos, NodeGraphView graphView) 
        : base(node, nodeInputFieldInfos, graphView)
    {
        AddOutputPort();
    }
}
