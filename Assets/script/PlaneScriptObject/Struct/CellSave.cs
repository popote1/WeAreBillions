using System;
using UnityEngine;

namespace script
{
    [Serializable]
    public struct CellSave {
        public Vector2Int Pos;
        public bool IsBlock;
        public bool IsDesctrucible;

        public CellSave(Cell cell) {
            Pos = cell.Pos;
            IsBlock = cell.IsBlock;
            IsDesctrucible = cell.IsDestructible;
        }
    }
}