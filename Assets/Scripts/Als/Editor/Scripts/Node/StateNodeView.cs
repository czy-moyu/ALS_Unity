using System;
using System.Collections.Generic;
using Moyu.Anim;
using Plugins.Als.Editor.Scripts.Inspector;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 状态机图表里的结点
/// </summary>
/// <typeparam name="T"></typeparam>
public class StateNodeView<T> : Node where T : PlayableNode
{
    private T _node;
    private Port _inputPort;
    private Port _outputPort;
    private StateAnimGraph _graphView;
    private readonly StateMachineView _stateMachineView;
    private double lastClickTime = 0;
    
    public StateNodeView(T node, string uiPath, StateMachineView stateMachineView) : base(uiPath)
    {
        _node = node;
        _stateMachineView = stateMachineView;
        switch (node)
        {
            case StateNode stateNode:
                SetName(stateNode.NodeName);
                break;
            case EntryStateNode entryStateNode:
                SetName(entryStateNode.NodeName);
                break;
        }
        CreatePorts();
        
        // 添加鼠标点击事件监听器
        RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button != (int)MouseButton.LeftMouse) return;
            double timeSinceLastClick = EditorApplication.timeSinceStartup - lastClickTime;
            if (timeSinceLastClick < 0.3) // 检查双击（例如，300毫秒内的两次点击）
            {
                OnDoubleClick();
            }
            lastClickTime = EditorApplication.timeSinceStartup;
        });
    }
    
    public void RemoveOutputTransition(StateNodeView<StateNode> outputNodeView)
    {
        switch (_node)
        {
            case StateNode stateNode:
                stateNode.RemoveOutputNode(outputNodeView.GetNode());
                break;
            case EntryStateNode entryStateNode:
                entryStateNode.RemoveOutputNode(outputNodeView.GetNode());
                break;
        }
        var editor = _stateMachineView.GetEditor();
        int index = editor.GetGraphAsset().GetGraphs().IndexOf(_stateMachineView.GetGraph());
        editor.OnIndexChangedEvent(index);
    }

    public void SetStateAnimGraph(StateAnimGraph graphView)
    {
        _graphView = graphView;
    }

    public TransitionEdge AddTransition(StateNodeView<StateNode> outputNodeView)
    {
        switch (_node)
        {
            case StateNode stateNode:
                stateNode.AddOutputNode(outputNodeView.GetNode());
                break;
            case EntryStateNode entryStateNode:
                entryStateNode.AddOutputNode(outputNodeView.GetNode());
                break;
        }
        // (_stateMachineView.GetGraph() as StateMachine).RemoveStateNode(outputNodeView._node);
        return _outputPort.ConnectTo<TransitionEdge>(outputNodeView._inputPort);
    }
    
    public TransitionEdge ConnectTo(StateNodeView<StateNode> stateView)
    {
        return _outputPort.ConnectTo<TransitionEdge>(stateView._inputPort);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if(!Application.isPlaying)
        {
            evt.menu.AppendAction("Add Transition", a => DragTransitionEdge());
            if (_node is StateNode stateNode)
            {
                evt.menu.AppendAction("Delete Node", a => Delete());
            }
        }
    }
    
    // private void DeleteOutputTransition()
    // {
    //     switch (_node)
    //     {
    //         case StateNode stateNode:
    //             stateNode.ForEachOutputNode(outputStateNode =>
    //             {
    //                 stateNode.RemoveOutputNode(outputStateNode);
    //             });
    //             break;
    //     }
    // }

    private void Delete()
    {
        var stateMachine = (StateMachine)_stateMachineView.GetGraph();
        if (_node is not StateNode stateNode) return;
        stateMachine.StateNodes.Remove(stateNode);
        foreach (StateNode _stateNode in stateMachine.StateNodes)
        {
            _stateNode.ForEachOutputNode(outputStateNode =>
            {
                if (outputStateNode.GetGuid == stateNode.GetGuid)
                {
                    _stateNode.RemoveOutputNode(outputStateNode);
                }
            });
        }
        stateMachine.EntryNode.RemoveOutputNode(stateNode);
        var editor = _stateMachineView.GetEditor();
        editor.GetGraphAsset().GetGraphs().Remove(_graphView);
        int index = editor.GetGraphAsset().GetGraphs().IndexOf(_stateMachineView.GetGraph());
        editor.OnIndexChangedEvent(index);
    }
    
    private void OnDoubleClick()
    {
        // 双击事件的处理逻辑
        var editor = _stateMachineView.GetEditor();
        int index = editor.GetGraphAsset().GetGraphs().IndexOf(_graphView);
        if (index > -1)
        {
            editor.OnIndexChangedEvent(index);
            editor.RefreshAnimGraphListView(index);
        }
    }
    
    private void DragTransitionEdge()
    {
        _outputPort.SendEvent(new DragEvent(_outputPort.GetGlobalCenter(), _outputPort));
    }
    
    public override void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
    {
        base.CollectElements(collectedElementSet, conditionFunc);

        if(_inputPort != null)
        {
            foreach(var connection in _inputPort.connections)
            {
                collectedElementSet.Add(connection);
            }
        }

        if(_outputPort != null)
        {
            foreach(var connection in _outputPort.connections)
            {
                collectedElementSet.Add(connection);
            }
        }
    }

    private void CreatePorts()
    {
        switch (_node)
        {
            case StateNode stateNode:
                _inputPort = CreatePort(Direction.Input, Port.Capacity.Multi);
                _outputPort = CreatePort(Direction.Output, Port.Capacity.Multi);
                break;
            case EntryStateNode entryStateNode:
                _outputPort = CreatePort(Direction.Output, Port.Capacity.Multi);
                break;
        }
    }
    
    public void SetName(string name)
    {
        switch (_node)
        {
            case StateNode stateNode:
                stateNode.NodeName = name;
                break;
        }
        if (_graphView != null)
        {
            _graphView.Name = name;
        }
        Label titleLabel = this.Q<VisualElement>("node-border").Q<VisualElement>("title").Q<Label>("title-label");
        titleLabel.text = name;
        _stateMachineView.GetEditor().Refresh();
    }
    
    private Port CreatePort(Direction direction, Port.Capacity capacity)
    {
        Port port = Port.Create<TransitionEdge>(Orientation.Vertical, direction, capacity, typeof(bool));
        Insert(0, port);
        return port;
    }

    public T GetNode()
    {
        return _node;
    }
    
    public void Save()
    {
        _node.GraphPosition = GetPosition();
    }
    
    public void OnNodeSelected()
    {
        // VisualElement nodeBorder = this.Q<VisualElement>("node-border");
        // nodeBorder.style.borderBottomColor = new StyleColor(Color.cyan);
        // nodeBorder.style.borderLeftColor = new StyleColor(Color.cyan);
        // nodeBorder.style.borderRightColor = new StyleColor(Color.cyan);
        // nodeBorder.style.borderTopColor = new StyleColor(Color.cyan);
    }

    public StateNodeInspector<T> GetNodeInspector()
    {
        return new StateNodeInspector<T>(this);
    }
    
    public class DragEvent : MouseDownEvent
    {
        public DragEvent(Vector2 mousePosition, VisualElement target)
        {
            this.mousePosition = mousePosition;
            this.target = target;
        }
    }
}
