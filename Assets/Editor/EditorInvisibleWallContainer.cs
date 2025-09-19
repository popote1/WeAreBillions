using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InvisibleWallContaner))]
public class EditorInvisibleWallContainer :Editor
{
    private InvisibleWallContaner _target;
    public override void OnInspectorGUI()
    {
        _target = (InvisibleWallContaner)target;
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Childs Tag");
        _target._tagHandle= EditorGUILayout.TagField(_target._tagHandle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Childs Layer");
        _target._layerMask = EditorGUILayout.LayerField(_target._layerMask);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Apply Tag and Layer")) _target.ApplyTagAndLayer();
        _target._material = (Material) EditorGUILayout.ObjectField(_target._material, typeof(Material));
        if (GUILayout.Button("Show Walls"))_target.ShowAllChildrens();
        if( GUILayout.Button("Hide Walls"))_target.HideAllChildrens();
        //base.OnInspectorGUI();
    }
}
