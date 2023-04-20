using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeGraphView : GraphView
{
    private readonly AnimGraphEditor _editor;
    private List<INodeView> _nodeViews = new();
    
    public NodeGraphView(AnimGraphEditor editor)
    {
        _editor = editor;
        
        // 设置节点拖拽
        SelectionDragger dragger = new SelectionDragger()
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

    private void AddNode(PlayableNode node, IReadOnlyDictionary<Type, Type> animNodeToEditorNode)
    {
        if (animNodeToEditorNode.TryGetValue(node.GetType(), out Type editorNodeType))
        {
            ConstructorInfo constructorInfo = editorNodeType.GetConstructor(new[] { node.GetType() });
            if (constructorInfo != null)
            {
                // 创建构造函数的参数数组
                object[] constructorParameters = { node };
                // 使用反射调用构造函数并创建MyClass的实例
                INodeView nodeView = (INodeView)constructorInfo.Invoke(constructorParameters);
                _nodeViews.Add(nodeView);
                AddElement((GraphElement)nodeView);
            }
            else
            {
                Debug.LogError("No constructor for " + editorNodeType);
            }
        }
        else
        {
            Debug.LogError("No editor node for " + node.GetType());
        }
    }
    
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.ctrlKey && evt.keyCode == KeyCode.S)
        {
            SaveChanges();
        }
    }

    private void SaveChanges()
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
