using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
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
        nodeView.GetGraphView().GetEditor().SaveChanges();
        AddAllSerializedFieldsToView();
    }

    private async void AddAllSerializedFieldsToView()
    {
        // await Task.Delay(100);
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
            
            if (childField.FieldType.IsSubclassOf(typeof(Object)))
            {
                AddObjectField(FormatLabel(childField.Name), (Object)value, childField.FieldType, SetValue);
            }
            else if (childField.FieldType == typeof(float))
            {
                AddFloatField(FormatLabel(childField.Name), (float)value, true, SetValue);
            }
            else if (childField.FieldType == typeof(bool))
            {
                AddBoolField(FormatLabel(childField.Name), (bool)value, true, SetValue);
            }
            else if (childField.FieldType == typeof(string))
            {
                AddTextField(FormatLabel(childField.Name), (string)value, true, (Action<object>)SetValue);
            }
            else if (childField.FieldType == typeof(Vector3))
            {
                AddVector3Field(FormatLabel(childField.Name), (Vector3)value, true, SetValue);
            }
            else if (childField.FieldType == typeof(Vector2))
            {
                AddVector2Field(FormatLabel(childField.Name), (Vector2)value, true, SetValue);
            }
            else if (childField.FieldType == typeof(BlendSpace))
            {
                AddEnumField(FormatLabel(childField.Name), (BlendSpace)value, true, SetValue);
            }
            else if (childField.FieldType == typeof(List<string>))
            {
                AddStringListView(FormatLabel(childField.Name), (List<string>)value);
            }
            else
            {
                Debug.Log("not support:" + childField.FieldType);
            }
        }
    }

    private static string FormatLabel(string label)
    {
        var replace = label.Replace("_", "");
        string output = replace[..1].ToUpper() + replace[1..];
        return output;
    }

    private void AddStringListView(string label, List<string> data)
    {
        var inputListViewLabel = new Label(label)
        {
            style =
            {
                height = 20,
                marginLeft = 3,
                marginRight = 3,
                unityTextAlign = TextAnchor.MiddleLeft,
            }
        };
        Add(inputListViewLabel);
        var listView = new ListView
        {
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            makeItem = () =>
            {
                var visualElement = new TextField();
                return visualElement;
            },
            bindItem = (element, i) =>
            {
                element.parent.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
                element.parent.style.alignItems = Align.Center;
                element.style.width = Length.Percent(100);
                element.style.paddingTop = 0;
                var textField = (TextField)element;
                textField.labelElement.style.minWidth = StyleKeyword.Auto;
                textField.labelElement.style.maxWidth = StyleKeyword.Auto;
                textField.labelElement.style.width = Length.Percent(30);
                textField.label = "Element " + i;
                textField.value = data[i];
                textField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
                {
                    data[i] = evt.newValue;
                });
            },
            selectionType = SelectionType.Single,
            showAddRemoveFooter = true,
            itemsSource = data,
            showBorder = true
        };
        Add(listView);
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
    
    protected void AddTextField(string label, string value, bool enable, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField(label);
        textField.labelElement.style.minWidth = StyleKeyword.Auto;
        textField.labelElement.style.maxWidth = StyleKeyword.Auto;
        textField.labelElement.style.width = FieldLabelWidth;
        textField.value = value;
        if (onValueChanged != null)
            textField.RegisterValueChangedCallback(onValueChanged);
        textField.isReadOnly = !enable;
        Add(textField);
        AddSeparator(5);
        textField.RegisterCallback<MouseDownEvent>(OnMouseDownTwice);
    }

    private void OnMouseDownTwice(MouseDownEvent evt)
    {
        // 检查点击次数是否为2，以确定是否是双击
        if (evt.clickCount != 2 || evt.button != (int)PointerEventData.InputButton.Left) return;
        var monoScript = Tools.GetMonoScriptFromType(nodeView.GetType());
        Assert.IsNotNull(monoScript);
        // 执行双击操作
        string scriptPath = AssetDatabase.GetAssetPath(monoScript);
        Debug.Log(scriptPath);
        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, 1);
    }
    
    private void AddObjectField(string label, Object value, Type type, Action<object> SetValueFunc)
    {
        ObjectField clipField = new ObjectField(label);
        clipField.objectType = type;
        clipField.labelElement.style.minWidth = StyleKeyword.Auto;
        clipField.labelElement.style.maxWidth = StyleKeyword.Auto;
        clipField.labelElement.style.width = FieldLabelWidth;
        clipField.value = value;
        if (SetValueFunc != null)
            clipField.RegisterValueChangedCallback(evt => SetValueFunc(evt.newValue));
        Add(clipField);
        AddSeparator(5);
    }

    private void AddSeparator(int height)
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