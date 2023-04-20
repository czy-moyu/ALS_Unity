using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class NodeView<T> : Node, INodeView where T : PlayableNode
{
    protected readonly T _node;
    protected Port outputPort;
    protected readonly List<Port> inputPorts = new ();
    protected NodeGraphView _graphView;
    
    protected NodeView(T node, int inputPortNum, NodeGraphView graphView)
    {
        _graphView = graphView;
        _node = node;
        title = $"{_node.name}({_node.GetType()})";

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
        if (outputPort != null)
        {
            if (outputPort.connected)
            {
                evt.menu.AppendAction("Disconnect " + outputPort.portName, (e) =>
                {
                    foreach (Edge edge in new List<Edge>(outputPort.connections))
                    {
                        outputPort.Disconnect(edge);
                        edge.input.Disconnect(edge);
                        _graphView.RemoveElement(edge);
                        
                        if (edge.input.node is INodeView nodeView)
                        {
                            nodeView.OnInputPortDisconnect(edge.input);
                        }
                        OnOutputPortDisconnect();
                    }
                });
                evt.menu.AppendSeparator();
            }
        }


        for (var index = 0; index < inputPorts.Count; index++)
        {
            Port inputPort = inputPorts[index];
            if (!inputPort.connected) continue;
            evt.menu.AppendAction("Disconnect " + inputPort.portName, (e) =>
            {
                foreach (Edge edge in new List<Edge>(inputPort.connections))
                {
                    inputPort.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    _graphView.RemoveElement(edge);

                    if (edge.output.node is INodeView nodeView)
                    {
                        nodeView.OnOutputPortDisconnect();
                    }

                    OnInputPortDisconnect(inputPort);
                }
            });
            evt.menu.AppendSeparator();
        }
    }

    protected void AddInputPort(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Port inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, 
                Port.Capacity.Single, typeof(PlayableNode));
            inputPort.portName = "Input" + i;
            inputContainer.Add(inputPort);
            inputPorts.Add(inputPort);
        }
    }
    
    protected void AddOutputPort()
    {
        outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, 
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

    public void OnInputPortDisconnect(Port port)
    {
        Debug.Log(GetType() + " OnInputPortDisconnect");
    }

    public void OnOutputPortDisconnect()
    {
        Debug.Log(GetType() + " OnOutputPortDisconnect");
    }

    public List<PlayableNode> GetPlayableInputNodes()
    {
        List<PlayableNode> inputNodes = new();
        
        // foreach (var inputPort in inputContainer.Children())
        // {
        //     if (inputPort is Port port)
        //     {
        //         foreach (var edge in port.connections)
        //         {
        //             if (edge.input.node is NodeView<PlayableNode> nodeView)
        //             {
        //                 inputNodes.Add(nodeView._node);
        //             }
        //         }
        //     }
        // }
        List<FieldInfo> fieldsWithCustomAttribute = Tools
            .GetFieldsWithCustomAttribute<PlayableInputAttribute>(_node.GetType());
        for (int index = 0; index < fieldsWithCustomAttribute.Count; index++)
        {
            FieldInfo fieldInfo = fieldsWithCustomAttribute[index];
            object value = fieldInfo.GetValue(_node);
            if (value is PlayableNode node)
            {
                inputNodes.Add(node);
            }
        }

        return inputNodes;
    }

    public Node GetNode()
    {
        return this;
    }

    public void Save()
    {
        _node.GraphPosition = GetPosition();
        
        // 保存更改
        EditorUtility.SetDirty(_node);
        AssetDatabase.SaveAssetIfDirty(_node);
    }
}

public interface INodeView
{
    void Save();

    List<PlayableNode> GetPlayableInputNodes();

    Node GetNode();
    
    Port GetOutputPort();
    
    Port GetInputPort(int index);
    
    PlayableNode GetPlayableNode();
    
    void OnInputPortDisconnect(Port port);

    void OnOutputPortDisconnect();
}