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
    private AnimGraph _graphAsset;
    private NodeGraphView nodeGraphView;
    private TripleSplitterRowView tripleSplitterRowView;
    
    [OnOpenAsset]
    public static bool OpenGraphAsset(int instanceId, int line)
    {
        Object asset = EditorUtility.InstanceIDToObject(instanceId);
        if (asset is not AnimGraph animGraphAsset) return false;
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

    public AnimGraph GetGraphAsset()
    {
        return _graphAsset;
    }

    private void OnEnable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        
        // AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        // AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
    }

    private void ConstructGraphView()
    {
        CreateToolBar();
        tripleSplitterRowView = new TripleSplitterRowView(
            new Vector2(200, 400), new Vector2(200, 400));
        rootVisualElement.Add(tripleSplitterRowView);
        CreateNodeGraphView(tripleSplitterRowView.MiddlePane);
    }

    private void OnBeforeAssemblyReload()
    {
        Assert.IsNotNull(nodeGraphView);
        nodeGraphView.SaveChanges();
    }

    private void OnAfterAssemblyReload()
    {
        Assert.IsNotNull(_graphAsset);
        OpenGraphAsset(_graphAsset);
    }

    private void OpenGraphAsset(AnimGraph animGraphAsset)
    {
        _graphAsset = animGraphAsset;
        ConstructGraphView();
    }
    
    public TripleSplitterRowView GetTripleSplitterRowView()
    {
        return tripleSplitterRowView;
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
        Button AddNodeBtn = CreateCustomToolbarButton("AddSelectNode", AddSelectNode);

        // Add buttons to the toolbar
        toolbar.Add(saveBtn);
        toolbar.Add(AddNodeBtn);

        // Add toolbar to the root visual element
        root.Add(toolbar);
    }

    private void AddSelectNode()
    {
        PlayableNode node = Selection.activeObject as PlayableNode;
        if (node == null) return;
        if (nodeGraphView.IsNodeExist(node))
        {
            Debug.LogWarning("Node already exist");
            return;
        }
        nodeGraphView.AddNode(node, false, 
            nodeGraphView.GetRootNode().GetPlayableNode().GraphPosition);
    }
    
    private void OnSaveToolBarBtnClicked()
    {
        nodeGraphView.SaveChanges();
    }
    
    private Button CreateCustomToolbarButton(string text, Action onClick)
    {
        var button = new Button(onClick) { text = text };
        button.AddToClassList("custom-toolbar-button");
        return button;
    }

    private void CreateNodeGraphView(VisualElement root)
    {
        nodeGraphView = new(this)
        {
            style = { flexGrow = 1}
        };
        root.Add(nodeGraphView);
    }
}
