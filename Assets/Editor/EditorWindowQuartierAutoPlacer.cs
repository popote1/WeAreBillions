using System.Collections.Generic;
using script;
using UnityEditor;
using UnityEngine;
using static AutoQuaterElement;
using Random = UnityEngine.Random;

public class EditorWindowQuartierAutoPlacer : EditorWindow {
    private Vector3 _startPos;
    private Vector3 _endPos;
    private bool _updateWorkingZone;
    bool _dragBox = false;
    private TestCell _prfTestCell;
    private float _offsetToTheGround =0.5f;
    private static EditorWindowQuartierAutoPlacer window;
    
    //BSP Parameters---------------//
    private int _propertyMinBorder = 7;
    private List<PropertyAutoQuarter> _propertyAutoQuarters;
    
    private CellAutoQuarter[,] _workingZone;
    private List<TestCell> _testCells =new List<TestCell>();
    
    private Vector3 GridOffSet {
        get { return new Vector3(
            Mathf.Min(_startPos.x, _endPos.x)
            ,_startPos.y+_offsetToTheGround
            ,Mathf.Min(_startPos.z, _endPos.z)); }
    }

    public Vector2Int WorkingZoneSize {
        get { return new Vector2Int(_workingZone.GetLength(0), _workingZone.GetLength(1)); }
    }

    [MenuItem("PopoteTools/WindowQuatierAutoPlacer")]
    public static void ShowWindow() {
        window = GetWindow<EditorWindowQuartierAutoPlacer>();
        window.titleContent = new GUIContent("QuartierAutoPlacer");
        SceneView.duringSceneGui += window.OnSceneGUI;
    }
    
    private void OnDisable() {
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearTestCell();
    }
    
    private void OnGUI() {
        _startPos =EditorGUILayout.Vector3Field("StartPos",_startPos);
        _endPos =EditorGUILayout.Vector3Field("EndPos",_endPos);
        _prfTestCell = (TestCell) EditorGUILayout.ObjectField(_prfTestCell, typeof(TestCell));
        if (GUILayout.Button("ReLinkToScene And rest")) {
            SceneView.duringSceneGui += OnSceneGUI;
            if( _testCells!=null) ClearTestCell();
            _testCells = new List<TestCell>();
        }
        if (GUILayout.Button("UpdateWorkingZone")) {
            _updateWorkingZone = true;
        }

        if (_prfTestCell != null) {
            _offsetToTheGround = EditorGUILayout.FloatField("TestCellOffsetToTheGRound", _offsetToTheGround);
            if (GUILayout.Button("GenerateGrid And DisplayCells")) {
                CreateNewWorkGrid();
                SpawnDebugCell();
            }
        }

        _propertyMinBorder = EditorGUILayout.IntField("BspMinSize", _propertyMinBorder);
        if (GUILayout.Button("Do BSP")) {
            PropertyAutoQuarter property = new PropertyAutoQuarter();
            property.Cell = _workingZone;
            _propertyAutoQuarters =DoBSP(property);
            ColorPropertyQuarter();
        }

        if (GUILayout.Button("Color Properties")) {
            foreach (var property in _propertyAutoQuarters) {
                property.SetCellType(this);
                property.ColorCells();
            }
        }
    }
    
    private void OnSceneGUI(SceneView sceneView) {
        DrawSelectionBox();
        
        if (!_updateWorkingZone) return;
        Event e = Event.current;
        Vector2 mousePosition = e.mousePosition;
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
        RaycastHit hit;
        Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);
        
