using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AmbianceSourceTracker))]
public class AmbianceSourceTrackerManager : Editor
{
    private AmbianceSourceTracker m_targget;

    private void OnSceneGUI()
    {
        m_targget = (AmbianceSourceTracker) target;

        if (m_targget.Area == null) return;
        float y = m_targget.transform.position.y;
        Vector3 min =m_targget.Area.bounds.min;
        Vector3 max =m_targget.Area.bounds.max;

        Vector3[] point = new Vector3[4];
        Vector3[] pointOuter = new Vector3[4];
        Handles.color = m_targget.DebugColor;

        Vector3 p1 = min;
        p1.y = y;
        Vector3 p2 = new Vector3(min.x, y, max.z);
        p2.y = y;
        point[0] = p2;

        Vector3 p11 = p1 + new Vector3(-m_targget.BlendeDistance, 0, -m_targget.BlendeDistance);
        Vector3 p12 = p2 + new Vector3(-m_targget.BlendeDistance, 0, m_targget.BlendeDistance);
        pointOuter[0] = p12;
        Handles.DrawDottedLine(p1, p2, 1);
        Handles.DrawDottedLine(p11, p12, 1);
        //======
        p1 = p2;
        p2 = max;
        p2.y = y;
        point[1] = p2;

        p11 = p12;
        p12 = p2 + new Vector3(m_targget.BlendeDistance, 0, m_targget.BlendeDistance);
        pointOuter[1] = p12;
        Handles.DrawDottedLine(p1, p2, 2);
        Handles.DrawDottedLine(p11, p12, 1);
        //======
        p1 = p2;
        p2 = new Vector3(max.x, y, min.z);
        p2.y = y;
        point[2] = p2;
        Handles.DrawDottedLine(p1, p2, 2);
        Handles.DrawDottedLine(p11, p12, 1);
      
        p11 = p12;
        p12 = p2 + new Vector3(m_targget.BlendeDistance, 0, -m_targget.BlendeDistance);
        pointOuter[2] = p12;
        Handles.DrawDottedLine(p1, p2, 2);
        Handles.DrawDottedLine(p11, p12, 1);
        //===== 
        p1 = p2;
        p2 = min;
        p2.y = y;
        point[3] = p2;
        Handles.DrawDottedLine(p1, p2, 2);
        Handles.DrawDottedLine(p11, p12, 1);
      
        p11 = p12;
        p12 = p2 + new Vector3(-m_targget.BlendeDistance, 0, -m_targget.BlendeDistance);
        pointOuter[3] = p12;
        Handles.DrawDottedLine(p1, p2, 2);
        Handles.DrawDottedLine(p11, p12, 1);
    
      
        Handles.color = m_targget.DebugColor * new Color(1, 1, 1, 0.2f);
        Handles.DrawAAConvexPolygon(point);
        Handles.DrawAAConvexPolygon(pointOuter);
    }
}