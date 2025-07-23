using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AttackStruct.SpecialAttack))]
public class EditorSpecialAttackCustomPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        Rect enumRect = new Rect(position.x, position.y, position.width / 2, position.height);
        Rect ValueRect = new Rect(position.x+position.width/2, position.y, position.width/2, position.height);
        EditorGUI.PropertyField(enumRect, property.FindPropertyRelative("UniteType"), GUIContent.none);
        EditorGUI.PropertyField(ValueRect, property.FindPropertyRelative("Damage"), GUIContent.none);
        EditorGUI.EndProperty();
        //base.OnGUI(position,  property,label);
    }
}



