

using Moyu.Anim;

[BindAnimNode(typeof(AnimClipNode))]
public class ClipNodeView : NodeView<AnimClipNode>
{
    public ClipNodeView(AnimClipNode node, int inputPortNum, NodeGraphView graphView) 
        : base(node, inputPortNum, graphView)
    {
        AddOutputPort();
    }
}