        if (e.type == EventType.MouseDown && !_dragBox) {
            if (Physics.Raycast(ray, out  hit)) {
                _startPos = hit.point;
                _dragBox = true;
                OnGUI();
                return;
            }
        }
        if (e.type == EventType.MouseUp ) {
            if ( Physics.Raycast(ray, out hit))
            {
                _endPos = hit.point;
            }
            Debug.Log("StopSelectionBox");
            _dragBox = false;
            _updateWorkingZone = false;
            OnGUI();
        }
        if (_dragBox&&Physics.Raycast(ray, out hit )) {
                _endPos = hit.point;
                OnGUI();
                return;
        }
    }

    private void DrawSelectionBox() {
        Vector3[] points = new Vector3[4];
        Handles.color = Color.blue;

        points[0] = _startPos;
        points[1] = new Vector3(_startPos.x, _startPos.y, _endPos.z);
        points[2] = new Vector3(_endPos.x, _startPos.y, _endPos.z);
        points[3] = new Vector3(_endPos.x, _startPos.y, _startPos.z);
        
        Handles.DrawDottedLine(points[0], points[1], 1);
        Handles.DrawDottedLine(points[2], points[1], 1);
        Handles.DrawDottedLine(points[2], points[3], 1);
        Handles.DrawDottedLine(points[0], points[3], 1);
        Handles.color = new Color(0,0,1,0.2f);
        Handles.DrawAAConvexPolygon(points);
    }

    private void CreateNewWorkGrid() {
        int wight = Mathf.Abs(Mathf.RoundToInt(_startPos.x - _endPos.x));
        int height = Mathf.Abs(Mathf.RoundToInt(_startPos.z - _endPos.z));
        Debug.Log("Generat Grid of :"+ wight + ": wight and "+ height+":Height");
        _workingZone = new CellAutoQuarter[wight, height];
        for (int x = 0; x < wight; x++) {
            for (int y = 0; y < height; y++) {
                _workingZone[x, y] = new CellAutoQuarter(x, y);
            }
        }
    }
    
    #region BSP
    private List<PropertyAutoQuarter> DoBSP(PropertyAutoQuarter property) {
        List<PropertyAutoQuarter> openList = new List<PropertyAutoQuarter>();
        List<PropertyAutoQuarter> closeList = new List<PropertyAutoQuarter>();
        openList.Add(property);
        int security  =0;
        bool BspEnded = false;
        while (openList.Count >0) {
            security++;

            if (security > 1000)
            {
                Debug.LogWarning(" BSP Have done 1000, loop stop to prevent any freeze");
            }
            PropertyAutoQuarter currentProperty = openList[0];
            openList.Remove(currentProperty);

            if (!currentProperty.CanBeDividedOnX(_propertyMinBorder)&& !currentProperty.CanBeDividedOnY(_propertyMinBorder)) {
                closeList.Add(currentProperty);
                Debug.Log("BSP: Can't divideMore");
                continue;
            }

            int option = Random.Range(0, 2);
            if (option == 0) {
                if (currentProperty.CanBeDividedOnX(_propertyMinBorder)) {
                    openList.AddRange(BspCutOnX(currentProperty));
                }
                else {
                    openList.AddRange(BspCutOnY(currentProperty));
                }
            }
            else {
                if (currentProperty.CanBeDividedOnY(_propertyMinBorder)) {
                    openList.AddRange(BspCutOnY(currentProperty));
                }
                else {
                    openList.AddRange(BspCutOnX(currentProperty));
                }
            }
        }

        return closeList;
    }

    private PropertyAutoQuarter[] BspCutOnX(PropertyAutoQuarter property) {
        int cutValue = Random.Range(0,property.GetWight - _propertyMinBorder * 2)+_propertyMinBorder;
        PropertyAutoQuarter a = new PropertyAutoQuarter();
        PropertyAutoQuarter b = new PropertyAutoQuarter();
        a.Cell = new CellAutoQuarter[cutValue, property.GetHeight];
        b.Cell = new CellAutoQuarter[property.GetWight-cutValue, property.GetHeight];
        
        for (int x = 0; x < property.GetWight; x++) {
            for (int y = 0; y < property.GetHeight; y++) {
                //Debug.Log("Cell added ="+x+";"+y);
                if (x < cutValue) {
                    a.Cell[x, y] = property.Cell[x, y];
                }
                else {
                    b.Cell[x-cutValue, y] = property.Cell[x, y];
                }
            }
        }

        return new[] { a, b };
    }
    private PropertyAutoQuarter[] BspCutOnY(PropertyAutoQuarter property) {
        int cutValue = Random.Range(0,property.GetHeight - _propertyMinBorder * 2)+_propertyMinBorder;
        Debug.Log("Cut Value = "+cutValue+ "  height = "+property.GetHeight);
        PropertyAutoQuarter a = new PropertyAutoQuarter();
        PropertyAutoQuarter b = new PropertyAutoQuarter();
        a.Cell = new CellAutoQuarter[property.GetWight, cutValue];
        b.Cell = new CellAutoQuarter[property.GetWight, property.GetHeight-cutValue];
        
        for (int x = 0; x < property.GetWight; x++) {
            for (int y = 0; y < property.GetHeight; y++) {
                //Debug.Log("Cell added ="+x+";"+y);
                if (y < cutValue) {
                    a.Cell[x, y] = property.Cell[x, y];
                }
                else {
                    b.Cell[x, y-cutValue] = property.Cell[x, y];
                }
            }
        }
        return new[] { a, b };
    }
    #endregion
    #region DebugCells
    private void SpawnDebugCell() {
        ClearTestCell();
        _testCells = new List<TestCell>();
        for (int x = 0; x < _workingZone.GetLength(0); x++) {
            for (int y = 0; y < _workingZone.GetLength(1); y++) {
                _workingZone[x, y].TestCell =Instantiate(_prfTestCell,new Vector3(x, 0, y)+GridOffSet, Quaternion.identity);
                _testCells.Add(_workingZone[x, y].TestCell);
            }
        }
    }
    private void ClearTestCell() {
        foreach (var cell in _testCells) {
            if( cell==null)continue;
            DestroyImmediate(cell.gameObject);
        }
    }
    private void ColorPropertyQuarter() {
        foreach (var property in _propertyAutoQuarters) {
            Color color = Random.ColorHSV();
            foreach (var cell in property.Cell)
            {
                if (cell == null) continue;
                if (cell.TestCell == null) continue;
                cell.TestCell.Render.color = color;
            }
        }
    }
    #endregion

    
}