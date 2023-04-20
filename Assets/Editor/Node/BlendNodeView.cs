using UnityEditor.Experimental.GraphView;


[BindAnimNode(typeof(AnimBlendNode))]
public class BlendNodeView : NodeView<AnimBlendNode>
{
    public BlendNodeView(AnimBlendNode node, int inputPortNum) : base(node, inputPortNum)
    {
        AddOutputPort();
    }
}
