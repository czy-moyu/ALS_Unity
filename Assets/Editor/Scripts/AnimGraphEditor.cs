using System;
using System.Linq;
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
    
    [OnOpenAsset]
    public static bool OpenGraphAsset(int instanceId, int line)
    {
        Object asset = EditorUtility.InstanceIDToObject(instanceId);
        if (asset is AnimGraph animGraphAsset)
        {
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
        return false;
    }

    public AnimGraph GetGraphAsset()
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
        CreateToolBar();
        CreateNodeGraphView();
    }

    private void CreateToolBar()
    {
        var root = rootVisualElement;
        
        StyleSheet styleSheet = Resources.Load<StyleSheet>("CustomEditorStyles");
        Assert.IsNotNull(styleSheet);

        // Create the toolbar
        var toolbar = new Toolbar();
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
        nodeGraphView.SaveChanges();
    }
    
    private Button CreateCustomToolbarButton(string text, Action onClick)
    {
        var button = new Button(onClick) { text = text };
        button.AddToClassList("custom-toolbar-button");
        return button;
    }

    private void CreateNodeGraphView()
    {
        nodeGraphView = new(this)
        {
            style = { flexGrow = 1}
        };
        rootVisualElement.Add(nodeGraphView);
    }
}
