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
    private AnimInstance _graphAsset;
    private NodeGraphView rootGraphView;
    private TripleSplitterRowView tripleSplitterRowView;
    
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
    }

    private void ConstructGraphView()
    {
        CreateToolBar();
        tripleSplitterRowView = new TripleSplitterRowView(
            new Vector2(200, 400), new Vector2(200, 400));
        rootVisualElement.Add(tripleSplitterRowView);

        InitInspectorView();
        CreateNodeGraphView(tripleSplitterRowView.MiddlePane);
    }
    
    private void InitInspectorView()
    {
        var inspectorToggle = new CustomToolbarToggle
        {
            text = "Inspector",
            value = false,
        };
        inspectorToggle.style.borderBottomWidth = 0;
        inspectorToggle.style.borderTopWidth = 0;
        inspectorToggle.style.borderLeftWidth = 0;
        inspectorToggle.style.borderRightWidth = 0;
        tripleSplitterRowView.RightPane.Add(inspectorToggle);
        tripleSplitterRowView.RightPane.style.paddingLeft = 5;
    }

    private void OnBeforeAssemblyReload()
    {
        // Assert.IsNotNull(rootGraphView);
        // rootGraphView.SaveChanges();
        // if (EditorUtility.DisplayDialog("Warning", msg, "Save", "Discard"))
        // {
        //     
        // }
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
        rootGraphView.Update();
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
        rootGraphView.SaveChanges();
        EditorUtility.SetDirty(_graphAsset);
        AssetDatabase.SaveAssetIfDirty(_graphAsset);
    }
    
    private Button CreateCustomToolbarButton(string text, Action onClick)
    {
        var button = new Button(onClick) { text = text };
        button.AddToClassList("custom-toolbar-button");
        return button;
    }

    private void CreateNodeGraphView(VisualElement root)
    {
        rootGraphView = new NodeGraphView(this, _graphAsset.GetAnimGraph(AnimInstance.ROOT_GRAPH_NAME))
        {
            style = { flexGrow = 1}
        };
        root.Add(rootGraphView);
        rootGraphView.AddOnNodeViewSelected(OnNodeViewSelected);
    }

    private void OnNodeViewSelected(INodeInspector inspector)
    {
        // Debug.Log(inspector);
        tripleSplitterRowView.RightPane.Clear();
        InitInspectorView();
        tripleSplitterRowView.RightPane.Add(inspector as VisualElement);
    }
}
