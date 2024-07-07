using System;
using script;
//using UnityEditor;
using UnityEngine;

[Serializable]
public class TerrainLocomotionData : ScriptableObject
{
    public string MapName;
    public Vector2Int Size;
    [SerializeField]private Vector2Int _size;
    public ChunkSave[] Chunks;
    
    public CellSave[] Cells;
    
}
