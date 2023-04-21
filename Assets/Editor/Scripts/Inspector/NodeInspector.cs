using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class NodeInspector<T> : GraphElement, INodeInspector where T : INodeView
{
    protected T nodeView;
    protected Length FieldLabelWidth { get; set; } = Length.Percent(30);
    
    public NodeInspector(T nodeView)
    {
        this.nodeView = nodeView;
        AddToClassList("node-inspector");
        AddSeparator(5);
        AddTextField("Type", nodeView.GetType().Name, false);
        AddAllSerializedFieldsToView();
    }

    private void AddAllSerializedFieldsToView()
    {
        Type targetType = nodeView.GetPlayableNode().GetType();
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
        FieldInfo[] childFields = targetType.GetFields(bindingFlags);
        foreach (FieldInfo childField in childFields)
        {
            object[] customAttributes = childField.GetCustomAttributes(typeof(SerializeField), true);
            if (customAttributes.Length <= 0) continue;
            object value = childField.GetValue(nodeView.GetPlayableNode());
            
            Action<object> SetValue = (newValue) =>
            {
                childField.SetValue(nodeView.GetPlayableNode(), newValue);
            };
            
            switch (value)
            {
                case Object o:
                    AddObjectField(FormatLabel(childField.Name), o, SetValue);
                    break;
                case float f:
                    AddFloatField(FormatLabel(childField.Name), f, true, SetValue);
                    break;
                case bool b:
                    AddBoolField(FormatLabel(childField.Name), b, true, SetValue);
                    break;
                case string s:
                    AddTextField(FormatLabel(childField.Name), s, true, SetValue);
                    break;
            }
        }
    }

    private static string FormatLabel(string label)
    {
        var replace = label.Replace("_", "");
        string output = replace[..1].ToUpper() + replace[1..];
        return output;
    }
    
    private void AddBoolField(string label, bool value, bool enable, Action<object> SetValueFunc)
    {
        Toggle toggle = new Toggle(label);
        toggle.labelElement.style.minWidth = StyleKeyword.Auto;
        toggle.labelElement.style.maxWidth = StyleKeyword.Auto;
        toggle.labelElement.style.width = FieldLabelWidth;
        toggle.value = value;
        if (SetValueFunc != null)
            toggle.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        toggle.SetEnabled(enable);
        Add(toggle);
        AddSeparator(5);
    }
    
    private void AddFloatField(string label, float value, bool enable, Action<object> SetValueFunc)
    {
        FloatField floatField = new FloatField(label);
        floatField.labelElement.style.minWidth = StyleKeyword.Auto;
        floatField.labelElement.style.maxWidth = StyleKeyword.Auto;
        floatField.labelElement.style.width = FieldLabelWidth;
        floatField.value = value;
        if (SetValueFunc != null)
            floatField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        floatField.SetEnabled(enable);
        Add(floatField);
        AddSeparator(5);
    }

    private void AddTextField(string label, string value, bool enable, Action<object> SetValueFunc)
    {
        TextField textField = new TextField(label);
        textField.labelElement.style.minWidth = StyleKeyword.Auto;
        textField.labelElement.style.maxWidth = StyleKeyword.Auto;
        textField.labelElement.style.width = FieldLabelWidth;
        textField.value = value;
        if (SetValueFunc != null)
            textField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        textField.SetEnabled(enable);
        Add(textField);
        AddSeparator(5);
    }
    
    private void AddTextField(string label, string value, bool enable, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField(label);
        textField.labelElement.style.minWidth = StyleKeyword.Auto;
        textField.labelElement.style.maxWidth = StyleKeyword.Auto;
        textField.labelElement.style.width = FieldLabelWidth;
        textField.value = value;
        if (onValueChanged != null)
            textField.RegisterValueChangedCallback(onValueChanged);
        textField.SetEnabled(enable);
        Add(textField);
        AddSeparator(5);
    }
    
    protected void AddObjectField(string label, Object value, Action<object> SetValueFunc)
    {
        ObjectField clipField = new ObjectField(label);
        clipField.objectType = value.GetType();
        clipField.labelElement.style.minWidth = StyleKeyword.Auto;
        clipField.labelElement.style.maxWidth = StyleKeyword.Auto;
        clipField.labelElement.style.width = FieldLabelWidth;
        clipField.value = value;
        if (SetValueFunc != null)
            clipField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        Add(clipField);
        AddSeparator(5);
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