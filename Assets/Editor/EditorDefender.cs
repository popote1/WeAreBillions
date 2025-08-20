using script;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Defender))]
public class EditorDefender :UnityEditor.Editor
{
    private Defender m_target;
    private void OnSceneGUI() {
        m_target = (Defender) target;
        Handles.color = Color.red*new Color(1,1,1,0.2f);
        Handles.DrawSolidDisc(m_target.transform.position, Vector3.up, m_target.AttackRange);
        Handles.color = Color.green;
        Handles.DrawWireDisc(m_target.transform.position, Vector3.up, m_target.AlertCheckRadius);
    }
}