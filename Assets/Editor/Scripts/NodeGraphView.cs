using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class NodeGraphView : GraphView
{
    private readonly AnimGraphEditor _editor;
    private readonly List<INodeView> _nodeViews = new();
    
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
            
        // 创建背景
        Insert(0, new GridBackground());
        
        CreateNodeFromAnimGraphResource();
        
        RegisterCallback<KeyDownEvent>(OnKeyDown);
    }
    
    private void CenterNodeInView(GraphView container, GraphElement node)
    {
        // Vector2 graphViewCenter = new Vector2(layout.width / 2, layout.height / 2);
        // Vector2 nodeCenter = node.GetPosition().center;
        //
        // // 计算需要移动的距离以将节点置于视图中心
        // Vector2 translation = graphViewCenter - nodeCenter;
        //
        // // 设置viewTransform属性的平移组件以移动视图
        // viewTransform.position = translation;
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        
    }

    private void CreateNodeFromAnimGraphResource()
    {
        RootPlayableNode rootPlayableNode = _editor.GetGraphAsset().GetRootNode();
        
        Dictionary<Type, Type> animNodeToEditorNode = new();

        List<Type> typesImplementingInterface = Tools.GetTypesImplementingInterface<INodeView>();
        foreach (Type editorNodeType in typesImplementingInterface)
        {
            BindAnimNode myAttribute = (BindAnimNode)Attribute
                .GetCustomAttribute(editorNodeType, typeof(BindAnimNode));
            animNodeToEditorNode.Add(myAttribute.type, editorNodeType);
        }
        
        AddNode(rootPlayableNode, animNodeToEditorNode);
    }

    private INodeView AddNode(PlayableNode node, IReadOnlyDictionary<Type, Type> animNodeToEditorNode)
    {
        if (animNodeToEditorNode.TryGetValue(node.GetType(), out Type editorNodeType))
        {
            ConstructorInfo constructorInfo = editorNodeType
                .GetConstructor(new[] { node.GetType(), typeof(int) });
            if (constructorInfo != null)
            {
                // 获取带有PlayableInputAttribute特性的字段
                List<FieldInfo> fieldsWithCustomAttribute = Tools
                    .GetFieldsWithCustomAttribute<PlayableInputAttribute>(node.GetType());
                // 创建构造函数的参数数组
                object[] constructorParameters = { node, fieldsWithCustomAttribute.Count};
                // 使用反射调用构造函数并创建MyClass的实例
                INodeView nodeView = (INodeView)constructorInfo.Invoke(constructorParameters);
                _nodeViews.Add(nodeView);
                AddElement((GraphElement)nodeView);

                if (nodeView is RootNodeView rootNodeView)
                {
                    // 将节点定位到GraphView的中心
                    CenterNodeInView(this, rootNodeView);
                }
                
                List<PlayableNode> playableInputNodes = nodeView.GetPlayableInputNodes();
                
                for (int index = 0; index < playableInputNodes.Count; index++)
                {
                    PlayableNode playableNode = playableInputNodes[index];
                    INodeView inputNodeView = AddNode(playableNode, animNodeToEditorNode);
                    if (inputNodeView != null)
                    {
                        // Debug.Log($"连接 {inputNodeView.GetType()} 和 {nodeView.GetType()} index {index}");
                        ConnectNodes(inputNodeView, nodeView, index);
                    }
                }

                return nodeView;
            }
            else
            {
                Debug.LogError("No constructor for " + editorNodeType);
                return null;
            }
        }
        else
        {
            Debug.LogError("No editor node for " + node.GetType());
            return null;
        }
    }
    
    public void ConnectNodes(INodeView inputNode, INodeView outputNode, int inputPortIndex)
    {
        // 创建一个新的边缘连接输出端口和输入端口
        var edge = new Edge
        {
            output = inputNode.GetOutputPort(),
            input = outputNode.GetInputPort(inputPortIndex)
        };

        // 将边缘添加到图形视图的边缘集合中
        AddElement(edge);
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
