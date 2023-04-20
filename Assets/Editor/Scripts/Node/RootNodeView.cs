using UnityEditor.Experimental.GraphView;


[BindAnimNode(typeof(RootPlayableNode))]
public class RootNodeView : NodeView<RootPlayableNode>
{
    // reflection call
    public RootNodeView(RootPlayableNode node, int inputPortNum) 
        : base(node, inputPortNum)
    {
        
    }
}
