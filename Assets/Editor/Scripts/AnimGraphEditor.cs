using System;
using System.Linq;
using GBG.AnimationGraph.Editor.ViewElement;
using Moyu.Anim;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class AnimGraphEditor : EditorWindow
{
    [SerializeField]
    private AnimInstance _graphAsset;
    private NodeGraphView currentGraphView;
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

    private void ConstructGraphView()
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
        _animGraphListView.AddOnIndexChangedEvent(delegate(int index)
        {
            CreateNodeGraphView(_graphAsset.GetAnimGraphs()[index].Name);
        });
    }

    private void CreateParameterView()
    {
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
        Assert.IsNotNull(_graphAsset);
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
        currentGraphView?.Update();
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
        Button saveBtn = CreateCustomToolbarButton("Save", OnSaveToolBarBtnClicked);

        // Add buttons to the toolbar
        toolbar.Add(saveBtn);

        // Add toolbar to the root visual element
        root.Add(toolbar);
    }

    private void OnSaveToolBarBtnClicked()
    {
        SaveChanges();
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

    private void CreateNodeGraphView(string graphName = AnimInstance.ROOT_GRAPH_NAME)
    {
        if (tripleSplitterRowView == null)
        {
            tripleSplitterRowView = new TripleSplitterRowView(
                new Vector2(200, 400), new Vector2(200, 400));
            rootVisualElement.Add(tripleSplitterRowView);
        }
        
        currentGraphView = new NodeGraphView(this, _graphAsset.GetAnimGraph(graphName))
        {
            style = { flexGrow = 1}
        };
        tripleSplitterRowView.MiddlePane.Clear();
        tripleSplitterRowView.MiddlePane.Add(currentGraphView);
        // add on node view selected callback
        currentGraphView.AddOnNodeViewSelected(OnNodeViewSelected);
    }

    private void OnNodeViewSelected(INodeInspector inspector)
    {
        // Debug.Log(inspector);
        tripleSplitterRowView.RightPane.Clear();
        CreateInspectorView();
        tripleSplitterRowView.RightPane.Add(inspector as VisualElement);
    }
}
