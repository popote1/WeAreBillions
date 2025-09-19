using System;
using System.Collections.Generic;
using System.Diagnostics;
using script;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public static class AutoQuaterElement
{
    public enum AutoQuarterCellType {
        Free, House, Borders, Pass 
    }
    public class CellAutoQuarter {
        public Vector2Int Pos;
        public Vector2Int PropertyPlace;
        public AutoQuarterCellType Type;
        public TestCell TestCell;

        public CellAutoQuarter(int x, int y) {
            Pos = new Vector2Int(x, y);
        }
    }
    public class PropertyAutoQuarter {
        public CellAutoQuarter[,] Cell;
        
        private EditorWindowQuartierAutoPlacer _autoPlacerManager;
        public CellAutoQuarter CenterCell;
        public Vector2Int DirectionToRoad;
        public List<CellAutoQuarter> BorderCells= new List<CellAutoQuarter>();
        public List<CellAutoQuarter> PassCells = new List<CellAutoQuarter>();
        
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

        public CellAutoQuarter[] Get8Neighbours(CellAutoQuarter cell) {
            List<CellAutoQuarter> neighbours = new List<CellAutoQuarter>();
            if( GetCell(cell.PropertyPlace+new Vector2Int(1,0))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(1,0)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(1,1))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(1,1)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(0,1))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(0,1)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(-1,1))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(-1,1)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(-1,0))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(-1,0)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(-1,-1))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(-1,-1)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(0,-1))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(0,-1)));
            if( GetCell(cell.PropertyPlace+new Vector2Int(1,-1))!=null) neighbours.Add(GetCell(cell.PropertyPlace+new Vector2Int(1,-1)));
            return neighbours.ToArray();
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
            CenterCell = Cell[GetWight / 2, GetHeight / 2];
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
                        BorderCells.Add(Cell[x,y]);
                    } 
                }
            }
        }

        private void SetPass() {
            Vector2Int workingZoneSize =_autoPlacerManager.WorkingZoneSize;

            Vector2Int additionalCell = Vector2Int.zero;
            Vector2Int bestVector = new Vector2Int(1,0);
            int _bestDitance = workingZoneSize.x - CenterCell.Pos.x;

            if (_bestDitance > workingZoneSize.y - CenterCell.Pos.y) {
                _bestDitance = workingZoneSize.y - CenterCell.Pos.y;
                bestVector = new Vector2Int(0, 1);
            }
            if (_bestDitance > CenterCell.Pos.y) {
                _bestDitance = CenterCell.Pos.y;
                bestVector = new Vector2Int(0, -1);
            }
            if (_bestDitance >  CenterCell.Pos.x) {
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

            DirectionToRoad = bestVector;
            bool isComplet = false;
            int i = 0;
            while (!isComplet) {
                i++;
                Vector2Int newPos = CenterCell.PropertyPlace + (bestVector * i);
                if (GetCell(newPos) != null) {
                    if (GetCell(newPos).Type == AutoQuarterCellType.Borders) BorderCells.Remove(GetCell(newPos));
                    GetCell(newPos).Type = AutoQuarterCellType.Pass;
                    PassCells.Add(GetCell(newPos));
                    
                }
                else {
                    isComplet = true;
                }
                if (GetCell(newPos+additionalCell) != null) {
                    if (GetCell(newPos).Type == AutoQuarterCellType.Borders) BorderCells.Remove(GetCell(newPos));
                    GetCell(newPos+additionalCell).Type = AutoQuarterCellType.Pass;
                    PassCells.Add(GetCell(newPos));
                }

                if (i > 1000) {
                    Debug.LogWarning("AutoQuater Property Pass calculation does over 1000 pass and had been stop" );
                    isComplet = true;
                }
            }
        }

        private List<CellAutoQuarter> GetNeighboursCellOfType(CellAutoQuarter cell, AutoQuarterCellType type) {
            List<CellAutoQuarter> neighbours = new List<CellAutoQuarter>();
            if (GetCell(cell.PropertyPlace + new Vector2Int(1, 0)) != null &&
                GetCell(cell.PropertyPlace + new Vector2Int(1, 0)).Type == type) {
                neighbours.Add(GetCell(cell.PropertyPlace + new Vector2Int(1, 0)));
            } 
            if (GetCell(cell.PropertyPlace + new Vector2Int(-1, 0)) != null &&
                GetCell(cell.PropertyPlace + new Vector2Int(-1, 0)).Type == type) {
                neighbours.Add(GetCell(cell.PropertyPlace + new Vector2Int(-1, 0)));
            } 
            if (GetCell(cell.PropertyPlace + new Vector2Int( 0,1)) != null &&
                GetCell(cell.PropertyPlace + new Vector2Int( 0,1)).Type == type) {
                neighbours.Add(GetCell(cell.PropertyPlace + new Vector2Int(0,1)));
            } 
            if (GetCell(cell.PropertyPlace + new Vector2Int(0,-1)) != null &&
                GetCell(cell.PropertyPlace + new Vector2Int(0,-1)).Type == type) {
                neighbours.Add(GetCell(cell.PropertyPlace + new Vector2Int(0,-1)));
            }

            return neighbours;
        }

        public CellAutoQuarter[] GetPalissadeKeyCells() { 
            CellAutoQuarter currentCell = GetFirstCellBorder();
            if (currentCell == null) return null;
            List<CellAutoQuarter> keycells = new List<CellAutoQuarter>();
            keycells.Add(currentCell);
            int i = 0;
            List<CellAutoQuarter> neighbours;
            CellAutoQuarter previewsCell=null;
            Vector2Int currentDir =
                GetNeighboursCellOfType(currentCell, AutoQuarterCellType.Borders)[0].PropertyPlace - currentCell.PropertyPlace;
            while (i<100) {
                i++;
                if (GetCell(currentCell.PropertyPlace + currentDir)!=null&&GetCell(currentCell.PropertyPlace + currentDir).Type == AutoQuarterCellType.Borders) {
                    previewsCell = currentCell;
                    currentCell = GetCell(currentCell.PropertyPlace + currentDir);
                    continue;
                }
                neighbours=GetNeighboursCellOfType(currentCell, AutoQuarterCellType.Borders);
                if (neighbours.Count < 2) {
                    keycells.Add(currentCell);
                    return keycells.ToArray();
                }
                foreach (var neighbour in neighbours) {
                    if (neighbour == previewsCell) continue;
                    keycells.Add(currentCell);
                    currentDir = neighbour.PropertyPlace - currentCell.PropertyPlace;
                    previewsCell = currentCell;
                    currentCell = neighbour;
                    break;
                }
            }
            Debug.LogWarning("Get Palissade Key Cell Got Stop After over 1000loops");
            return keycells.ToArray();
        }

        private CellAutoQuarter GetFirstCellBorder() {
            foreach (var cell in BorderCells) {
                if (GetNeighboursCellOfType(cell,AutoQuarterCellType.Borders).Count==1) return cell;
            }
            return null;
        }

        public CellAutoQuarter GetRandomPassCell() {
            return PassCells[Random.Range(0, PassCells.Count)];
        }

        public List<CellAutoQuarter> GetFreeCells() {
            List<CellAutoQuarter> freelist = new List<CellAutoQuarter>();
            foreach (var cell in Cell) {
                if (cell.Type  ==AutoQuarterCellType.Free) freelist.Add(cell);
            }
            return freelist;
        }

        public void SetHouseCells() {
            foreach (var cell in Get8Neighbours(CenterCell)) {
                if (cell.Type == AutoQuarterCellType.Borders) BorderCells.Remove(cell);
                if (cell.Type == AutoQuarterCellType.Pass) PassCells.Remove(cell);
                cell.Type = AutoQuarterCellType.House;
            }
        }

        public void SetBorderCellsToFree() {
            foreach (var cell in BorderCells) {
                cell.Type = AutoQuarterCellType.Free;
            }
            BorderCells.Clear();
        }
    }  
}