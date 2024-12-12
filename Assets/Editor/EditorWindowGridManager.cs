using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using script;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EditorWindowGridManager : EditorWindow
{

    private GridManager _target;
    private bool _showDebugOptions;
    private bool _showChunkLinks;
    private TestCell _prefabsTestCell;
    private int _debugCellTestZoneSize = 20;
    private Vector2 _debugCellTestZonePos;
    private LayerMask _layerMaskGround = 10;
    private Color _colorFree = new Color(0,1,0,0.5f);
    private Color _colorBlock = new Color(1,0,0,0.5f);
    private Color _colorDesctuctible = new Color(0,0,1,0.5f);

    private List<Cell> _currentTestCells=new List<Cell>();
    
    [MenuItem("PopoteTools/WindowGridManager")]
    public static void ShowWindow() {
        EditorWindowGridManager window = GetWindow<EditorWindowGridManager>();
        window.titleContent = new GUIContent(" WindowGridManager");
        window.GetDebugCell();
    }


    private void OnGUI() {
        if( GUILayout.Button("TestButton"))Debug.Log("Test Button Pressed");
        if( GUILayout.Button("Search GridManger"))GetGridManager();
        GUILayout.Label("Selected Grid Manager");
        _target = (GridManager)EditorGUILayout.ObjectField( _target ,typeof(GridManager), true);

        if (_target == null) return;
        _showDebugOptions =EditorGUILayout.Foldout(_showDebugOptions, "Show Debug Options");
        if (_showDebugOptions) {
            if( GUILayout.Button("CalculateTheGrid"))GenerateCells();
            if (GUILayout.Button("DisplayCells")) GenerateDebugCells2();
            if (GUILayout.Button("CalculateColliders")) CheckColliders();
            if (GUILayout.Button("RecalculateChunks")) RecalculateChunks();
            if (GUILayout.Button("ColorChunks")) ColorChunks();
            if (GUILayout.Button("ClearDebugsCells")) ClearDebugCells2();
            
            GUILayout.Space(10);
            if (GUILayout.Button("GetDebugCell")) GetDebugCell();
            _debugCellTestZonePos = EditorGUILayout.Vector2Field("DebugCellTestZone", _debugCellTestZonePos);
            _debugCellTestZoneSize = EditorGUILayout.IntField("DebugCellTestZoneSize", _debugCellTestZoneSize); 
            _prefabsTestCell = (TestCell) EditorGUILayout.ObjectField(_prefabsTestCell, typeof(TestCell));
            _layerMaskGround =(EditorGUILayout.MaskField(_layerMaskGround,InternalEditorUtility.layers));
            //_showChunkLinks = GUILayout.Toggle(_showChunkLinks, "Show Chunk links");
            Metrics.UsDebugCellGroundOffsetting = GUILayout.Toggle(Metrics.UsDebugCellGroundOffsetting, "UsDebugCellGroundOffsetting");
            _colorBlock = EditorGUILayout.ColorField("Color Blocked",_colorBlock);
            _colorFree = EditorGUILayout.ColorField("Color Free",_colorFree);
            _colorDesctuctible = EditorGUILayout.ColorField("Color Destructible",_colorDesctuctible);
            GUILayout.Space(20);
        }
        if( GUILayout.Button("Bake Terrain Locomotion Data"))BakeTerrainLocomotionData();
        if( GUILayout.Button("Load Locomotion Data"))_target.LoadLocomotionData();
        if( GUILayout.Button("Save Locomotion Data"))SaveLocomotionData();
        
    }

    [ExecuteInEditMode]
    private void Update() {
        if( _target!=null&&_showChunkLinks)DisplayDebugChunkLinks();
    }

    private void GetGridManager() {
        GridManager[] grids =SceneAsset.FindObjectsByType<GridManager>(FindObjectsSortMode.None);
        
        if( grids.Length ==0)Debug.Log("NoGridManagerFound");
        if (grids.Length == 1) {
            _target = grids[0];
            Debug.Log("Grid Manager Found");
        }
        if( grids.Length >1)Debug.Log("Multyple Grid Manager found");
        
    }
    public void GetDebugCell()=> _prefabsTestCell =AssetDatabase.LoadAssetAtPath<TestCell>("Assets/Prefab/DebugCell.prefab");
    public void GenerateCells() {
        _target.ClearGrid();

        List<Chunk> chunks = new List<Chunk>();
        Cell[,] cells;
        chunks = new List<Chunk>();
        for (int x = 0; x < _target.Size.x; x++)
            
        {
            for (int y = 0; y < _target.Size.y; y++)
            {
                Chunk chunk = new Chunk(x, y);
                chunks.Add(chunk);
                if (x > 0)
                {
                    Chunk neighbor = GetChunk(x - 1, y, chunks);
                    neighbor.neighbors.Add(chunk);
                    chunk.neighbors.Add(neighbor);
                }

                if (y > 0)
                {
                    Chunk neighbor = GetChunk(x, y - 1, chunks);
                    neighbor.neighbors.Add(chunk);
                    chunk.neighbors.Add(neighbor);
                }
            }
        }

        cells = new Cell[_target.Size.x * Metrics.chunkSize, _target.Size.y * Metrics.chunkSize];
        for (int x = 0; x < _target.Size.x * Metrics.chunkSize; x++)
        {
            for (int y = 0; y < _target.Size.y * Metrics.chunkSize; y++)
            {
                //Debug.Log(x+";"+ y);
                cells[x, y] = new Cell(x, y, _target.Offset);
                cells[x, y].Chunk = GetChunk(x / Metrics.chunkSize, y / Metrics.chunkSize, chunks);
                GetChunk(x / Metrics.chunkSize, y / Metrics.chunkSize, chunks).cells.Add(cells[x, y]);
            }
        }

        foreach (var chunk in chunks)
        {
            chunk.CalculatCenter();
        }
        
        _target.SetGridData(chunks, cells);
    }
    private Chunk GetChunk(int x, int y, List<Chunk>chunks)
    {
        if (x < 0 || x >= _target.Size.x || y < 0 || y >= _target.Size.y) return null;
        int i = x * _target.Size.y + y;
        return chunks[i];
    }
    private void GenerateDebugCells() {
        ClearDebugCells();
        
        Cell[,] cells = _target.GetAllCells();
        foreach (var cell in cells) {
            cell.DebugCell = Instantiate(_prefabsTestCell, cell.WordPos, Quaternion.identity);
            cell.DebugCell.transform.SetParent(_target.transform);
            
            if( cell.IsBlock) cell.ColorDebugCell(_colorBlock);
            else if (cell.MoveCost == Metrics.DestructibleMoveCost)cell.ColorDebugCell(_colorDesctuctible);
            else cell.ColorDebugCell(_colorFree);

            if (Metrics.UsDebugCellGroundOffsetting) {
                RaycastHit hit;
                if (Physics.Raycast(cell.DebugCell.transform.position + new Vector3(0, 50, 0), Vector3.down,
                    out hit, _layerMaskGround))
                {
                    cell.DebugCell.transform.position = hit.point+new Vector3(0,0.5f,0);
                }
            }

        }
    }

    private void GenerateDebugCells2() {
        ClearDebugCells2();
        Vector2Int center = 
            new Vector2Int(
                Mathf.RoundToInt((_target.Size.x*Metrics.chunkSize-_debugCellTestZoneSize) * _debugCellTestZonePos.x)+_debugCellTestZoneSize / 2
                , Mathf.RoundToInt((_target.Size.y*Metrics.chunkSize-_debugCellTestZoneSize) * _debugCellTestZonePos.y)+_debugCellTestZoneSize / 2) ;
        
        List<Cell> cells = _target.GetCellNeighborRange(center, _debugCellTestZoneSize/2);

        for (int i =_currentTestCells.Count-1; i >=0; i--){
            if (cells.Contains(_currentTestCells[i])) return; 
            Destroy(_currentTestCells[i].DebugCell.gameObject);
            _currentTestCells.RemoveAt(i);
        }

        _currentTestCells = cells;
        foreach (var cell in _currentTestCells) {
            cell.DebugCell = Instantiate(_prefabsTestCell, cell.WordPos, Quaternion.identity);
            cell.DebugCell.transform.SetParent(_target.transform);
            
            if( cell.IsBlock) cell.ColorDebugCell(_colorBlock);
            else if (cell.MoveCost == Metrics.DestructibleMoveCost)cell.ColorDebugCell(_colorDesctuctible);
            else cell.ColorDebugCell(_colorFree);

            if (Metrics.UsDebugCellGroundOffsetting) {
                RaycastHit hit;
                if (Physics.Raycast(cell.DebugCell.transform.position + new Vector3(0, 50, 0), Vector3.down,
                    out hit, _layerMaskGround)) {
                    cell.DebugCell.transform.position = hit.point+new Vector3(0,0.5f,0);
                }
            }
        }
        Debug.Log(" Current cells get ="+ _currentTestCells.Count);

    }
    private void CheckColliders() {
        
        GridManager.OnClearPathFindingData?.Invoke();
        Cell[,] cells = _target.GetAllCells();
        ColorAllDebugGridToColor(Color.white, cells);
        foreach (var cell in cells) {
            cell.CheckCellColliders();
        }
    }
    private void ColorAllDebugGridToColor(Color color, Cell[,] cells) {
        foreach (var cell in cells)
        {
            if (cell != null) cell.ColorDebugCell(color);
        }
    }
    private void DisplayDebugChunkLinks() {
        foreach (var chunk in _target.GetAllChunks()) {
            if (chunk == null)
            {
                Debug.Log("Chunk is null");
                continue;
            }
            if (chunk.neighbors == null) continue;

            foreach (var neighbor in chunk.neighbors) {
                if (neighbor == null)
                {
                    Debug.DrawLine(chunk.Center, chunk.Center + new Vector3(3f, 3f), Color.red);
                    continue;
                }

                Debug.DrawLine(chunk.Center, Vector3.Lerp(chunk.Center, neighbor.Center, 0.4f), Color.blue);
            }
        }
    }
    private void ColorChunks() {
        foreach (var chunk in _target.GetAllChunks())
        {
            Color col = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
            foreach (var cell in chunk.cells)
            {
                cell.ColorDebugCell(col);
            }
        }
    }
    private void RecalculateChunks() {
        Cell[,] cells = _target.GetAllCells(); 
        List<Chunk> chunks = _target.GetAllChunks().ToList();
            List<Cell> CloseList = new List<Cell>();
            List<List<Cell>> Rooms = new List<List<Cell>>();

            foreach (var chunk in chunks.ToList())
            {
                Rooms.Clear();
                CloseList.Clear();
                foreach (var cell in chunk.cells)
                {
                    if (cell == null) continue;
                    if (cell.IsBlock) continue;
                    if (CloseList.Contains(cell)) continue;
                    List<Cell> room = GetCellRoom(cell, chunk, cells);
                    CloseList.AddRange(room);
                    Rooms.Add(room);
                }

                //Debug.Log("Rooms founds = " + Rooms.Count);
                foreach (var neighbor in chunk.neighbors)
                {
                    neighbor.neighbors.Remove(chunk);
                }

                if (Rooms.Count > 1)
                {
                    Rooms = GetBlockedCells(chunk, Rooms);
                    Vector2Int originalPos = chunk.Coordonate;
                    foreach (var cell in chunk.cells)
                    {
                        cell.Chunk = null;
                    }

                    chunks.Remove(chunk);

                    foreach (var room in Rooms)
                    {
                        Chunk newChunk = new Chunk(chunk.Coordonate);
                        newChunk.cells = room;
                        newChunk.CalculatCenter();
                        newChunk.neighbors = GetChunksNeighbors(newChunk,cells);
                        newChunk.Coordonate = originalPos;
                        chunks.Add(newChunk);

                        foreach (var cell in newChunk.cells)
                        {
                            cell.Chunk = newChunk;
                        }

                        foreach (var neighbor in newChunk.neighbors)
                        {
                            if (neighbor != null) neighbor.neighbors.Add(newChunk);
                        }
                    }
                }
                else if (Rooms.Count > 0)
                {
                    //chunk.cells.Clear();
                    //chunk.cells = Rooms[0];
                    chunk.CalculatCenter();
                    chunk.neighbors = GetChunksNeighbors(chunk,cells);
                    //chunk.Coordonate = chunk.Coordonate;
                    foreach (var neighbor in chunk.neighbors)
                    {
                        if (neighbor != null) neighbor.neighbors.Add(chunk);
                    }
                }
            }

        _target.SetGridData(chunks.ToList(), cells);
    }
    private List<Cell> GetCellRoom(Cell originCell, Chunk chunk, Cell[,]cells) {
        List<Cell> OpenList = new List<Cell>();
        List<Cell> CLoseList = new List<Cell>();
        OpenList.Add(originCell);
        while (OpenList.Count > 0)
        {
            Cell current = OpenList[0];
            OpenList.Remove(current);
            CLoseList.Add(current);
            foreach (var neighbor in GetNeighbors(current, cells))
            {
                if (!chunk.cells.Contains(neighbor)) continue;
                if (neighbor.IsBlock) continue;
                if (CLoseList.Contains(neighbor)) continue;
                if (OpenList.Contains(neighbor)) continue;
                OpenList.Add(neighbor);
            }
        }

        return CLoseList;
    }
    private Cell[] GetNeighbors(Cell cell, Cell[,]cells) {
        Cell[] neighbors = new Cell[8];
        neighbors[0] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y + 1,cells);
        neighbors[1] = GetCellFromPos(cell.Pos.x, cell.Pos.y + 1,cells);
        neighbors[2] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y + 1,cells);
        neighbors[3] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y,cells);
        neighbors[4] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y - 1,cells);
        neighbors[5] = GetCellFromPos(cell.Pos.x, cell.Pos.y - 1,cells);
        neighbors[6] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y - 1,cells);
        neighbors[7] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y,cells);
        return neighbors;
    }
    private Cell GetCellFromPos(int x, int y, Cell[,]cells) {
        if (x < 0 || x >= _target.Size.x * Metrics.chunkSize || y < 0 || y >= _target.Size.y * Metrics.chunkSize) return null;
        return cells[x, y];
    }
    private List<List<Cell>> GetBlockedCells(Chunk chunk, List<List<Cell>> rooms)
    {
        Dictionary<Cell, int> classedCell = new Dictionary<Cell, int>();
        List<List<Cell>> newClassed = new List<List<Cell>>();
        for (int i = 0; i < rooms.Count; i++)
        {
            newClassed.Add(new List<Cell>());
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            foreach (Cell cell in rooms[i])
            {
                classedCell.Add(cell, i);
            }
        }

        foreach (Cell cell in chunk.cells)
        {
            if (classedCell.Keys.Contains(cell)) continue;
            newClassed[GetClosestRoom(cell, classedCell)].Add(cell);
        }

        for (int i = 0; i < newClassed.Count; i++)
        {
            if (newClassed.Count <= 0) continue;
            rooms[i].AddRange(newClassed[i]);
        }

        return rooms.ToList();
    }
    private int GetClosestRoom(Cell cell, Dictionary<Cell, int> classify)
    {
        float bestDistance = float.PositiveInfinity;
        int bestRoom = 1;
        foreach (var cellsC in classify)
        {
            if (Vector2.Distance(cell.Pos, cellsC.Key.Pos) < bestDistance)
            {
                bestDistance = Vector2.Distance(cell.Pos, cellsC.Key.Pos);
                bestRoom = cellsC.Value;
            }
        }

        return bestRoom;
    }
    private List<Chunk> GetChunksNeighbors(Chunk chunk, Cell[,]cells)
    {
        List<Chunk> chunks = new List<Chunk>();
        foreach (Cell cell in chunk.cells)
        {
            foreach (var neighbor in Get4Neighbors(cell,cells))
            {
                if (neighbor == null) continue;
                if (neighbor.IsBlock) continue;
                if (chunk.cells.Contains(neighbor)) continue;
                if (neighbor.Chunk == chunk) continue;
                if (chunks.Contains(neighbor.Chunk)) continue;

                chunks.Add(neighbor.Chunk);
            }
        }

        return chunks;
    }
    private Cell[] Get4Neighbors(Cell cell, Cell[,]cells)
    {
        Cell[] neighbors = new Cell[4];
        neighbors[0] = GetCellFromPos(cell.Pos.x, cell.Pos.y + 1,cells);
        neighbors[1] = GetCellFromPos(cell.Pos.x + 1, cell.Pos.y,cells);
        neighbors[2] = GetCellFromPos(cell.Pos.x, cell.Pos.y - 1,cells);
        neighbors[3] = GetCellFromPos(cell.Pos.x - 1, cell.Pos.y,cells);
        return neighbors;
    }
    private void ClearDebugCells() {
        for (int i = _target.transform.childCount - 1; i > 0; i--) {
            DestroyImmediate(_target.transform.GetChild(i).gameObject);
        }
    }
    private void ClearDebugCells2() {
        foreach (var cell in _currentTestCells) {
            if (cell ==null) continue;
            if( cell.DebugCell!=null) DestroyImmediate(cell.DebugCell.gameObject);
        }
        _currentTestCells.Clear();
        for (int i = _target.transform.childCount - 1; i > 0; i--) {
            DestroyImmediate(_target.transform.GetChild(i).gameObject);
        }
    }

    private void BakeTerrainLocomotionData() {
        GenerateCells();
        CheckColliders();
        RecalculateChunks();
        Debug.Log("Terrain Locomotion Data Baked");
    }
    private int[] GetNeighborsID(Chunk chunk, List<Chunk> chunks)
    {
        int[] ids = new int[chunk.neighbors.Count];
        for (int i = 0; i < chunk.neighbors.Count; i++)
        {
            ids[i] = chunks.IndexOf(chunk.neighbors[i]);
        }

        return ids;
    }

    
     #region SaveLoadSystem

     
        public void SaveLocomotionData()
        {
            List<Chunk> chunks = _target.GetAllChunks().ToList();
            Cell[,] cells = _target.GetAllCells();
            if (_target.TerrainLocomotionData == null)
            {
                _target.TerrainLocomotionData = ScriptableObject.CreateInstance<TerrainLocomotionData>();
                _target.TerrainLocomotionData.MapName = SceneManager.GetActiveScene().name;
                                 if (!Directory.Exists("Assets/LocomotionData"))
                                 {
                    Directory.CreateDirectory("Assets/LocomotionData");
                }

                UnityEditor.AssetDatabase.CreateAsset(_target.TerrainLocomotionData,
                    "Assets/LocomotionData/TLD" + _target.TerrainLocomotionData.MapName + ".asset");
            }
            _target.TerrainLocomotionData.Size = _target.Size;
            _target.TerrainLocomotionData.Chunks = new ChunkSave[chunks.Count];
            for (int i = 0; i < _target.TerrainLocomotionData.Chunks.Length; i++)
            {
                _target.TerrainLocomotionData.Chunks[i] = new ChunkSave(i, chunks[i].GetCellPos(), GetNeighborsID(chunks[i], chunks),
                    chunks[i].Coordonate);
            }

            _target.TerrainLocomotionData.Cells = new CellSave[cells.Length];


            for (int x = 0; x < _target.Size.x * Metrics.chunkSize; x++)
            {
                for (int y = 0; y < _target.Size.y * Metrics.chunkSize; y++)
                {
                    _target.TerrainLocomotionData.Cells[x * Metrics.chunkSize * _target.Size.y + y] = cells[x, y].GetCellSave();

                }
            }

            Save(_target.TerrainLocomotionData);
            
            Debug.Log("Data Sava");
        }
        public void Save(TerrainLocomotionData terrainLocomotionData) {
            SerializedObject so = new SerializedObject(terrainLocomotionData);
            so.FindProperty("_size").vector2IntValue = terrainLocomotionData.Size;
            so.ApplyModifiedProperties();
           }
        
        #endregion
    
}
