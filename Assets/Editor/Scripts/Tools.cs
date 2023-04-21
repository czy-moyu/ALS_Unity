using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// show a read-only property in inspector
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }
}

/// <summary>
/// this attribute is used to bind a node to a specific type
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class BindAnimNode : Attribute
{
    public readonly Type type;

    public BindAnimNode(Type type)
    {
        this.type = type;
    }
}

public static class Tools
{
    public static List<Type> GetTypesImplementingInterface<T>()
    {
        // 获取当前运行时程序集
        Assembly assembly = Assembly.GetExecutingAssembly();

        // 从程序集中查找所有实现了指定接口的类型
        return assembly.GetTypes().Where(type =>
            type.IsClass &&
            !type.IsAbstract &&
            typeof(T).IsAssignableFrom(type)
        ).ToList();
    }
    
    public static List<FieldInfo> GetFieldsWithCustomAttribute<T>(Type objType) where T : Attribute
    {
        List<FieldInfo> fieldsWithAttribute = new();
        // Type objType = obj.GetType();
        FieldInfo[] fields = objType.GetFields(BindingFlags.Public 
                                               | BindingFlags.Instance
                                               | BindingFlags.NonPublic);

        foreach (FieldInfo field in fields)
        {
            if (field.GetCustomAttribute(typeof(T)) != null)
            {
                fieldsWithAttribute.Add(field);
            }
        }

        return fieldsWithAttribute;
    }
    
    public static Task WaitUntil(Func<bool> condition)
    {
        return Task.Run(() =>
        {
            while (!condition())
            {
            }
        });
    }
    
    public static bool IsVector3NaN(Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }
    
    public static Task Delay(float seconds)
    {
        return Task.Delay(TimeSpan.FromSeconds(seconds));
    }
}

public class HorizontalSeparatorVisualElement : VisualElement
{
    public HorizontalSeparatorVisualElement(int height)
    {
        style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0f);
        style.height = height;
        style.flexGrow = 1;
    }
}