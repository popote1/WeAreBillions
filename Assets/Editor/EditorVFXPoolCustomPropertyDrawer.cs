using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(VFXPoolManager.VFXPoolInitializeData))]
    public class EditorVFXPoolCustomPropertyDrawer :PropertyDrawer
    {
        private bool _showMore;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            EditorGUI.BeginProperty(position, label, property);
            Rect enumRect = new Rect(position.x, position.y, position.width / 2, 20);
            Rect ValueRect = new Rect(position.x+position.width/2, position.y, position.width/2-20, 20);
            Rect showMore = new Rect(position.width, position.y, 20, 20);
            EditorGUI.PropertyField(enumRect, property.FindPropertyRelative("Type"), GUIContent.none);
            EditorGUI.PropertyField(ValueRect, property.FindPropertyRelative("PrfVfx"), GUIContent.none);
            _showMore =EditorGUI.Toggle(showMore, _showMore);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_showMore) return 60;
            return 20;
        }
    }