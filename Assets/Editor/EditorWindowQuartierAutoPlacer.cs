using System.Collections.Generic;
using script;
using Unity.Hierarchy;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using static AutoQuaterElement;
using Random = UnityEngine.Random;

public class EditorWindowQuartierAutoPlacer : EditorWindow {
    private Vector3 _startPos;
    private Vector3 _endPos;
    private bool _updateWorkingZone;
    bool _dragBox = false;
    private TestCell _prfTestCell;
    private GameObject _debugCellHolder;
    private float _offsetToTheGround =0.5f;
    private bool _showDebug;
    private bool _showObjectSpawningDetail;
    private bool _showSpawningParameters;
    
    
    
    private static EditorWindowQuartierAutoPlacer window;
    
    //BSP Parameters---------------//
    private int _propertyMinBorder = 7;
    private List<PropertyAutoQuarter> _propertyAutoQuarters;
    // Element Generation Parameters----------------------//
    private string _holderNameBuildings = "Buildings";
    private string _holderNameDestructibles = "Destructibles";
    private string _holderNameinsignifiats = "Insignifiants";
    private string _holderNameNoColliders = "NoColliders";
    private string _holderNameWalkBlockers = "WalkBlockers";

    public Transform _rootObject;
    public Transform _rootBuildings;
    public Transform _rootDestructible;
    public Transform _rootInsignifiants;
    public Transform _rootNoColliders;
    public Transform _rootWalkBlockers;

    public SoObjectList _objectListHouses;
    public SoObjectList _objectListCars;
    public SoObjectList _objectListPalissade;
    public SoObjectList _objectListInsignifiant;
    public SoObjectList _objectListWalkBlocker;
    public SoObjectList _objectListNoColliders;

    public float _palissadeChance = 50;
    public float _houseMinRot = -10;
    public float _houseMaxRot = 10;
    private float _carChanceToSpawn =0.5f;
    public float _carMinRot = -20;
    public float _carMaxRot = 20;
    
    private float _gardenDensity = 25;
    private float _gardenWalkBlockerPart = 20;
    private float _gardenInsignifiantPart = 40;
    private float _gardenNoCollidersPart= 40;
    

    private List<GameObject> _spawnedGameObjects = new List<GameObject>();
    private CellAutoQuarter[,] _workingZone;
    private List<TestCell> _testCells =new List<TestCell>();
    private Vector2 _scrollViewPosition;
    
    private Vector3 GridOffSet {
        get { return new Vector3(
            Mathf.Min(_startPos.x, _endPos.x)
            ,_startPos.y
            ,Mathf.Min(_startPos.z, _endPos.z)); }
    }
    
    public Vector2Int WorkingZoneSize {
        get { return new Vector2Int(_workingZone.GetLength(0), _workingZone.GetLength(1)); }
    }

    private Vector3 GetWorldPosFromCell(CellAutoQuarter cell) {
        if (cell == null) return GridOffSet;
        return new Vector3(cell.Pos.x, 0, cell.Pos.y) + GridOffSet;
    }
    private Vector3 GetWorldPosFromCellForPalisse(CellAutoQuarter cell, CellAutoQuarter originCell) {
        if (cell == null) return GridOffSet;
        return new Vector3(cell.PropertyPlace.x-originCell.PropertyPlace.x, 0, cell.PropertyPlace.y-originCell.PropertyPlace.y) ;
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
        ClearSpawnedGameObjects();
    }
    
