using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class NodeInspector<T> : GraphElement, INodeInspector where T : INodeView
{
    protected T nodeView;
    protected Length FieldLabelWidth { get; set; } = Length.Percent(25);
    
    public NodeInspector(T nodeView)
    {
        this.nodeView = nodeView;
        AddToClassList("node-inspector");

        AddSeparator(5);

        var NodeType = new TextField("Type");
        NodeType.labelElement.style.minWidth = StyleKeyword.Auto;
        NodeType.labelElement.style.maxWidth = StyleKeyword.Auto;
        NodeType.labelElement.style.width = FieldLabelWidth;
        NodeType.value = nodeView.GetPlayableNode().GetType().Name;
        NodeType.SetEnabled(false);
        Add(NodeType);
    }

    protected void AddSeparator(int height)
    {
        var separator = new HorizontalSeparatorVisualElement(height);
        Add(separator);
    }

    public virtual void UpdateAllDataInView()
    {
        
    }
}

public interface INodeInspector
{
    
}