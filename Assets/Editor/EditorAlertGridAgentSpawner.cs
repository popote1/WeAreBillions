using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlertGridAgentSpawner))]
public class EditorAlertGridAgentSpawner : UnityEditor.Editor {
    private AlertGridAgentSpawner m_targget;
    private void OnSceneGUI() {
        m_targget = (AlertGridAgentSpawner) target;
        
        Handles.color = Color.yellow;
        Vector3[] points = new[]
        {
            m_targget.transform.position + new Vector3(m_targget.Radius, 0, m_targget.Radius),
            m_targget.transform.position + new Vector3(m_targget.Radius, 0, -m_targget.Radius),
            m_targget.transform.position + new Vector3(-m_targget.Radius, 0, -m_targget.Radius),
            m_targget.transform.position + new Vector3(-m_targget.Radius, 0, m_targget.Radius),
            m_targget.transform.position + new Vector3(m_targget.Radius, 0, m_targget.Radius)
            
        };
        Handles.DrawPolyLine(points);
        
        Handles.DrawDottedLine(m_targget.transform.position, m_targget.GetMoveTarget(), 10);
        
    }
}