    private void OnGUI() {
        _scrollViewPosition =EditorGUILayout.BeginScrollView(_scrollViewPosition, GUILayout.Height(position.height), GUILayout.Width(position.width));
        _propertyMinBorder = EditorGUILayout.IntField("BspMinSize", _propertyMinBorder);
        if (GUILayout.Button("UpdateWorkingZone")) {
            _updateWorkingZone = true;
        }
        _showDebug = EditorGUILayout.Foldout(_showDebug, "Show Debug");
        if (_showDebug) {
            _startPos = EditorGUILayout.Vector3Field("StartPos", _startPos);
            _endPos = EditorGUILayout.Vector3Field("EndPos", _endPos);
            _prfTestCell = (TestCell)EditorGUILayout.ObjectField(_prfTestCell, typeof(TestCell));
            if (GUILayout.Button("ReLinkToScene And rest")) {
                SceneView.duringSceneGui += OnSceneGUI;
                if (_testCells != null) ClearTestCell();
                _testCells = new List<TestCell>();
            }
            if (_prfTestCell != null) {
                _offsetToTheGround = EditorGUILayout.FloatField("TestCellOffsetToTheGRound", _offsetToTheGround);
                if (GUILayout.Button("GenerateGrid And DisplayCells")) {
                    CreateNewWorkGrid();
                    SpawnDebugCell();
                }
            }
            if (GUILayout.Button("Do BSP")) StartBsp(); 
            if (GUILayout.Button("Color Properties")) {
                foreach (var property in _propertyAutoQuarters) {
                    property.SetCellType(this);
                    property.ColorCells();
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Debug Cells")) ClearTestCell();
            if (GUILayout.Button("ClearAllData")) ClearAllData();
            EditorGUILayout.EndHorizontal();
        }
        _showObjectSpawningDetail = EditorGUILayout.Foldout(_showObjectSpawningDetail, "Show Object Spawning Details");
        if (_showObjectSpawningDetail)
        {
            if (GUILayout.Button("Find Roots Object")) CheckForRoots();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root Object");
            _rootObject = (Transform)EditorGUILayout.ObjectField(_rootObject, typeof(Transform));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Holder Buildings");
            _rootBuildings = (Transform)EditorGUILayout.ObjectField(_rootBuildings, typeof(Transform));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Holder Destructibles");
            _rootDestructible = (Transform)EditorGUILayout.ObjectField(_rootDestructible, typeof(Transform));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Holder Insignifiant");
            _rootInsignifiants = (Transform)EditorGUILayout.ObjectField(_rootInsignifiants, typeof(Transform));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Holder NoColliders");
            _rootNoColliders = (Transform)EditorGUILayout.ObjectField(_rootNoColliders, typeof(Transform));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Holder WalkBlocker");
            _rootWalkBlockers = (Transform)EditorGUILayout.ObjectField(_rootWalkBlockers, typeof(Transform));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ObjectList Houses");
            _objectListHouses = (SoObjectList)EditorGUILayout.ObjectField(_objectListHouses, typeof(SoObjectList));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ObjectList Cars");
            _objectListCars = (SoObjectList)EditorGUILayout.ObjectField(_objectListCars, typeof(SoObjectList));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ObjectList Palissades");
            _objectListPalissade =
                (SoObjectList)EditorGUILayout.ObjectField(_objectListPalissade, typeof(SoObjectList));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ObjectList Insignifiant");
            _objectListInsignifiant =
                (SoObjectList)EditorGUILayout.ObjectField(_objectListInsignifiant, typeof(SoObjectList));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ObjectList WalkBlocker");
            _objectListWalkBlocker =
                (SoObjectList)EditorGUILayout.ObjectField(_objectListWalkBlocker, typeof(SoObjectList));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ObjectList NoColliders");
            _objectListNoColliders =
                (SoObjectList)EditorGUILayout.ObjectField(_objectListNoColliders, typeof(SoObjectList));
            EditorGUILayout.EndHorizontal();
        }
        _showSpawningParameters = EditorGUILayout.Foldout(_showSpawningParameters, "Show Object Spawning Details");
        if (_showSpawningParameters) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Palissade Chance %");
            _palissadeChance = EditorGUILayout.Slider(_palissadeChance, 0, 100);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("House Random Rot");
            EditorGUILayout.BeginHorizontal();
            _houseMinRot =EditorGUILayout.FloatField(_houseMinRot);
            _houseMaxRot =EditorGUILayout.FloatField(_houseMaxRot);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref _houseMinRot, ref _houseMaxRot, -180,180);
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Garden density %");
            _gardenDensity = EditorGUILayout.Slider(_gardenDensity, 0, 100);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Garder WalkBlocker Part");
            EditorGUILayout.LabelField("Garden Insignifiant Part");
            EditorGUILayout.LabelField("Garden NoCollider Part");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_gardenWalkBlockerPart.ToString()+"%", style);
            EditorGUILayout.LabelField((_gardenInsignifiantPart-_gardenWalkBlockerPart).ToString()+"%", style);
            EditorGUILayout.LabelField((100-(_gardenInsignifiantPart)).ToString()+"%", style);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref _gardenWalkBlockerPart, ref _gardenInsignifiantPart, 0,100);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Car Spawn Chance %");
            _carChanceToSpawn = EditorGUILayout.Slider(_carChanceToSpawn, 0, 100);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Car Random Rot");
            EditorGUILayout.BeginHorizontal();
            _carMinRot =EditorGUILayout.FloatField(_carMinRot);
            _carMaxRot =EditorGUILayout.FloatField(_carMaxRot);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref _carMinRot, ref _carMaxRot, -180,180);
            if (GUILayout.Button("Spawn Quarter")) SpawnQuarterElements();
        }

        if (GUILayout.Button("GeneralQuarter"))GeneralQuarter();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ApplySpawnedObjects"))ApplySpawnedObjects();
        if (GUILayout.Button("Clear Spawned Objects"))ClearSpawnedGameObjects();
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.EndScrollView();
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
        _workingZone = new CellAutoQuarter[wight, height];
        for (int x = 0; x < wight; x++) {
            for (int y = 0; y < height; y++) {
                _workingZone[x, y] = new CellAutoQuarter(x, y);
            }
        }
    }

    private void ClearAllData() {
        ClearTestCell();
        _startPos = Vector3.zero;
        _endPos = Vector3.zero;
        _propertyAutoQuarters.Clear();
        _updateWorkingZone = false;
        _workingZone = new CellAutoQuarter[0, 0];
        _testCells.Clear();
    }

    private void CheckForRoots()
    {
        if (_rootObject == null) {
            Debug.Log("No Root Object find");
            _rootObject = (new GameObject()).transform;
            _rootObject.gameObject.name = "NewQuarter";
        }
        _rootBuildings = GetRootObjectOfName(_holderNameBuildings);
        _rootDestructible = GetRootObjectOfName(_holderNameDestructibles);
        _rootInsignifiants = GetRootObjectOfName(_holderNameinsignifiats);
        _rootNoColliders = GetRootObjectOfName(_holderNameNoColliders);
        _rootWalkBlockers = GetRootObjectOfName(_holderNameWalkBlockers);

    }

    private Transform GetRootObjectOfName(string name) {
        for (int i = 0; i < _rootObject.childCount; i++) {
            if (_rootObject.GetChild(i).name == name) {
                return _rootObject.GetChild(i).transform;
            }
        }
        GameObject go = new GameObject();
        go.transform.SetParent(_rootObject);
        go.name = name;
        return go.transform;
    }

    private void ClearSpawnedGameObjects() {
        for (int i = _spawnedGameObjects.Count - 1; i >= 0; i--) {
            DestroyImmediate(_spawnedGameObjects[i]);
        }
        _spawnedGameObjects.Clear();
        
    }

    private void ApplySpawnedObjects() {
        for (int i = 0; i < _rootWalkBlockers.childCount; i++) {
            _rootWalkBlockers.GetChild(i).gameObject.isStatic = true;
        }
        for (int i = 0; i < _rootNoColliders.childCount; i++) {
            _rootNoColliders.GetChild(i).gameObject.isStatic = true;
        }
        _spawnedGameObjects.Clear();
        if( _rootInsignifiants.childCount==0)DestroyImmediate(_rootInsignifiants.gameObject);
        if( _rootWalkBlockers.childCount==0)DestroyImmediate(_rootWalkBlockers.gameObject);
        if( _rootDestructible.childCount==0)DestroyImmediate(_rootDestructible.gameObject);
        if( _rootNoColliders.childCount==0)DestroyImmediate(_rootNoColliders.gameObject);
        if( _rootBuildings.childCount==0)DestroyImmediate(_rootBuildings.gameObject);
    }

    private void GeneralQuarter() {
        CreateNewWorkGrid();
        StartBsp(); 
        foreach (var property in _propertyAutoQuarters) property.SetCellType(this);
        CheckForRoots();
        SpawnQuarterElements();
    }
    #region SpawningElements
    private void SpawnQuarterElements() {
        foreach (var property in _propertyAutoQuarters) {
            SpawnHouse(property);
            SpawnPalissade(property);
            property.SetHouseCells();
            SpawnCar(property);
            SpawnGardenObjects(property);
        }
    }
    
    private void SpawnHouse(PropertyAutoQuarter property) {
        GameObject house  =Instantiate(
            _objectListHouses.GetRandomObject()
            ,GetWorldPosFromCell(property.CenterCell)
            ,Quaternion.identity) ;

        house.transform.forward = new Vector3(property.DirectionToRoad.x, 0,property.DirectionToRoad.y );
        house.transform.eulerAngles += new Vector3(0, Random.Range(_houseMinRot, _houseMaxRot), 0);
        house.transform.SetParent(_rootBuildings);
        _spawnedGameObjects.Add(house);
    }

    private void SpawnPalissade(PropertyAutoQuarter property) {
        if (Random.Range(0, 100) > _palissadeChance) {
            property.SetBorderCellsToFree();
            return;
        }
        CellAutoQuarter[] keycell = property.GetPalissadeKeyCells();
        if (keycell == null || keycell.Length == 1) return;

        GameObject palissade =Instantiate(_objectListPalissade.GetRandomObject(), GetWorldPosFromCell(keycell[0]), Quaternion.identity);
        palissade.transform.SetParent(_rootInsignifiants);
        _spawnedGameObjects.Add(palissade);
        SplineContainer spline = palissade.GetComponent<SplineContainer>();
        spline.Spline = new Spline(keycell.Length);
        for (int i = 0; i < keycell.Length; i++) {
            spline.Spline.Add(new BezierKnot(GetWorldPosFromCellForPalisse(keycell[i],keycell[0])));
        }
        spline.Warmup();
    }

    private void SpawnCar(PropertyAutoQuarter property) {
        if (Random.Range(0, 100) > _carChanceToSpawn) return;
        Vector3 pos = GetWorldPosFromCell(property.GetRandomPassCell());
        GameObject car = Instantiate(_objectListCars.GetRandomObject(), pos, Quaternion.identity);
        car.transform.SetParent(_rootDestructible);
        car.transform.forward = new Vector3(property.DirectionToRoad.x, 0, property.DirectionToRoad.y);
        car.transform.eulerAngles += new Vector3(0, Random.Range(_carMinRot, _carMaxRot), 0);
        _spawnedGameObjects.Add(car);
    }

    private void SpawnGardenObjects(PropertyAutoQuarter property) {
        List<CellAutoQuarter> freeCells = property.GetFreeCells();
        int spawnCount = Mathf.RoundToInt(freeCells.Count * (_gardenDensity / 100));
        for (int i = 0; i < spawnCount; i++) {
            CellAutoQuarter cell = freeCells[Random.Range(0, freeCells.Count)];
            GameObject go;
            float randomValue = Random.Range(0, 100);
            if (randomValue < _gardenWalkBlockerPart) {
                //SpawnWalkBlokers
                go= Instantiate(_objectListWalkBlocker.GetRandomObject(), GetWorldPosFromCell(cell), Quaternion.identity);
                go.transform.SetParent(_rootWalkBlockers);
            }
            else if (randomValue < _gardenWalkBlockerPart-_gardenInsignifiantPart) {
                //SpawnInsignifiant
                go= Instantiate(_objectListInsignifiant.GetRandomObject(), GetWorldPosFromCell(cell), Quaternion.identity);
                go.transform.SetParent(_rootInsignifiants);
            }
            else {
                //Spawn NoColliders
                go= Instantiate(_objectListNoColliders.GetRandomObject(), GetWorldPosFromCell(cell), Quaternion.identity);
                go.transform.SetParent(_rootNoColliders);
            }
            go.transform.eulerAngles += new Vector3(0, Random.Range(-180, 180), 0);
            _spawnedGameObjects.Add(go);
            freeCells.Remove(cell);
        }
    }
    #endregion
    #region BSP

    private void StartBsp() {
        PropertyAutoQuarter property = new PropertyAutoQuarter();
        property.Cell = _workingZone;
        _propertyAutoQuarters = DoBSP(property);
        ColorPropertyQuarter();
    }
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
                    a.Cell[x, y].PropertyPlace = new Vector2Int(x, y);
                }
                else {
                    b.Cell[x-cutValue, y] = property.Cell[x, y];
                    b.Cell[x-cutValue, y].PropertyPlace = new Vector2Int(x-cutValue, y);
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
                    a.Cell[x, y].PropertyPlace = new Vector2Int(x, y);
                }
                else {
                    b.Cell[x, y-cutValue] = property.Cell[x, y];
                    b.Cell[x, y-cutValue].PropertyPlace = new Vector2Int(x, y-cutValue);
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
        _debugCellHolder = new GameObject();
        _debugCellHolder.name = "DebugCellHolder";
        for (int x = 0; x < _workingZone.GetLength(0); x++) {
            for (int y = 0; y < _workingZone.GetLength(1); y++) {
                _workingZone[x, y].TestCell =Instantiate(_prfTestCell,new Vector3(x, 0+_offsetToTheGround, y)+GridOffSet, Quaternion.identity);
                _testCells.Add(_workingZone[x, y].TestCell);
                _workingZone[x, y].TestCell.transform.SetParent(_debugCellHolder.transform);
            }
        }
    }
    private void ClearTestCell() {
        foreach (var cell in _testCells) {
            if( cell==null)continue;
            DestroyImmediate(cell.gameObject);
        }
        DestroyImmediate(_debugCellHolder);
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