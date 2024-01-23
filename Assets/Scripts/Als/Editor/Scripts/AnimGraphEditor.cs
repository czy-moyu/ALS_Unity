using System;
using System.Linq;
using GBG.AnimationGraph.Editor.ViewElement;
using Moyu.Anim;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class AnimGraphEditor : EditorWindow
{
    [SerializeField]
    private AnimInstance _graphAsset;
    private BaseGraphView currentGraphView;
    private TripleSplitterRowView tripleSplitterRowView;
    private AnimGraphListView _animGraphListView;
    
    [OnOpenAsset]
    public static bool OpenGraphAsset(int instanceId, int line)
    {
        Object asset = EditorUtility.InstanceIDToObject(instanceId);
        if (asset is not AnimInstance animGraphAsset) return false;
        bool success = true;
        AnimGraphEditor editor = Resources.FindObjectsOfTypeAll<AnimGraphEditor>()
            .FirstOrDefault(window => window._graphAsset == animGraphAsset);
        if (!editor)
        {
            editor = CreateInstance<AnimGraphEditor>();
            try
            {
                editor.OpenGraphAsset(animGraphAsset);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                success = false;
            }
        }
            
        if (success)
        {
            editor.Show();
            editor.Focus();
        }
        else
        {
            editor.Close();
        }

        return true;
    }

    public AnimInstance GetGraphAsset()
    {
        return _graphAsset;
    }

    private void OnEnable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        
        AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

        if (_graphAsset != null)
        {
            ConstructGraphView();
        }
    }

    public void ConstructGraphView()
    {
        currentGraphView = null;
        tripleSplitterRowView = null;
        _animGraphListView = null;
        rootVisualElement.Clear();
        CreateToolBar();
        CreateNodeGraphView();
        CreateInspectorView();
        CreateParameterView();
        CreateGraphListView();
    }

    private void CreateGraphListView()
    {
        var inspectorToggle = new CustomToolbarToggle
        {
            text = "AnimGraphs",
            value = false,
            style =
            {
                borderBottomWidth = 0,
                borderTopWidth = 0,
                borderLeftWidth = 0,
                borderRightWidth = 0
            }
        };
        tripleSplitterRowView.LeftBottomPane.Add(inspectorToggle);
        _animGraphListView = new AnimGraphListView(this);
        tripleSplitterRowView.LeftBottomPane.Add(_animGraphListView);
        _animGraphListView.RemoveOnIndexChangedEvent(OnIndexChangedEvent);
        _animGraphListView.AddOnIndexChangedEvent(OnIndexChangedEvent);
    }

    public void OnIndexChangedEvent(int index)
    {
        var graph = _graphAsset.GetGraphs()[index];
        if (graph is AnimGraph animGraph)
        {
            CreateNodeGraphView(animGraph.Name);
        }

        if (graph is StateMachine stateMachine)
        {
            CreateStateMachineView(stateMachine.Name);
        }
    }

    private void CreateParameterView()
    {
        tripleSplitterRowView.LeftTopPane.Clear();
        var inspectorToggle = new CustomToolbarToggle
        {
            text = "Parameters",
            value = false,
            style =
            {
                borderBottomWidth = 0,
                borderTopWidth = 0,
                borderLeftWidth = 0,
                borderRightWidth = 0
            }
        };
        tripleSplitterRowView.LeftTopPane.Add(inspectorToggle);
        tripleSplitterRowView.LeftTopPane.Add(new AnimParamsView(_graphAsset));
    }
    
    private void CreateInspectorView()
    {
        var inspectorToggle = new CustomToolbarToggle
        {
            text = "Inspector",
            value = false,
            style =
            {
                borderBottomWidth = 0,
                borderTopWidth = 0,
                borderLeftWidth = 0,
                borderRightWidth = 0
            }
        };
        tripleSplitterRowView.RightPane.Add(inspectorToggle);
        tripleSplitterRowView.RightPane.style.paddingLeft = 5;
    }

    private static void OnBeforeAssemblyReload()
    {
        
    }

    private void OnAfterAssemblyReload()
    {
        Assert.IsNotNull(_graphAsset, "Graph asset is null");
        OpenGraphAsset(_graphAsset);
    }

    private void OpenGraphAsset(AnimInstance animGraphAsset)
    {
        _graphAsset = animGraphAsset;
        ConstructGraphView();
    }
    
    public TripleSplitterRowView GetTripleSplitterRowView()
    {
        return tripleSplitterRowView;
    }

    private void Update()
    {
        currentGraphView.Update();
    }

    private void CreateToolBar()
    {
        VisualElement root = rootVisualElement;
        
        StyleSheet styleSheet = Resources.Load<StyleSheet>("CustomEditorStyles");
        Assert.IsNotNull(styleSheet);

        // Create the toolbar
        Toolbar toolbar = new();
        toolbar.styleSheets.Add(styleSheet);

        // Create the buttons for the toolbar
        Button saveBtn = CreateCustomToolbarButton("Save", SaveChanges);
        
        Button deleteGraphBtn = CreateCustomToolbarButton("DeleteGraph", DeleteGraph);

        // Add buttons to the toolbar
        toolbar.Add(saveBtn);
        toolbar.Add(deleteGraphBtn);

        // Add toolbar to the root visual element
        root.Add(toolbar);
    }

    private void DeleteGraph()
    {
        if (currentGraphView.GetGraph() is StateAnimGraph)
        {
            EditorUtility.DisplayDialog("提示", "只能通过删除对应的状态机结点才能把这个AnimGraph删除。", "确认");
            return;
        }
        _graphAsset.GetGraphs().Remove(currentGraphView.GetGraph());
        OnIndexChangedEvent(0);
        _animGraphListView.Refresh();
    }
    
    public void Refresh()
    {
        _animGraphListView.Refresh();
    }

    public void RefreshAnimGraphListView(int index)
    {
        _animGraphListView.Refresh(index);
    }

    public override void SaveChanges()
    {
        base.SaveChanges();
        currentGraphView.SaveChanges();
        EditorUtility.SetDirty(_graphAsset);
        AssetDatabase.SaveAssetIfDirty(_graphAsset);
    }
    
    private static Button CreateCustomToolbarButton(string text, Action onClick)
    {
        var button = new Button(onClick) { text = text };
        button.AddToClassList("custom-toolbar-button");
        return button;
    }
    
    private void CreateStateMachineView(string graphName)
    {
        if (tripleSplitterRowView == null)
        {
            tripleSplitterRowView = new TripleSplitterRowView(
                new Vector2(200, 400), new Vector2(200, 400));
            rootVisualElement.Add(tripleSplitterRowView);
        }
        
        BaseGraph graph = _graphAsset.GetGraph(graphName);
        if (graph is not StateMachine stateMachine)
            throw new Exception("Graph is not a StateMachine");
        currentGraphView = new StateMachineView(this, stateMachine)
        {
            style = { flexGrow = 1}
        };
        
        tripleSplitterRowView.MiddlePane.Clear();
        tripleSplitterRowView.MiddlePane.Add(currentGraphView);
        
        currentGraphView.AddOnNodeViewSelected(OnNodeViewSelected);
    }

    private void CreateNodeGraphView(string graphName = AnimInstance.ROOT_GRAPH_NAME)
    {
        if (tripleSplitterRowView == null)
        {
            tripleSplitterRowView = new TripleSplitterRowView(
                new Vector2(200, 400), new Vector2(200, 400));
            rootVisualElement.Add(tripleSplitterRowView);
        }

        BaseGraph graph = _graphAsset.GetGraph(graphName);
        if (graph is not AnimGraph animGraph)
            throw new Exception("Graph is not an AnimGraph");
        currentGraphView = new NodeGraphView(this, animGraph)
        {
            style = { flexGrow = 1}
        };
        tripleSplitterRowView.MiddlePane.Clear();
        tripleSplitterRowView.MiddlePane.Add(currentGraphView);
        // add on node view selected callback
        currentGraphView.AddOnNodeViewSelected(OnNodeViewSelected);
    }

    // 结点单击时调用
    private void OnNodeViewSelected(VisualElement inspector)
    {
        // Debug.Log(inspector);
        tripleSplitterRowView.RightPane.Clear();
        CreateInspectorView();
        if (inspector != null)
        {
            tripleSplitterRowView.RightPane.Add(inspector);
        }
    }
}
