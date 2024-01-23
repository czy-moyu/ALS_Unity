using System;
using System.Collections.Generic;
using Moyu.Anim;
using UnityEngine;

[Serializable]
public class AnimGraph : BaseGraph
{
    [SerializeReference]
    [ReadOnly]
    private RootPlayableNode rootNode;
    
#if UNITY_EDITOR
    [SerializeReference]
    [ReadOnly]
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
