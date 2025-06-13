using System;
using System.Collections.Generic;
using System.Linq;
using script.UIs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace script
{
    public class GameController:MonoBehaviour
    {
        public Camera Camera;
        public GridManager GridManager;
        public ZombieAgent PrefabsZombieAgent;
        [SerializeField] private HUDSelectionBoxDisplayer _hudSelectionBoxDisplayer;
        [SerializeField] private GameObject _prefabsVFXMoveOrder;

        [Header("Strat Zombie")] 
        public Transform[] zombies;

        [Header("SelectedZombie")] public float SelectionBoxMin = -10;
        public float SelectionBoxMax = 10;
        public LayerMask GroundLayer;
        public LayerMask SelectingLayer;
        public List<GridAgent> Selected = new List<GridAgent>();
        public float MinimumSelectBoxSize = 0.1f; 
        public GameObject DebugCube;
        [Header("CameraScroll")] 
        public int PixelBorder = 50;
        

        private Vector2Int originChunkTarget = new Vector2Int(0, 0);
        private bool _isInSelectionBox;
        private Vector2 _startSelectionBox;

        //public event EventHandler<List<GridAgent>> OnSelectionChange; 
        public static Action<GridAgent> AddAgentToSelection; 
        
        private void Start() {
            AddAgentToSelection += AddGridAGentToSelection;
            if (zombies == null || zombies.Length == 0) return;
            foreach (var z in zombies) {
                if (z == null) continue;
                ZombieAgent zombie = Instantiate(PrefabsZombieAgent,  z.position+ new Vector3(0, 0.5f, 0),
                    Quaternion.identity);
                zombie.Generate(GridManager);
            }
            
            StaticEvents.OnSetGameOnPause += DebugOpenMenuPause;
            StaticEvents.OnSubmitSelectionChange+= StaticEventsOnOnSubmitSelectionChange; 
        }

        

        private void OnDestroy()
        {
            StaticEvents.OnSetGameOnPause -= DebugOpenMenuPause;
        }

        public void Update()
        {
            
            if (StaticData.BlockControls) return;
            ManageBorderCameraMovement();
            if (Input.GetKeyDown(KeyCode.Escape))ManagePressEscape();
            if (Input.GetKeyDown(KeyCode.F2))ManagerSelectAllZombies();
            if (Input.GetButton("Fire1")) ManageBoxSelectionDisplay();
           
            if (Input.GetButtonUp("Fire1")) 
            {
                if (!CanBeSelectBox(_startSelectionBox , (Vector2)Input.mousePosition)) ManageRayCastSelection();
                else ManageBoxSelection();
                

                _isInSelectionBox = false;
                StaticData.IsDraging = false;
                if (_hudSelectionBoxDisplayer) _hudSelectionBoxDisplayer.CloseSelectionBox();
            }
            
            if (Input.GetButtonDown("Fire2")) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    if (hit.collider.transform.GetComponent<House>())
                    {
                        ManageAttackBuilding(hit.collider.transform.GetComponent<House>());
                        Debug.Log("Click On A House");
                        return;
                    }
                    
                    Cell targetCell = GridManager.GetCellFromWorldPos(hit.point);
                    if (Input.GetKey(KeyCode.LeftShift)) {
                        Debug.Log("FollowOrder");
                        ManagerGiveExtraOrder(targetCell);
                        if (_prefabsVFXMoveOrder != null) {
                            Instantiate(_prefabsVFXMoveOrder, hit.point, quaternion.identity);
                        }
                        return;
                    }
                    ManagerGiveMoveOrder(targetCell);
                    if (_prefabsVFXMoveOrder != null) {
                        Instantiate(_prefabsVFXMoveOrder, hit.point, quaternion.identity);
                    }
                }
            }
            
            
            
            if (Input.GetKeyDown(KeyCode.A)&& StaticData.CheatEnableZombieSpawning) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    Cell cell = GridManager.GetCellFromWorldPos(hit.point);
                    ZombieAgent zombie = Instantiate(PrefabsZombieAgent, hit.point + new Vector3(0, 0.5f, 0),
                        Quaternion.identity);
                    zombie.Generate(GridManager);
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                    Cell cell = GridManager.GetCellFromWorldPos(hit.point);

                    GridManager.ColorAllDebugGridToColor(Color.white);
                    foreach (var neibourgh in GridManager.GetCellNeighborRange(cell, 3)) {
                        neibourgh.ColorDebugCell(Color.green);
                    }
                }
            }
        }

        public void CalculateSelectionPathFinding(Cell targetCell)
        {
            List<Chunk> startchunks = new List<Chunk>();
            
            List<Chunk> totalChunks = GetChunksPathFromZombie(Selected, targetCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
            subgrid.StartCalcFlowfield(new []{targetCell});

            foreach (var zombieAgent in Selected)
            {
                if (zombieAgent == null) continue;
                zombieAgent.SetNewSubGrid(subgrid);
            }
            
            GridManager.ColorAllDebugGridToColor(Color.white);

            foreach (var chunk in totalChunks)
            {
                foreach (var cell in chunk.cells)
                {
                    cell.ColorDebugCell(Color.blue);
                }
            }
            foreach (var chunk in startchunks) {
                foreach (var cell in chunk.cells) {
                    cell.ColorDebugCell(Color.red);
                }
            }

            foreach (var cell in targetCell.Chunk.cells) {
                cell.ColorDebugCell(Color.green);
            }
            //foreach (var chunk in pathChunks) {
                //    foreach (var cell in chunk.cells) {
                //        cell.ColorDebugCell(Color.green);
                //    }
                //}
//
                //foreach (var chunk in GridManager.GetNeighborsOfPath(pathChunks)) {
                //    foreach (var cell in chunk.cells) {
                //        cell.ColorDebugCell(Color.yellow);
                //    }
                //}
        }
        public void CalculateSelectionExtraPathFinding(Cell targetCell)
        {
            List<Chunk> startchunks = new List<Chunk>();
            
            List<Chunk> totalChunks = GetChunksPathFromSubGrid(targetCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
            subgrid.StartCalcFlowfield(new []{targetCell});
            
            List<Subgrid> analizeSubgrid = new List<Subgrid>();
            foreach (var zombieAgent in Selected) {
                if (zombieAgent == null) continue;
                if (zombieAgent.Subgrid != null) {
                    if( analizeSubgrid.Contains(zombieAgent.Subgrid.GetLastSubgrid()))return;
                    zombieAgent.Subgrid.GetLastSubgrid().SetNextSubGrid( subgrid);
                    analizeSubgrid.Add(zombieAgent.Subgrid.GetLastSubgrid());
                }
                else zombieAgent.SetNewSubGrid( subgrid);
            }
        }

        private void ManageRayCastSelection() {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            RaycastHit hit;
            if(Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                ClearSelection();
                if (hit.collider.GetComponent<ZombieAgent>()) {
                    Selected = new List<GridAgent>() {hit.collider.GetComponent<ZombieAgent>()};
                    SetSelection(Selected);
                }
            }
        }
        public void ManageBoxSelectionDisplay() {
            if (!_isInSelectionBox) {
                _startSelectionBox = Input.mousePosition;
                _isInSelectionBox = true;
                StaticData.IsDraging = true;
            }

            Vector2 center = (_startSelectionBox + (Vector2) Input.mousePosition) / 2;
            Vector2 size = new Vector2(
                Mathf.Abs(_startSelectionBox.x - Input.mousePosition.x),
                Mathf.Abs(_startSelectionBox.y - Input.mousePosition.y));
            
            if( _hudSelectionBoxDisplayer )_hudSelectionBoxDisplayer.SetSelectionBox(center, size);
        }
        
        public void ManageBoxSelection() {
            Vector2[] points2D = new Vector2[4];
            Vector3[] points3D = new Vector3[8];

            points2D[0] = _startSelectionBox;
            points2D[1] = new Vector2(_startSelectionBox.x, Input.mousePosition.y);
            points2D[2] = Input.mousePosition;
            points2D[3] = new Vector2(Input.mousePosition.x, _startSelectionBox.y);


            if (_startSelectionBox == (Vector2)Input.mousePosition) return;
            RaycastHit hit;
            for (int i = 0; i < points2D.Length; i++) {
                if (Physics.Raycast(Camera.ScreenPointToRay(points2D[i]), out hit)) {
                    points3D[i] = new Vector3(hit.point.x, SelectionBoxMin, hit.point.z);
                    points3D[i+4] = new Vector3(hit.point.x, SelectionBoxMax, hit.point.z);
                }
                else {
                    points3D[i] = points3D[i + 4] = Vector3.zero;
                }
            }

            int[] triangle ={
                0,3,1,
                1,3,2,
                4,0,1,
                4,1,5,
                4,3,0,
                4,7,3,
                5,1,2,
                6,5,2,
                3,6,2,
                7,6,3,
                4,5,7,
                5,6,7,
            };
            Mesh mesh = new Mesh();
            mesh.vertices = points3D;
            mesh.triangles = triangle;

            MeshCollider meshcollier = gameObject.AddComponent<MeshCollider>();
            meshcollier.sharedMesh = mesh;
            meshcollier.convex = true;
            meshcollier.isTrigger = true;

            if (DebugCube != null)
            {
                GameObject go =Instantiate(DebugCube);
                go.GetComponent<MeshFilter>().mesh = mesh;
            }
            
            Destroy(meshcollier , 0.2f);
            ClearSelection();
        }
        private void OnTriggerEnter(Collider other) {
           
            if (other.GetComponent<ZombieAgent>() != null&&!Selected.Contains(other.GetComponent<ZombieAgent>())) {
                Selected.Add(other.GetComponent<ZombieAgent>());
                other.GetComponent<ZombieAgent>().IsSelected = true;
            }
            StaticEvents.ChangeSelection(Selected);
            
        }
        private void ManagerGiveMoveOrder(Cell targetCell) {
            Chunk target = targetCell.Chunk;
            if (target != null) {
                if (Selected.Count > 0) {
                    CalculateSelectionPathFinding(targetCell);
                    return;
                }

                Debug.Log("Try doing A start on chunks");
                GridManager.ColorAllDebugGridToColor(Color.white);
                List<Chunk> list =
                    GridManager.GetAStartPath(GridManager.GetCellFromPos(originChunkTarget.x, originChunkTarget.y).Chunk,
                        target);
                originChunkTarget = targetCell.Pos;
                Debug.Log("Done");
                if (list == null)
                {
                    Debug.Log("No path found");
                    return;
                }
                foreach (var chunk in list) {
                    foreach (var cell in chunk.cells) {
                        cell.ColorDebugCell(Color.green);
                    }
                }

                foreach (var chunk in GridManager.GetNeighborsOfPath(list)) {
                    foreach (var cell in chunk.cells) {
                        cell.ColorDebugCell(Color.yellow);
                    }
                }
            }
        }

        private void ManagerGiveExtraOrder(Cell targetCell)
        {
            Chunk target = targetCell.Chunk;
            if (target != null) {
                if (Selected.Count > 0) {
                    CalculateSelectionExtraPathFinding(targetCell);
                }
            }
        }
        private List<Chunk> GetChunksPathFromZombie(List<GridAgent> zombie, Cell targetCell) {
            List<Chunk> startchunks = new List<Chunk>();
            List<Chunk> pathChunks = new List<Chunk>();
            foreach (var ZombieAgent in Selected) {
                if (ZombieAgent == null) continue;
                if (!startchunks.Contains(
                    GridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk))
                {
                    startchunks.Add(GridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk);
                }
            }

            if (startchunks.Count == 0) {
                Debug.LogWarning("StartChunks not founds ");
                return null;
            }

            foreach (var chunk in startchunks) {
                foreach (var chunkpath in GridManager.GetAStartPath(chunk, targetCell.Chunk)) {
                    if (!pathChunks.Contains(chunkpath)) pathChunks.Add(chunkpath);
                }
            }
            pathChunks.AddRange(GridManager.GetNeighborsOfPath(pathChunks));
            return pathChunks;
        }
        
        private List<Chunk> GetChunksPathFromSubGrid( Cell targetCell) {
            List<Chunk> startchunks = new List<Chunk>();
            List<Chunk> pathChunks = new List<Chunk>();
            foreach (var ZombieAgent in Selected) {
                if (ZombieAgent == null) continue;
                if (ZombieAgent.Subgrid == null) {
                    if (!startchunks.Contains(
                        GridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk)) {
                        startchunks.Add(GridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk);
                    }
                }
                else {
                    foreach (var target in ZombieAgent.Subgrid.GetLastSubgrid().TargetCells) {
                        if (!startchunks.Contains(target.Chunk)) {
                            startchunks.Add(target.Chunk);
                        }
                    }
                }
            }

            if (startchunks.Count == 0) {
                Debug.LogWarning("StartChunks not founds ");
                return null;
            }

            foreach (var chunk in startchunks) {
                foreach (var chunkpath in GridManager.GetAStartPath(chunk, targetCell.Chunk)) {
                    if (!pathChunks.Contains(chunkpath)) pathChunks.Add(chunkpath);
                }
            }
            pathChunks.AddRange(GridManager.GetNeighborsOfPath(pathChunks));
            return pathChunks;
        }
        private void ManageAttackBuilding(House house) {
            Cell centerCell = GridManager.GetCellFromWorldPos(house.transform.position);
            List<Cell> investigatedCells = GridManager.GetCellNeighborRange(centerCell, 5);
            List<Cell> homeCells = new List<Cell>();
            
            GridManager.ColorAllDebugGridToColor(Color.white);
            foreach (var neighbor in investigatedCells) {
                if (neighbor.CheckCellColliderContain(house.gameObject)) {
                    homeCells.Add(neighbor);
                    neighbor.ColorDebugCell(Color.blue);
                    continue;
                }
                neighbor.ColorDebugCell(Color.green);
            }

            
            List<Chunk> totalChunks = GetChunksPathFromZombie(Selected, centerCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
            subgrid.StartAttackBuilding(homeCells.ToArray());
            
            foreach (var zombieAgent in Selected)
            {
                if (zombieAgent == null) continue;
                zombieAgent.SetNewSubGrid(subgrid);
            }



        }
        public void ClearSelection() {
            foreach (var zombie in Selected) {
                if (zombie != null) zombie.IsSelected = false;
            }
            Selected.Clear();
            StaticEvents.ChangeSelection(Selected);
        }

        public void AddGridAGentToSelection(GridAgent gridAgent) {
            if (gridAgent == null) return;
            Selected.Add(gridAgent);
            gridAgent.IsSelected = true;
            StaticEvents.ChangeSelection(Selected);
        }

        public void SetSelection(List<GridAgent> selection)
        {
            ClearSelection();
            foreach (var gridAgent in selection) {
                gridAgent.IsSelected = true;
            }

            Selected = selection;
            StaticEvents.ChangeSelection(selection);
        }

        private void ManageBorderCameraMovement()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 dir = Vector3.zero;
            if( mousePosition.x<PixelBorder) dir+= Vector3.left;
            if( mousePosition.x>Screen.width-PixelBorder) dir+= Vector3.right;
            if( mousePosition.y<PixelBorder) dir+= Vector3.back;
            if( mousePosition.y>Screen.height-PixelBorder) dir+= Vector3.forward;
            StaticData.CameraMoveVector = dir;
        }

        private void ManagePressEscape() {
            if (Selected.Count > 0) {
                ClearSelection();
            }
            else {
                Debug.Log("Open Menu Pause 0");
                StaticEvents.SetGameOnPause();
            }
        }

        private void DebugOpenMenuPause(object sender, bool e) {
            Debug.Log("Open Menu Pause");
        }

        private void ManagerSelectAllZombies() {
            ClearSelection();
            foreach (var zombie in StaticData.AllZombies) {
                AddGridAGentToSelection(zombie);
            }
        }
        
        private void StaticEventsOnOnSubmitSelectionChange(object sender, List<GridAgent> e) {
            SetSelection(e);
        }

        private bool CanBeSelectBox(Vector2 a, Vector2 b) {
            if (Mathf.Abs(a.x - b.x) < MinimumSelectBoxSize) return false;
            if (Mathf.Abs(a.y - b.y) < MinimumSelectBoxSize) return false;
            return true;
        }
    }
    
}