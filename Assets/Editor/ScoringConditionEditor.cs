using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SoEndGameScoring.ScoringCondition))]
public class ScoringConditionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        Rect enumRect = new Rect(position.x, position.y, position.width/2, 30);
        Rect ValueRect = new Rect(position.x+position.width/2, position.y, position.width/2, 30);
        EditorGUI.PropertyField(enumRect, property.FindPropertyRelative("Operator"), GUIContent.none);
        EditorGUI.PropertyField(ValueRect, property.FindPropertyRelative("baseValue"), GUIContent.none);
        EditorGUI.EndProperty();
        //base.OnGUI(position,  property,label);
    }
}
