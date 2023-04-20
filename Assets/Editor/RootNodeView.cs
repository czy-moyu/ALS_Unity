using UnityEditor.Experimental.GraphView;
using UnityEngine;

[BindAnimNode(typeof(RootPlayableNode))]
public class RootNodeView : NodeView<RootPlayableNode>
{
    public RootNodeView(RootPlayableNode node) : base(node)
    {
        // 节点标题
        title = "RootNode";
        
        // 创建入连接口
        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, 
            Port.Capacity.Single, typeof(Port));
        inputPort.portName = "input";
        inputContainer.Add(inputPort);
    }
}
