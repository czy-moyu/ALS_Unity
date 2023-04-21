using System;
using GBG.AnimationGraph.Editor.GraphEdge;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodePort : Port
{
    private NodePort(Orientation portOrientation, Direction portDirection, 
        Capacity portCapacity, Type type) 
        : base(portOrientation, portDirection, portCapacity, type)
    {
        
    }

    public static NodePort Create(Orientation portOrientation, Direction portDirection, 
        Capacity portCapacity, Type type)
    {
        EdgeConnectorListener connectorListener = new();
        NodePort port = new (portOrientation, portDirection, portCapacity, type)
        {
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
        };
        port.AddManipulator(port.m_EdgeConnector);
        return port;
    }

    public override void Connect(Edge edge)
    {
        base.Connect(edge);

        if (edge.output.node is INodeView nodeView)
        {
            nodeView.OnOutputPortConnect();
        }
        if (edge.input.node is INodeView nodeView2)
        {
            nodeView2.OnInputPortConnect(edge.input, edge);
        }
    }

    public override void Disconnect(Edge edge)
    {
        if (edge.output.node is INodeView nodeView)
        {
            nodeView.OnOutputPortDisconnect();
        }
        if (edge.input.node is INodeView nodeView2)
        {
            nodeView2.OnInputPortDisconnect(edge.input);
        }
        
        base.Disconnect(edge);
    }
}
