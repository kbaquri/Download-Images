using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class DrawButtonAttribute: PropertyAttribute
{
    public string buttonName;
    public bool playModeOnly;
    public bool editorModeOnly;
   public DrawButtonAttribute(string buttonName = null){
       this.buttonName = buttonName;
   }
}
#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( DrawButtonAttribute ) )]
public class DrawButtonDrawer : PropertyDrawer
{
    DrawButtonAttribute buttonAttribClass { get {return (DrawButtonAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width -= 18;
        label = EditorGUI.BeginProperty( position, label, property );
        if(GUILayout.Button(buttonAttribClass.buttonName))
        {
        }
        EditorGUI . EndProperty ();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // if(condition)
        //     return base.GetPropertyHeight(property,label);
        // else 
            return 0;
    }
}
#endif
#if UNITY_EDITOR
[CanEditMultipleObjects] 
[CustomEditor(typeof(MonoBehaviour), true)] 
public class MonoBehaviourCustomEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        // if (!Application.isPlaying)
        // {
            var type = target.GetType();
            foreach (var method in type.GetMethods(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance))
            {
                if (method.GetParameters().Length == 0)
                {
                    if(method.GetCustomAttributes(typeof(DrawButtonAttribute), true).Length > 0)
                    {
                        DrawButtonAttribute buttonAttribute = (DrawButtonAttribute)method.GetCustomAttributes(typeof(DrawButtonAttribute), true)[0];
                        string buttonText = string.IsNullOrEmpty(buttonAttribute.buttonName) ? method.Name : buttonAttribute.buttonName;
                        using (new EditorGUI.DisabledScope( (buttonAttribute.playModeOnly) ? !UnityEditor.EditorApplication.isPlaying : (buttonAttribute.editorModeOnly) ?  UnityEditor.EditorApplication.isPlaying : false))
                        {
                            if (GUILayout.Button(buttonText))
                            {
                                method.Invoke(target, null);
                            }
                        }
                    }
                }
                else
                {
                    if(method.GetCustomAttributes(typeof(DrawButtonAttribute), true).Length > 0)
                    {
                        string warning = typeof(DrawButtonAttribute).Name + " works only on methods with no parameters";
                        EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    }
                }
            }
        // }
        DrawDefaultInspector();
    }
}
#endif