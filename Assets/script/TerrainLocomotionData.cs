using System;
using script;
using UnityEditor;
using UnityEngine;

[Serializable]
public class TerrainLocomotionData : ScriptableObject
{
    public string MapName;
    public Vector2Int Size;
    [SerializeField]private Vector2Int _size;
    public ChunkSave[] Chunks;
    
    public CellSave[] Cells;

    public void Save() {
        var so = new SerializedObject(this);
        so.FindProperty("_size").vector2IntValue = Size;
        so.ApplyModifiedProperties();
        Debug.Log("Size save with =>"+Size + " to "+ _size);
    }
    
}
