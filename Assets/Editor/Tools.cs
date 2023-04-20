using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

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
}