using Codice.CM.WorkspaceServer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(ObjectifAbstract))]
public class EditorObjectifPropertyDrawer : PropertyDrawer
{
    private bool _showMore;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        float ypos = 0;
        ObjectifAbstract target = property.boxedValue as ObjectifAbstract;
        
        EditorGUI.BeginProperty(position, label, property);
        Rect enumRect = new Rect(position.x, position.y+ypos, position.width /3, 20);
        EditorGUI.PropertyField(enumRect, property.FindPropertyRelative("_objectifType"), GUIContent.none);

        if (target.Type == ObjectifAbstract.ObjectifType.ZoneToReach) {
            Rect LabelTrigerRect = new Rect(position.x+position.width / 3+40, position.y+ypos, position.width / 3-80, 20);
            Rect triggerRect = new Rect(position.x+position.width * ((float)2/3)+40, position.y+ypos, position.width / 3-80, 20);
            
            GUI.Label(LabelTrigerRect, "Trigger Zone");
            EditorGUI.PropertyField(triggerRect, property.FindPropertyRelative("_triggerZone"), GUIContent.none);
        }
        else {
            Rect LabelTrigerRect = new Rect(position.x+position.width / 3+40, position.y+ypos, position.width / 3-80, 20);
            Rect triggerRect = new Rect(position.x+position.width * ((float)2/3)+40, position.y+ypos, position.width / 3-80, 20);
            GUI.Label(LabelTrigerRect, "Horse Size");
            EditorGUI.PropertyField(triggerRect, property.FindPropertyRelative("_hordeSize"), GUIContent.none);
        }
        Rect FoldoutRect = new Rect(position.width , position.y+ypos,20 , 20);
        GUIContent content = EditorGUIUtility.IconContent("_Popup");
        _showMore = EditorGUI.Foldout(FoldoutRect, _showMore, content);

        if (_showMore) {
            ypos += 30;
            Rect LabelboolRect = new Rect(position.x, position.y+ypos,40 , 20);
            Rect boolRect = new Rect(position.x+40, position.y+ypos,20 , 20);
            EditorGUI.LabelField(LabelboolRect, new GUIContent("Do win on complet"));
            EditorGUI.PropertyField(boolRect, property.FindPropertyRelative("_doWin"), GUIContent.none);
            Rect LabelIdRect = new Rect(position.width-60, position.y+ypos,20 , 20);
            Rect IdRect = new Rect(position.width-40, position.y+ypos,40 , 20);
            EditorGUI.LabelField(LabelIdRect, new GUIContent("ID"));
            EditorGUI.PropertyField(IdRect, property.FindPropertyRelative("_objectifId"), GUIContent.none);
            ypos += 20;
            Rect LabelDescriptionRect = new Rect(position.x, position.y+ypos, position.width , 20);
            Rect DescriptionRect = new Rect(position.x, position.y+ypos, position.width , 60);
            EditorGUI.LabelField(LabelDescriptionRect, new GUIContent("Desciption"));
            EditorGUI.PropertyField(DescriptionRect, property.FindPropertyRelative("_description"),GUIContent.none);
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_showMore) return 110;
        return 25;
    }
}
