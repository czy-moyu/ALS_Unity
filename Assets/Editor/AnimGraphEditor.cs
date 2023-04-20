using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

public class AnimGraphEditor : EditorWindow
{
    private AnimGraph _graphAsset;
    
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
    
    private void OpenGraphAsset(AnimGraph animGraphAsset)
    {
        _graphAsset = animGraphAsset;
        CreateNodeGraphView();
    }
    
    private void CreateNodeGraphView()
    {
        NodeGraphView nodeGraphView = new(this)
        {
            style = { flexGrow = 1}
        };
        rootVisualElement.Add(nodeGraphView);
    }
}
