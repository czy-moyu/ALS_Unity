

[BindAnimNode(typeof(AnimClipNode))]
public class ClipNodeView : NodeView<AnimClipNode>
{
    public ClipNodeView(AnimClipNode node, int inputPortNum) 
        : base(node, inputPortNum)
    {
        AddOutputPort();
    }
}
