using System;
using script;
using UnityEngine;
using Random = UnityEngine.Random;

public static class AutoQuaterElement
{
    public enum AutoQuarterCellType {
        Free, House, Borders, Pass 
    }
    public class CellAutoQuarter {
        public Vector2Int Pos;
        public AutoQuarterCellType Type;
        public TestCell TestCell;

        public CellAutoQuarter(int x, int y) {
            Pos = new Vector2Int(x, y);
        }
    }
    public class PropertyAutoQuarter {
        public CellAutoQuarter[,] Cell;
        
        private EditorWindowQuartierAutoPlacer _autoPlacerManager;
        private CellAutoQuarter _centerCell;
        public int GetWight => Cell.GetLength(0);
        public int GetHeight => Cell.GetLength(1);
        public bool CanBeDividedOnX(int minsize) {
            if (GetWight < minsize * 2) return false;
            return true;
        }
        public bool CanBeDividedOnY(int minsize) {
            if (GetHeight < minsize * 2) return false;
            return true;
        }

        public void SetCellType(EditorWindowQuartierAutoPlacer AutoPlacer) {
            _autoPlacerManager = AutoPlacer;
            SetCenterCell();
            SetBorders();
            SetPass();
        }

        public void ColorCells() {
            foreach (var cell in Cell) {
                if( cell==null) continue;
                if (cell.TestCell == null) continue;
                switch (cell.Type) {
                    case AutoQuarterCellType.Free: cell.TestCell.Render.color = Color.green; break;
                    case AutoQuarterCellType.House: cell.TestCell.Render.color = Color.red; break;
                    case AutoQuarterCellType.Borders: cell.TestCell.Render.color = Color.blue; break;
                    case AutoQuarterCellType.Pass: cell.TestCell.Render.color = Color.yellow; break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private CellAutoQuarter GetCell(Vector2Int pos) {
            if (pos.x < 0 || pos.x >= Cell.GetLength(0) || pos.y < 0 || pos.y >= Cell.GetLength(1))
                return null;
            return Cell[pos.x, pos.y];
        }

        private void SetCenterCell() {
            Cell[GetWight / 2, GetHeight / 2].Type = AutoQuarterCellType.House;
            _centerCell = Cell[GetWight / 2, GetHeight / 2];
        }

        private void SetBorders() {
            for (int x = 0; x < GetWight; x++) {
                for (int y = 0; y < GetHeight; y++) {
                    if (x == 0 || x == GetWight - 1 || y == 0 || y == GetHeight - 1) {
                        if (Cell[x, y] == null)
                        {
                            Debug.Log("Cell don't find att"+ x+"; "+y);
                            continue;
                        }
                        Cell[x, y].Type = AutoQuarterCellType.Borders;
                    } 
                }
            }
        }

        private void SetPass() {
            Vector2Int workingZoneSize =_autoPlacerManager.WorkingZoneSize;

            Vector2Int additionalCell = Vector2Int.zero;
            Vector2Int bestVector = new Vector2Int(1,0);
            int _bestDitance = workingZoneSize.x - _centerCell.Pos.x;

            if (_bestDitance > workingZoneSize.y - _centerCell.Pos.y); {
                _bestDitance = workingZoneSize.y - _centerCell.Pos.y;
                bestVector = new Vector2Int(0, 1);
            }
            if (_bestDitance > _centerCell.Pos.y); {
                _bestDitance = _centerCell.Pos.y;
                bestVector = new Vector2Int(0, -1);
            }
            if (_bestDitance >  _centerCell.Pos.x); {
                bestVector = new Vector2Int(-1, 0);
            }

            if (bestVector.x == 0) {
                if (Random.Range(0, 2) < 1) additionalCell = new Vector2Int(1, 0);
                else additionalCell = new Vector2Int(-1, 0);
            }
            if (bestVector.y == 0) {
                if (Random.Range(0, 2) < 1) additionalCell = new Vector2Int(0,1);
                else additionalCell = new Vector2Int(0, -1);
            }

            bool isComplet = false;
            int i = 1;
            while (!isComplet) {
                
                
            }


        }
        
    }  
}