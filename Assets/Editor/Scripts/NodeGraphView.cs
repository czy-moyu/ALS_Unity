using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moyu.Anim;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using MouseButton = UnityEngine.UIElements.MouseButton;

public class NodeGraphView : GraphView
{
    private readonly AnimGraphEditor _editor;
    private readonly List<INodeView> _nodeViews = new();
    private readonly Dictionary<Type, Type> animNodeToEditorNode = new();
    private Action<INodeInspector> OnNodeViewSelected;

    public NodeGraphView(AnimGraphEditor editor)
    {
        _editor = editor;

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

        // 设置创建节点回调
        // nodeCreationRequest += (info) =>
        // {
        //     AddElement(new RootNodeView());
        // };

        // 添加界面移动
        this.AddManipulator(new ContentDragger());
        // 添加举行选择框
        this.AddManipulator(new RectangleSelector());

        CreateBackground();

        CreateNodeFromAnimGraphResource();

        RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void AddOnNodeViewSelected(Action<INodeInspector> action)
    {
        OnNodeViewSelected += action;
    }
    
    private ISelectable prevSelection;
    public void Update()
    {
        if (selection.Count == 1 && prevSelection != selection[0])
        {
            OnSelectionChange();
        }
        if (selection.Count == 1)
            prevSelection = selection[0];
    }
    
    private void OnSelectionChange()
    {
        if (selection.Count == 1)
        {
            ISelectable selectable = selection[0];
            if (selectable is INodeView nodeView)
            {
                nodeView.OnNodeSelected();
                OnNodeViewSelected?.Invoke(nodeView.GetNodeInspector());
            }
        }
    }

    private void CreateBackground()
    {
        var gridStyleSheet = Resources.Load<StyleSheet>("GridBackground");
        styleSheets.Add(gridStyleSheet);
        // 创建背景
        GridBackground gridBackground = new GridBackground();
        Insert(0, gridBackground);
    }
    
    public void DeleteNode(INodeView nodeView)
    {
        _nodeViews.Remove(nodeView);
        RemoveElement(nodeView.GetNode());
    }

    public INodeView GetRootNode()
    {
        foreach (INodeView nodeView in _nodeViews)
        {
            if (nodeView.GetPlayableNode() is RootPlayableNode)
            {
                return nodeView;
            }
        }

        return null;
    }

    private async void CenterNodeInView(INodeView node)
    {
        Vector2 graphViewCenter = new Vector2(viewport.layout.width / 2, viewport.layout.height / 2);
        while (Tools.IsVector3NaN(graphViewCenter))
        {
            await Task.Delay(100);
            graphViewCenter = new Vector2(viewport.layout.width / 2, viewport.layout.height / 2);
        }
        
        viewTransform.position = -node.GetPlayableNode().GraphPosition.center + graphViewCenter;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(port 
            => port.direction != startPort.direction && port.node != startPort.node).ToList();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        foreach (KeyValuePair<Type, Type> pair in animNodeToEditorNode)
        {
            if (pair.Key == typeof(RootPlayableNode))
            {
                continue;
            }
            evt.menu.AppendAction("New " +  pair.Key.Name, (action) =>
            {
                ConstructorInfo constructorInfo = pair.Key
                    .GetConstructor(Type.EmptyTypes);
                Assert.IsTrue(constructorInfo != null, nameof(constructorInfo) + " != null");
                AddNode((PlayableNode)constructorInfo.Invoke(new object[] { }), false);
            }, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
        }
    }

    private void CreateNodeFromAnimGraphResource()
    {
        RootPlayableNode rootPlayableNode = _editor.GetGraphAsset().GetRootNodeOfRootGraph();

        List<Type> typesImplementingInterface = Tools.GetTypesImplementingInterface<INodeView>();
        foreach (Type editorNodeType in typesImplementingInterface)
        {
            BindAnimNode myAttribute = (BindAnimNode)Attribute
                .GetCustomAttribute(editorNodeType, typeof(BindAnimNode));
            animNodeToEditorNode.Add(myAttribute.type, editorNodeType);
        }

        AddNode(rootPlayableNode, true);
    }
    
    public bool IsNodeExist(PlayableNode node)
    {
        foreach (INodeView nodeView in _nodeViews)
        {
            if (nodeView.GetPlayableNode() == node)
            {
                return true;
            }
        }

        return false;
    }

    public INodeView AddNode(PlayableNode node, bool createInput, Rect? position = null)
    {
        if (animNodeToEditorNode.TryGetValue(node.GetType(), out Type editorNodeType))
        {
            ConstructorInfo constructorInfo = editorNodeType
                .GetConstructor(new[] { node.GetType(), typeof(int), GetType()});
            if (constructorInfo != null)
            {
                // 获取带有PlayableInputAttribute特性的字段
                List<FieldInfo> fieldsWithCustomAttribute = Tools
                    .GetFieldsWithCustomAttribute<PlayableInputAttribute>(node.GetType());
                // 创建构造函数的参数数组
                object[] constructorParameters = { node, fieldsWithCustomAttribute.Count, this };
                // 使用反射调用构造函数并创建MyClass的实例
                INodeView nodeView = (INodeView)constructorInfo.Invoke(constructorParameters);
                _nodeViews.Add(nodeView);
                AddElement((GraphElement)nodeView);

                if (position != null)
                {
                    nodeView.GetNode().SetPosition((Rect)position);
                }

                if (nodeView is RootNodeView rootNodeView)
                {
                    // 将节点定位到GraphView的中心
                    CenterNodeInView(rootNodeView);
                }

                if (!createInput) return nodeView;
                List<PlayableNode> playableInputNodes = nodeView.GetPlayableInputNodesUsingReflection();

                for (int index = 0; index < playableInputNodes.Count; index++)
                {
                    PlayableNode playableNode = playableInputNodes[index];
                    INodeView inputNodeView = AddNode(playableNode, true);
                    if (inputNodeView != null)
                    {
                        // Debug.Log($"连接 {inputNodeView.GetType()} 和 {nodeView.GetType()} index {index}");
                        ConnectNodes(inputNodeView, nodeView, index);
                    }
                }

                return nodeView;
            }

            Debug.LogError("No constructor for " + editorNodeType);
            return null;
        }

        Debug.LogError("No editor node for " + node.GetType());
        return null;
    }

    public void ConnectNodes(INodeView inputNode, INodeView outputNode, int inputPortIndex)
    {
        Assert.IsFalse(inputNode == outputNode);
        Port inputPort = outputNode.GetInputPort(inputPortIndex);
        Edge connect = inputNode.GetOutputPort().ConnectTo(inputPort);
        // 将边缘添加到图形视图的边缘集合中
        AddElement(connect);
        
        // inputNode.OnOutputPortConnect();
        // outputNode.OnInputPortConnect(inputPort, node);
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.ctrlKey && evt.keyCode == KeyCode.S)
        {
            SaveChanges();
        }
    }

    public void SaveChanges()
    {
        SaveNodes();
    }

    private void SaveNodes()
    {
        foreach (var node in _nodeViews)
        {
            node.Save();
        }
    }
}