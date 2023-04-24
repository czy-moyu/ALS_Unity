using System.Collections.Generic;
using System.Reflection;
using Moyu.Anim;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public abstract class NodeView<T> : Node, INodeView where T : PlayableNode
{
    private readonly T _node;
    private Port outputPort;
    private readonly List<Port> inputPorts = new ();
    // protected readonly List<PlayableNode> inputNodes = new ();
    private readonly NodeGraphView graphView;
    
    protected NodeView(T node, int inputPortNum, NodeGraphView graphView)
    {
        this.graphView = graphView;
        _node = node;
        title = $"{_node.GetType().Name}";

        RemoveCollapseButton();
        AddSpacer();
        SetPosition(node.GraphPosition);
        AddInputPort(inputPortNum);
    }

    private void AddSpacer()
    {
        // 创建一个占位VisualElement
        VisualElement spacer = new()
        {
            style =
            {
                width = 50, // 设置占位宽度
                flexShrink = 0 // 确保宽度保持不变
            },
            name = "spacer" // 可选: 设置元素名称以便调试
        };

        // 将占位VisualElement添加到标题容器中
        titleContainer.Add(spacer);
    }
    
    public NodeGraphView GetGraphView()
    {
        return graphView;
    }
    
    private void RemoveCollapseButton()
    {
        VisualElement chevronButton = this.Q("collapse-button", (string) null);
        if (chevronButton != null)
        {
            titleButtonContainer.Remove(chevronButton);
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.StopPropagation();

        if (GetType() != typeof(RootNodeView))
        {
            evt.menu.AppendAction("Delete", (e) =>
            {
                graphView.DeleteNode(this);
                DisConnectOutputEdge();
                DisConnectAllInputEdge();
                if (graphView.GetAnimGraph().NodeWithoutOutput.Contains(_node))
                    graphView.GetAnimGraph().NodeWithoutOutput.Remove(_node);
            });
            evt.menu.AppendSeparator();
        }
        
        if (outputPort != null)
        {
            if (outputPort.connected)
            {
                evt.menu.AppendAction("Disconnect " + outputPort.portName, 
                    (e) =>
                {
                    // foreach (Edge edge in new List<Edge>(outputPort.connections))
                    // {
                    //     DisConnectEdge(edge);
                    // }
                    DisConnectOutputEdge();
                });
                evt.menu.AppendSeparator();
            }
        }


        for (int index = 0; index < inputPorts.Count; index++)
        {
            Port inputPort = inputPorts[index];
            if (!inputPort.connected) continue;
            evt.menu.AppendAction("Disconnect " + inputPort.portName, 
                (e) =>
            {
                foreach (Edge edge in new List<Edge>(inputPort.connections))
                {
                    DisConnectEdge(edge);
                }
            });
            evt.menu.AppendSeparator();
        }
    }
    
    private void DisConnectOutputEdge()
    {
        if (outputPort == null) return;
        foreach (Edge edge in new List<Edge>(outputPort.connections))
        {
            DisConnectEdge(edge);
        }
    }
    
    private void DisConnectAllInputEdge()
    {
        foreach (Port inputPort in inputPorts)
        {
            foreach (Edge edge in new List<Edge>(inputPort.connections))
            {
                DisConnectEdge(edge);
            }
        }
    }
    
    private void DisConnectEdge(Edge edge)
    {
        // if (edge.output.node is INodeView nodeView)
        // {
        //     nodeView.OnOutputPortDisconnect();
        // }
        // if (edge.input.node is INodeView nodeView2)
        // {
        //     nodeView2.OnInputPortDisconnect(edge.input);
        // }
        
        edge.input.Disconnect(edge);
        edge.output.Disconnect(edge);
        graphView.RemoveElement(edge);
    }

    protected void AddInputPort(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Port inputPort = NodePort.Create(Orientation.Horizontal, Direction.Input, 
                Port.Capacity.Single, typeof(PlayableNode));
            inputPort.portName = "Input" + i;
            inputContainer.Add(inputPort);
            inputPorts.Add(inputPort);
        }
    }
    
    protected void AddOutputPort()
    {
        outputPort = NodePort.Create(Orientation.Horizontal, Direction.Output, 
            Port.Capacity.Single, typeof(PlayableNode));

        outputPort.portName = "Output";
        outputContainer.Add(outputPort);
    }

    public virtual Port GetOutputPort()
    {
        return outputPort;
    }

    public virtual Port GetInputPort(int index)
    {
        return inputPorts[index];
    }

    public PlayableNode GetPlayableNode()
    {
        return _node;
    }

    public T GetActualNode()
    {
        return _node;
    }

    public void OnInputPortDisconnect(Port port)
    {
        // Debug.Log(GetType() + " OnInputPortDisconnect");
        // inputNodes.RemoveAt(inputPorts.IndexOf(port));
        
        // 获取带有PlayableInputAttribute特性的字段
        List<FieldInfo> fieldsWithCustomAttribute = Tools
            .GetFieldsWithCustomAttribute<PlayableInputAttribute>(_node.GetType());
        FieldInfo fieldInfo = fieldsWithCustomAttribute[inputPorts.IndexOf(port)];
        fieldInfo.SetValue(_node, null);
    }

    public void OnOutputPortDisconnect()
    {
        // Debug.Log(GetType() + " OnOutputPortDisconnect");
        if (!graphView.GetAnimGraph().NodeWithoutOutput.Contains(_node))
            graphView.GetAnimGraph().NodeWithoutOutput.Add(_node);
    }

    public void OnInputPortConnect(Port port, Edge edge)
    {
        PlayableNode playableNode = ((INodeView)edge.output.node).GetPlayableNode();

        // 获取带有PlayableInputAttribute特性的字段
        List<FieldInfo> fieldsWithCustomAttribute = Tools
            .GetFieldsWithCustomAttribute<PlayableInputAttribute>(_node.GetType());
        FieldInfo fieldInfo = fieldsWithCustomAttribute[inputPorts.IndexOf(port)];
        fieldInfo.SetValue(_node, playableNode);
        // inputNodes.Add(playableNode);
    }

    public void OnOutputPortConnect()
    {
        // Debug.Log(GetType() + " OnOutputPortConnect");
        Assert.IsNotNull(graphView);
        if (graphView.GetAnimGraph().NodeWithoutOutput.Contains(_node))
            graphView.GetAnimGraph().NodeWithoutOutput.Remove(_node);
    }

    public void OnNodeSelected()
    {
        
    }

    public virtual INodeInspector GetNodeInspector()
    {
        return new NodeInspector<INodeView>(this);
    }

    public List<PlayableNode> GetPlayableInputNodesUsingReflection()
    {
        List<PlayableNode> collectInputNodes = new();
        List<FieldInfo> fieldsWithCustomAttribute = Tools
            .GetFieldsWithCustomAttribute<PlayableInputAttribute>(_node.GetType());
        for (int index = 0; index < fieldsWithCustomAttribute.Count; index++)
        {
            FieldInfo fieldInfo = fieldsWithCustomAttribute[index];
            object value = fieldInfo.GetValue(_node);
            if (value is PlayableNode node)
            {
                collectInputNodes.Add(node);
            }
        }
        
        return collectInputNodes;
    }

    public Node GetNode()
    {
        return this;
    }

    public void Save()
    {
        _node.GraphPosition = GetPosition();
        
        // 保存更改
        // EditorUtility.SetDirty(_node);
        // AssetDatabase.SaveAssetIfDirty(_node);
    }
}

public interface INodeView
{
    void Save();

    List<PlayableNode> GetPlayableInputNodesUsingReflection();

    Node GetNode();
    
    Port GetOutputPort();
    
    Port GetInputPort(int index);
    
    PlayableNode GetPlayableNode();
    
    void OnInputPortDisconnect(Port port);

    void OnOutputPortDisconnect();
    
    void OnInputPortConnect(Port port, Edge edge);
    
    void OnOutputPortConnect();
    
    void OnNodeSelected();

    INodeInspector GetNodeInspector();

    NodeGraphView GetGraphView();
}