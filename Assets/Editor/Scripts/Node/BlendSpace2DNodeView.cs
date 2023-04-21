using System.Collections;
using System.Collections.Generic;
using Moyu.Anim;
using UnityEngine;

[BindAnimNode(typeof(BlendSpace2DNode))]
public class BlendSpace2DNodeView : NodeView<BlendSpace2DNode>
{
    public BlendSpace2DNodeView(BlendSpace2DNode node, int inputPortNum, NodeGraphView graphView) 
        : base(node, inputPortNum, graphView)
    {
        AddOutputPort();
    }
}
