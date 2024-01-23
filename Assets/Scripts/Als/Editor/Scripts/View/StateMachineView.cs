using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moyu.Anim;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class StateMachineView : BaseGraphView
{
    private readonly AnimGraphEditor _editor;
    private readonly StateMachine _stateMachine;
    private List<Node> _stateNodeViews = new();
    private Dictionary<string, Node> _stateNodeViewMap = new();

    public StateMachineView(AnimGraphEditor editor, StateMachine stateMachine)
    {
        _editor = editor;
        _stateMachine = stateMachine;
        
        // 设置节点拖拽
        SelectionDragger dragger = new()
        {
            // 不允许拖出边缘
            clampToParentEdges = true
        };
        
        // 其他按键触发节点拖拽
        dragger.activators.Add(new ManipulatorActivationFilter()
        {
            button = MouseButton.RightMouse,
            clickCount = 1,
            modifiers = EventModifiers.Alt
        });
        
        // 添加节点拖拽
        this.AddManipulator(dragger);

        // 设置界面缩放
        SetupZoom(ContentZoomer.DefaultMinScale, 2);
        
        // 添加界面移动
        this.AddManipulator(new ContentDragger());
        // 添加矩形选择框
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new SelectionDragger());
        
        CreateBackground();
        CreateNodes();
        CreateTransitions();
        
        RegisterCallback<KeyDownEvent>(OnKeyDown);
        graphViewChanged += OnGraphViewChanged;
    }

    public AnimGraphEditor GetEditor()
    {
        return _editor;
    }
    
    private async void CreateTransitions()
    {
        await Task.Delay(100);
        foreach (var node in _stateNodeViews)
        {
            switch (node)
            {
                case StateNodeView<EntryStateNode> entryStateNodeView:
                    entryStateNodeView.GetNode().ForEachOutputNode(outputNode =>
                    {
                        if (_stateNodeViewMap.TryGetValue(outputNode.GetGuid, out Node outputNodeView))
                        {
                            AddElement(entryStateNodeView.ConnectTo((StateNodeView<StateNode>)outputNodeView));
                        }
                    });
                    break;
                case StateNodeView<StateNode> stateNodeView:
                    stateNodeView.GetNode().ForEachOutputNode(outputNode =>
                    {
                        if (_stateNodeViewMap.TryGetValue(outputNode.GetGuid, out Node outputNodeView))
                        {
                            AddElement(stateNodeView.ConnectTo((StateNodeView<StateNode>)outputNodeView));
                        }
                    });
                    break;
            }
            // var stateNodeView = (StateNodeView<EntryStateNode>)node;
            // stateNodeView.GetNode().ForEachOutputNode(outputNode =>
            // {
            //     if (_stateNodeViewMap.TryGetValue(outputNode.GetGuid, out Node outputNodeView))
            //     {
            //         AddElement(stateNodeView.ConnectTo((StateNodeView<StateNode>)outputNodeView));
            //     }
            // });
        }
    }
    
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // Debug.Log(graphViewChange);
        var edgesToCreate = graphViewChange.edgesToCreate;

        if(edgesToCreate != null)
        {
            foreach(var edge in edgesToCreate)
            {
                CreateTransition(edge as TransitionEdge);
            }
        }
        
        return graphViewChange;
    }

    private void CreateTransition(Edge edge)
    {
        StateNodeView<StateNode> outputNode = edge.input.node as StateNodeView<StateNode>;
        // StateNodeView<StateNode> inputNode = edge.output.node as StateNodeView<StateNode>;
        switch (edge.output.node)
        {
            case StateNodeView<EntryStateNode> entryStateNodeView:
                entryStateNodeView.AddTransition(outputNode);
                break;
            case StateNodeView<StateNode> stateNodeView:
                stateNodeView.AddTransition(outputNode);
                break;
        }
        // inputNode.AddTransition(outputNode);
    }

    private void CreateNodes()
    {
        CreateNode(_stateMachine.EntryNode);
        foreach (StateNode stateNode in _stateMachine.StateNodes)
        {
            CreateNode(stateNode, null);
        }
    }
    
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        foreach(var endPort in ports)
        {
            if(endPort.direction == startPort.direction)
            {
                continue;
            }

            if (endPort.node is StateNodeView<EntryStateNode>)
            {
                continue;
            }

            // if(AreConnected(startPort, endPort))
            // {
            //     continue;
            // }

            compatiblePorts.Add(endPort);
        }

        return compatiblePorts;
    }

    private void CreateNode(StateNode node, Vector2? localMousePos)
    {
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("StateView");
        string uiPath = AssetDatabase.GetAssetPath(visualTreeAsset);
        BaseGraph graph = _editor.GetGraphAsset().GetGraphByGuid(node.GraphGuid);
        StateNodeView<StateNode> nodeView = new StateNodeView<StateNode>(node, uiPath, this);
        AddElement(nodeView);
        if (localMousePos != null)
        {
            Rect position = new Rect((Vector2)localMousePos, Vector2.zero);
            nodeView.SetPosition(position);
        }
        else
        {
            nodeView.SetPosition(nodeView.GetNode().GraphPosition);
        }
        CenterNodeInView(node);
        _stateNodeViews.Add(nodeView);
        _stateNodeViewMap.Add(node.GetGuid, nodeView);
        graph = AddAnimGraph(nodeView);
        nodeView.SetStateAnimGraph(graph as StateAnimGraph);
    }

    private void CreateNode(EntryStateNode node)
    {
        VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("StateView");
        string uiPath = AssetDatabase.GetAssetPath(visualTreeAsset);
        StateNodeView<EntryStateNode> nodeView = new(node, uiPath, this);
        AddElement(nodeView);
        nodeView.SetPosition(nodeView.GetNode().GraphPosition);
        CenterNodeInView(node);
        _stateNodeViews.Add(nodeView);
        // _stateNodeViewMap.Add(node.GetGuid, nodeView);
        // AddAnimGraph(nodeView);
    }

    private StateAnimGraph AddAnimGraph(StateNodeView<StateNode> nodeView)
    {
        // 不能是入口结点
        if (_stateMachine.EntryNode.GetGuid == nodeView.GetNode().GetGuid)
        {
            return null;
        }
        // 如果已经有了就不添加了
        if (_editor.GetGraphAsset().GetGraphByGuid(nodeView.GetNode().GraphGuid) != null)
        {
            return _editor.GetGraphAsset().GetGraphByGuid(nodeView.GetNode().GraphGuid) as StateAnimGraph;
        }
        StateAnimGraph animGraph = new(nodeView.GetNode());
        animGraph.Name = nodeView.GetNode().NodeName;
        nodeView.GetNode().GraphGuid = animGraph.GetGuid;
        _editor.GetGraphAsset().GetGraphs().Add(animGraph);
        _editor.Refresh();
        return animGraph;
    }
    
    private async void CenterNodeInView(PlayableNode node)
    {
        Vector2 graphViewCenter = new Vector2(viewport.layout.width / 2, viewport.layout.height / 2);
        while (Tools.IsVector3NaN(graphViewCenter))
        {
            await Task.Delay(100);
            graphViewCenter = new Vector2(viewport.layout.width / 2, viewport.layout.height / 2);
        }
        
        viewTransform.position = -node.GraphPosition.center + graphViewCenter;
    }
    
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.ctrlKey && evt.keyCode == KeyCode.S)
        {
            SaveChanges();
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 localMousePos = contentViewContainer.WorldToLocal(evt.mousePosition);
        evt.menu.AppendAction("Create State Node", (e) =>
        {
            var node = new StateNode("State node");
            CreateNode(node, localMousePos);
            _stateMachine.StateNodes.Add(node);
        }, DropdownMenuAction.AlwaysEnabled);
    }

    // 点击结点
    protected override void OnSelectionChange()
    {
        if (selection.Count != 1) return;
        ISelectable selectable = selection[0];
        switch (selectable)
        {
            case StateNodeView<StateNode> nodeView:
                nodeView.OnNodeSelected();
                OnNodeViewSelected?.Invoke(nodeView.GetNodeInspector());
                break;
            case StateNodeView<EntryStateNode> entryStateNodeView:
                entryStateNodeView.OnNodeSelected();
                OnNodeViewSelected?.Invoke(entryStateNodeView.GetNodeInspector());
                break;
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void SaveChanges()
    {
        foreach (var node in _stateNodeViews)
        {
            switch (node)
            {
                case StateNodeView<StateNode> stateNodeView:
                    stateNodeView.Save();
                    break;
                case StateNodeView<EntryStateNode> entryStateNodeView:
                    entryStateNodeView.Save();
                    break;
            }
        }
    }

    public override void AddOnNodeViewSelected(Action<VisualElement> action)
    {
        OnNodeViewSelected += action;
    }

    public override BaseGraph GetGraph()
    {
        return _stateMachine;
    }

    public override bool CanAcceptDrop(List<ISelectable> selection)
    {
        return true;
    }

    public override bool DragUpdated(DragUpdatedEvent evt, IEnumerable<ISelectable> selection,
        IDropTarget dropTarget, ISelection dragSource)
    {
        return true;
    }

    public override bool DragPerform(DragPerformEvent evt, IEnumerable<ISelectable> selection, 
        IDropTarget dropTarget, ISelection dragSource)
    {
        SaveChanges();
        return true;
    }

    public override bool DragEnter(DragEnterEvent evt, IEnumerable<ISelectable> selection, 
        IDropTarget enteredTarget, ISelection dragSource)
    {
        return true;
    }

    public override bool DragLeave(DragLeaveEvent evt, IEnumerable<ISelectable> selection, 
        IDropTarget leftTarget, ISelection dragSource)
    {
        return true;
    }

    public override bool DragExited()
    {
        return true;
    }
}
