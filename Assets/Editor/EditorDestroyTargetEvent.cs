using System;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(DestroyTargetEvent))]
public class EditorDestroyTargetEvent : Editor
{
    private DestroyTargetEvent _target;
    private void OnSceneGUI()
    {
        
        _target = (DestroyTargetEvent)target;
        if (_target.TargetToDestroy != null) {
            foreach (var agent in _target.TargetToDestroy)
            {
                if (agent == null) continue;
                Handles.color = Color.red;
                Handles.DrawLine(_target.transform.position, agent.transform.position);
                Handles.DrawWireCube(agent.transform.position, Vector3.one);
                
            }
        }

        if (_target.DestructiblesToDestroy != null)
        {
            foreach (var destructible in _target.DestructiblesToDestroy)
            {
                if (destructible == null) continue;
                Handles.color = Color.red;
                Handles.DrawLine(_target.transform.position, destructible.GetPosition());
                Handles.DrawWireCube(destructible.GetPosition(), Vector3.one*2.5f);
            }
        }
    }
}
