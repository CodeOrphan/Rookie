using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class XRenameAttribute : PropertyAttribute
{
    public string NewName;

    public XRenameAttribute(string name)
    {
        this.NewName = name;
    }
}


[CustomPropertyDrawer(typeof(XRenameAttribute))]
public class XRenameAttributeDrawer : PropertyDrawer
{
#if UNITY_EDITOR
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string name = (attribute as XRenameAttribute)?.NewName;
        EditorGUI.PropertyField(position, property, new GUIContent(name), true);
    }
#endif
}