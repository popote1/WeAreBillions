using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitStartOrder))]
public class EditorUnitStartOrder : Editor {
    private UnitStartOrder m_targget;
    private void OnSceneGUI()
    {
        m_targget = (UnitStartOrder) target;

        if (m_targget.Targets.Length == 0||m_targget.Targets[0]==null) return;
        if (m_targget.IsLoop)Handles.color = Color.blue;
        else Handles.color = Color.green;
        Handles.DrawWireDisc(m_targget.Targets[0], Vector3.up, 1);
        m_targget.Targets[0]=Handles.PositionHandle(m_targget.Targets[0], Quaternion.identity);
        
        for (int i = 1; i < m_targget.Targets.Length; i++) {
            if (m_targget.Targets[i] == null) return;
            Handles.DrawDottedLine(m_targget.Targets[i-1], m_targget.Targets[i], 10);
            Handles.DrawWireDisc(m_targget.Targets[i], Vector3.up, 1);
            m_targget.Targets[i]=Handles.PositionHandle(m_targget.Targets[i], Quaternion.identity);

        }
        if( m_targget.IsLoop)
            Handles.DrawDottedLine(m_targget.Targets[m_targget.Targets.Length-1], m_targget.Targets[0], 10);
    }
}