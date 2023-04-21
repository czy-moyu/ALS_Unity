using Moyu.Anim;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimClipInspector : NodeInspector<ClipNodeView>
{
    public AnimClipInspector(ClipNodeView nodeView) : base(nodeView)
    {
        AddSeparator(5);
        
        ObjectField clipField = new ObjectField("Clip");
        clipField.objectType = typeof(AnimationClip);
        clipField.labelElement.style.minWidth = StyleKeyword.Auto;
        clipField.labelElement.style.maxWidth = StyleKeyword.Auto;
        clipField.labelElement.style.width = FieldLabelWidth;
        clipField.value = nodeView.GetActualNode().GetClip();
        clipField.RegisterValueChangedCallback(OnValueChanged);
        Add(clipField);
    }
    
    private void OnValueChanged(ChangeEvent<Object> changeEvent)
    {
        nodeView.GetActualNode().SetClip((AnimationClip) changeEvent.newValue);
    }
}
