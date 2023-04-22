using System;
using Moyu.Anim;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AnimGraph
{
    [SerializeField]
    [ReadOnly]
    private string name;
    
    public string Name {
        get => name;
        set => name = value;
    }

    [SerializeReference]
    private RootPlayableNode rootNode;
    
    public RootPlayableNode GetRootNode()
    {
        if (rootNode == null)
        {
            rootNode = new RootPlayableNode();
        }
        return rootNode;
    }
}
