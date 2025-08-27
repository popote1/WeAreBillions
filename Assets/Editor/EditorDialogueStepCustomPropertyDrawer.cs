using script;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DialogueStep))]
public class EditorDialogueStepCustomPropertyDrawer : PropertyDrawer
{
    private bool _showMore;
   
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect DialogueRect = new Rect(position.x, position.y, position.width , 60);
        Rect LableCameraUsRect = new Rect(position.x, position.y+70, 120, 20);
        Rect CameraUsRect = new Rect(position.x+120, position.y+70, 20,20);
        Rect SpriteDialogue = new Rect(position.x+200, position.y+70, position.width-200,20);
        EditorGUI.PropertyField(DialogueRect, property.FindPropertyRelative("TxtDialogue"));
        EditorGUI.LabelField(LableCameraUsRect, new GUIContent("Us Camera Scroll"));
        EditorGUI.PropertyField(CameraUsRect, property.FindPropertyRelative("UsCameraScroll"), GUIContent.none);
        EditorGUI.PropertyField(SpriteDialogue, property.FindPropertyRelative("SpriteDialogue"));

        if (((DialogueStep)(property.boxedValue)).UsCameraScroll)
        {
            Rect LabelReturnToBegining = new Rect(position.x, position.y+100, 180, 20);
            Rect ReturnToBegining = new Rect(position.x+180, position.y+100, 20, 20);
            Rect LabelScroolSpeed = new Rect(position.x+200, position.y+100, 80, 20);
            Rect ScroolSpeed = new Rect(position.x+280, position.y+100, position.width-180, 20);
            Rect ScroolTraget = new Rect(position.x, position.y+130, position.width, 20);
            
            EditorGUI.LabelField(LabelReturnToBegining, new GUIContent("Return To Be Begining On complet"));
            EditorGUI.PropertyField(ReturnToBegining, property.FindPropertyRelative("ReturnToBeginingCameraPos"), GUIContent.none);
            EditorGUI.LabelField(LabelScroolSpeed, new GUIContent("Camera Scroll speed"));
            EditorGUI.PropertyField(ScroolSpeed, property.FindPropertyRelative("ScrollSpeed"), GUIContent.none);
            EditorGUI.PropertyField(ScroolTraget, property.FindPropertyRelative("EndCameraPosition"));
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (((DialogueStep)(property.boxedValue)).UsCameraScroll) return 150;
        return 100;
    }
}