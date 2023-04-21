using Moyu.Anim;


[BindAnimNode(typeof(AnimBlendNode))]
public class BlendNodeView : NodeView<AnimBlendNode>
{
    public BlendNodeView(AnimBlendNode node, int inputPortNum, NodeGraphView graphView) 
        : base(node, inputPortNum, graphView)
    {
        AddOutputPort();
    }
}
