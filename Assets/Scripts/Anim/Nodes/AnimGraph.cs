using System;
using System.Collections.Generic;
using Moyu.Anim;
using UnityEditor;
using UnityEngine;
using SGuid = System.Guid;

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
    
    [SerializeField]
    [ReadOnly]
    private string guid = SGuid.NewGuid().ToString();
        
    public string Guid => guid;

    [SerializeReference]
    [ReadOnly]
    private RootPlayableNode rootNode;
    
#if UNITY_EDITOR
    [SerializeReference]
    [ReadOnly]
    // [CustomListDrawer]
    private List<PlayableNode> nodeWithoutOutput = new ();
    
    public List<PlayableNode> NodeWithoutOutput
    {
        get { return nodeWithoutOutput ??= new List<PlayableNode>(); }
    }
#endif

    public RootPlayableNode GetRootNode()
    {
        if (rootNode == null)
        {
            rootNode = new RootPlayableNode();
        }
        return rootNode;
    }
}
