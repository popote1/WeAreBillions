using System;
using script;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(House))]
public class EditorToolHouse : Editor
{
    private House _house;

    private Vector3 _handlePos1 = Vector3.zero;
    private Vector3 _handlePos2 = Vector3.zero;
    private SerializedProperty _boundProperty;
    private SerializedObject _houseObject;
    private void OnEnable() {
        _house = target.GetComponent<House>();
        SetUpHandles();
        _houseObject = new UnityEditor.SerializedObject(_house);
        _boundProperty= _houseObject.FindProperty("_spawningBounds");
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.blue;
        
        //Vector3 handlePosition = _house.gameObject.transform.position+_lastPos;
        
        Vector3 pos1 = Handles.FreeMoveHandle(RotateVector3OnY(_handlePos1, _house.transform.eulerAngles.y)+_house.transform.position, 0.1f, Vector3.one, Handles.RectangleHandleCap);
        Vector3 pos2 = Handles.FreeMoveHandle(RotateVector3OnY(_handlePos2, _house.transform.eulerAngles.y)+_house.transform.position, 0.1f, Vector3.one, Handles.RectangleHandleCap);
        pos1 = RotateVector3OnY(pos1 - _house.transform.position, -_house.transform.eulerAngles.y)+_house.transform.position;
        pos2 = RotateVector3OnY(pos2 - _house.transform.position, -_house.transform.eulerAngles.y)+_house.transform.position;
        CalculateBoundries(pos1, pos2);
        _handlePos1 = pos1-_house.transform.position;
        _handlePos2 = pos2-_house.transform.position;
        
        DrawBound();
    }

    private void SetUpHandles()
    {
        _handlePos1 = _house.SpawningBound.min;
        _handlePos2 = _house.SpawningBound.max;
    }

    private void CalculateBoundries(Vector3 pos1, Vector3 pos2)
    {
        float xMin;
        float xMax;
        float zMin;
        float zMax;

        if (pos1.x > pos2.x) {
            xMin = pos2.x-_house.transform.position.x;
            xMax = pos1.x-_house.transform.position.x;
        }
        else {
            xMin = pos1.x-_house.transform.position.x;
            xMax = pos2.x-_house.transform.position.x;
        }
        if (pos1.z > pos2.z) {
            zMin = pos2.z-_house.transform.position.z;
            zMax = pos1.z-_house.transform.position.z;
        }
        else {
            zMin = pos1.z-_house.transform.position.z;
            zMax = pos2.z-_house.transform.position.z;
        }

        Bounds bound =  _house.SpawningBound;
        bound.center = Vector3.Lerp(pos1, pos2, 0.5f);
        bound.SetMinMax(new Vector3(xMin, 0, zMin), new Vector3(xMax, 0, zMax));
        //_boundProperty.boundsValue = bound;
        //_house.SpawningBound=bound ;
        _boundProperty.boundsValue = bound;

        _houseObject.ApplyModifiedProperties();
        PrefabUtility.RecordPrefabInstancePropertyModifications(_house);
        
    }

    private Vector3 CenterBoundZone() {
        return _house.transform.position + _house.SpawningBound.center;
    }

    private void DrawBound() {
        Vector3 pos1 = RotateVector3OnY(_house.SpawningBound.min, _house.transform.eulerAngles.y);
        Vector3 pos2 = RotateVector3OnY(new Vector3(_house.SpawningBound.min.x, 0,_house.SpawningBound.max.z), _house.transform.eulerAngles.y);
        Vector3 pos3 = RotateVector3OnY(_house.SpawningBound.max, _house.transform.eulerAngles.y);
        Vector3 pos4 = RotateVector3OnY(new Vector3(_house.SpawningBound.max.x, 0,_house.SpawningBound.min.z), _house.transform.eulerAngles.y);
        Handles.DrawLine(pos1+_house.transform.position, pos2+_house.transform.position);
        Handles.DrawLine(pos3+_house.transform.position, pos2+_house.transform.position);
        Handles.DrawLine(pos3+_house.transform.position, pos4+_house.transform.position);
        Handles.DrawLine(pos1+_house.transform.position, pos4+_house.transform.position);
    }
    
    public Vector2 RotateVector2(Vector2 v, float delta) {
        delta = Mathf.Deg2Rad * -delta;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public Vector3 RotateVector3OnY(Vector3 v, float delta) {
        delta = Mathf.Deg2Rad * -delta;
        return new Vector3(
            v.x * Mathf.Cos(delta) - v.z * Mathf.Sin(delta),0,
            v.x * Mathf.Sin(delta) + v.z * Mathf.Cos(delta)
        );
    }
    
}
