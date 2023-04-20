using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class NodeView<T> : Node, INodeView where T : PlayableNode
{
    protected T _node;
    
    protected NodeView(T node)
    {
        _node = node;
        
        SetPosition(node.GraphPosition);
    }

    public void Save()
    {
        _node.GraphPosition = GetPosition();
    }
}

public interface INodeView
{
    void Save();
}