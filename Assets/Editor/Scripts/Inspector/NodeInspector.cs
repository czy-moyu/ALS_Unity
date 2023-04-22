using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Toggle = UnityEngine.UIElements.Toggle;

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

            void SetValue(object newValue)
            {
                childField.SetValue(nodeView.GetPlayableNode(), newValue);
            }

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
                    AddTextField(FormatLabel(childField.Name), s, true, (Action<object>)SetValue);
                    break;
                case Vector3 v3:
                    AddVector3Field(FormatLabel(childField.Name), v3, true, SetValue);
                    break;
                case Vector2 v2:
                    AddVector2Field(FormatLabel(childField.Name), v2, true, SetValue);
                    break;
                case BlendSpace blendSpace:
                    AddEnumField(FormatLabel(childField.Name), blendSpace, true, SetValue);
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

    private void AddEnumField<TEnum>(string label, TEnum value, bool enable, Action<object> SetValueFunc) where TEnum : Enum
    {
        EnumField enumField = new EnumField(label, value);
        enumField.labelElement.style.minWidth = StyleKeyword.Auto;
        enumField.labelElement.style.maxWidth = StyleKeyword.Auto;
        enumField.labelElement.style.width = FieldLabelWidth;
        if (SetValueFunc != null)
            enumField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        enumField.SetEnabled(enable);
        Add(enumField);
        AddSeparator(5);
    }
    
    private void AddVector2Field(string label, Vector2 value, bool enable, Action<object> SetValueFunc)
    {
        Vector2Field vector2Field = new Vector2Field(label);
        vector2Field.labelElement.style.minWidth = StyleKeyword.Auto;
        vector2Field.labelElement.style.maxWidth = StyleKeyword.Auto;
        vector2Field.labelElement.style.width = FieldLabelWidth;
        vector2Field.value = value;
        if (SetValueFunc != null)
            vector2Field.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        vector2Field.SetEnabled(enable);
        Add(vector2Field);
        AddSeparator(5);
    }
    
    private void AddVector3Field(string label, Vector3 value, bool enable, Action<object> SetValueFunc)
    {
        Vector3Field vector3Field = new Vector3Field(label);
        vector3Field.labelElement.style.minWidth = StyleKeyword.Auto;
        vector3Field.labelElement.style.maxWidth = StyleKeyword.Auto;
        vector3Field.labelElement.style.width = FieldLabelWidth;
        vector3Field.value = value;
        if (SetValueFunc != null)
            vector3Field.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        vector3Field.SetEnabled(enable);
        Add(vector3Field);
        AddSeparator(5);
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