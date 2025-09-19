using System;
using System.Collections.Generic;
using System.Linq;
using script.UIs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace script
{
    public class GameController:MonoBehaviour
    {
        [SerializeField]private Camera _camera;
        [SerializeField]private GridManager _gridManager;
        [SerializeField] private HUDSelectionBoxDisplayer _hudSelectionBoxDisplayer;
        [SerializeField] private VFXPoolManager.VFXPooledType _vfxMoveOrderType = VFXPoolManager.VFXPooledType.MoveOrder;
        [SerializeField] private GameObject _prefabsVFXMoveOrder;

        [Header("SelectedZombie")] 
        [SerializeField]private List<GridAgent> _selected = new List<GridAgent>(); 
        [Header("CameraScroll")] 
        [SerializeField]private int _pixelBorder = 50;
        [Header("Cheat&Debug")]
        [SerializeField]private ZombieAgent _prfZombieAgent;
        

        private Vector2Int originChunkTarget = new Vector2Int(0, 0);
        private bool _isInSelectionBox;
        private Vector2 _startSelectionBox;
        
        private void Start() {
            StaticEvents.OnAddAgentToSelection += AddGridAGentToSelection;
            StaticEvents.OnSubmitSelectionChange+= StaticEventsOnOnSubmitSelectionChange; 
        }

        private void OnDestroy() {
            StaticEvents.OnAddAgentToSelection -= AddGridAGentToSelection;
            StaticEvents.OnSubmitSelectionChange-= StaticEventsOnOnSubmitSelectionChange; 
        }

        public void Update() {
            if (StaticData.BlockControls) return;
            ManageBorderCameraMovement();
            if (Input.GetKeyDown(KeyCode.Escape))ManagePressEscape();
            if (Input.GetKeyDown(KeyCode.F2))ManagerSelectAllZombies();
            if (Input.GetButton("Fire1")) ManageBoxSelectionDisplay();
            if (Input.GetButtonUp("Fire1")) ManageSelection();
            if (Input.GetButtonDown("Fire2")) ManageGiveOrder();
            if (Input.GetKeyDown(KeyCode.A)&& StaticData.CheatEnableZombieSpawning) CheatSpawnZombie();
        }
        #region Selection 
        private void ManageSelection() {
            if (!CanBeSelectBox(_startSelectionBox , (Vector2)Input.mousePosition)) ManageRayCastSelection();
            else ManageBoxSelection();
                

            _isInSelectionBox = false;
            StaticData.IsDraging = false;
            if (_hudSelectionBoxDisplayer) _hudSelectionBoxDisplayer.CloseSelectionBox();
        }
        private void ManageRayCastSelection() {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            RaycastHit hit;
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                ClearSelection();
                if (hit.collider.GetComponent<ZombieAgent>()) {
                    _selected = new List<GridAgent>() {hit.collider.GetComponent<ZombieAgent>()};
                    _selected[0].IsSelected = true;
                    StaticEvents.ChangeSelection(_selected);
                }
            }
            
        }
        private void ManageBoxSelectionDisplay() {
            if (!_isInSelectionBox) {
                _startSelectionBox = Input.mousePosition;
                _isInSelectionBox = true;
                StaticData.IsDraging = true;
            }

            if (!CanBeSelectBox(_startSelectionBox, Input.mousePosition)) return;
            
            Vector2 center = (_startSelectionBox + (Vector2) Input.mousePosition) / 2;
            Vector2 size = new Vector2(
                Mathf.Abs(_startSelectionBox.x - Input.mousePosition.x),
                Mathf.Abs(_startSelectionBox.y - Input.mousePosition.y));
            
            if( _hudSelectionBoxDisplayer )_hudSelectionBoxDisplayer.SetSelectionBox(center, size);
        }
        private void ManageBoxSelection() {
            Vector2[] points2D = new Vector2[4];
            Vector3[] points3D = new Vector3[8];

            points2D[0] = _startSelectionBox;
            points2D[1] = new Vector2(_startSelectionBox.x, Input.mousePosition.y);
            points2D[2] = Input.mousePosition;
            points2D[3] = new Vector2(Input.mousePosition.x, _startSelectionBox.y);


            if (_startSelectionBox == (Vector2)Input.mousePosition) return;
            RaycastHit hit;
            for (int i = 0; i < points2D.Length; i++) {
                if (Physics.Raycast(_camera.ScreenPointToRay(points2D[i]), out hit)) {
                    points3D[i] = new Vector3(hit.point.x, Metrics.SelectionBoxMinY, hit.point.z);
                    points3D[i+4] = new Vector3(hit.point.x, Metrics.SelectionBoxMaxY, hit.point.z);
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
            
            Destroy(meshcollier , 0.2f);
            ClearSelection();
        }
        private void OnTriggerEnter(Collider other) {
           
            if (other.GetComponent<ZombieAgent>() != null&&!_selected.Contains(other.GetComponent<ZombieAgent>())) {
                _selected.Add(other.GetComponent<ZombieAgent>());
                other.GetComponent<ZombieAgent>().IsSelected = true;
            }
            StaticEvents.ChangeSelection(_selected);
            
        }
        private void ClearSelection() {
            foreach (var zombie in _selected) {
                if (zombie != null) zombie.IsSelected = false;
            }
            _selected.Clear();
            StaticEvents.ChangeSelection(_selected);
        }
        private void SetSelection(List<GridAgent> selection)
        {
            Debug.Log("Selection submited = " + selection.Count);
            ClearSelection();
            foreach (var gridAgent in selection) {
                gridAgent.IsSelected = true;
            }

            _selected = selection;
            StaticEvents.ChangeSelection(selection);
        }
        private void ManagerSelectAllZombies() {
            ClearSelection();
            foreach (var zombie in StaticData.AllZombies) {
                AddGridAGentToSelection(null, zombie);
            }
        }
        #endregion
        
        #region Orders
        private void ManageGiveOrder() {
            RaycastHit hit;
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                if (hit.collider.transform.GetComponent<House>()) {
                    ManageAttackBuilding(hit.collider.transform.GetComponent<House>());
                    return;
                }
                    
                Cell targetCell = _gridManager.GetCellFromWorldPos(hit.point);
                if (Input.GetKey(KeyCode.LeftShift)) {
                    ManagerGiveExtraOrder(targetCell);
                    ManageSpawnVFXMoveOrder(hit.point);
                    return;
                }
                ManagerGiveMoveOrder(targetCell);
                ManageSpawnVFXMoveOrder(hit.point);
            }
        }
        private void CalculateSelectionPathFinding(Cell targetCell) {
            List<Chunk> totalChunks = GetChunksPathFromZombie(_selected, targetCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), _gridManager.Size, _gridManager.Offset);
            subgrid.StartCalcFlowfield(new []{targetCell});

            foreach (var zombieAgent in _selected) {
                if (zombieAgent == null) continue;
                zombieAgent.SetNewSubGrid(subgrid);
            }
        }
        private void CalculateSelectionExtraPathFinding(Cell targetCell) {
            List<Chunk> totalChunks = GetChunksPathFromSubGrid(targetCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), _gridManager.Size, _gridManager.Offset);
            subgrid.StartCalcFlowfield(new []{targetCell});
            
            List<Subgrid> analizeSubgrid = new List<Subgrid>();
            foreach (var zombieAgent in _selected) {
                if (zombieAgent == null) continue;
                if (zombieAgent.Subgrid != null) {
                    if( analizeSubgrid.Contains(zombieAgent.Subgrid.GetLastSubgrid()))return;
                    zombieAgent.Subgrid.GetLastSubgrid().SetNextSubGrid( subgrid);
                    analizeSubgrid.Add(zombieAgent.Subgrid.GetLastSubgrid());
                }
                else zombieAgent.SetNewSubGrid( subgrid);
            }
        }
        private void ManagerGiveMoveOrder(Cell targetCell) {
            Chunk target = targetCell.Chunk;
            if (target != null) {
                if (_selected.Count > 0) {
                    CalculateSelectionPathFinding(targetCell);
                    return;
                }
                List<Chunk> list =
                    _gridManager.GetAStartPath(_gridManager.GetCellFromPos(originChunkTarget.x, originChunkTarget.y).Chunk,
                        target);
                originChunkTarget = targetCell.Pos;
            }
        }
        private void ManagerGiveExtraOrder(Cell targetCell) {
            Chunk target = targetCell.Chunk;
            if (target != null) {
                if (_selected.Count > 0) {
                    CalculateSelectionExtraPathFinding(targetCell);
                }
            }
        }
        private List<Chunk> GetChunksPathFromZombie(List<GridAgent> zombie, Cell targetCell) {
            List<Chunk> startchunks = new List<Chunk>();
            List<Chunk> pathChunks = new List<Chunk>();
            foreach (var ZombieAgent in _selected) {
                if (ZombieAgent == null) continue;
                if (!startchunks.Contains(
                    _gridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk))
                {
                    startchunks.Add(_gridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk);
                }
            }

            if (startchunks.Count == 0) {
                return null;
            }

            foreach (var chunk in startchunks) {
                foreach (var chunkpath in _gridManager.GetAStartPath(chunk, targetCell.Chunk)) {
                    if (!pathChunks.Contains(chunkpath)) pathChunks.Add(chunkpath);
                }
            }
            pathChunks.AddRange(_gridManager.GetNeighborsOfPath(pathChunks));
            return pathChunks;
        }
        private List<Chunk> GetChunksPathFromSubGrid( Cell targetCell) {
            List<Chunk> startchunks = new List<Chunk>();
            List<Chunk> pathChunks = new List<Chunk>();
            foreach (var ZombieAgent in _selected) {
                if (ZombieAgent == null) continue;
                if (ZombieAgent.Subgrid == null) {
                    if (!startchunks.Contains(
                        _gridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk)) {
                        startchunks.Add(_gridManager.GetCellFromWorldPos(ZombieAgent.transform.position).Chunk);
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
                return null;
            }

            foreach (var chunk in startchunks) {
                foreach (var chunkpath in _gridManager.GetAStartPath(chunk, targetCell.Chunk)) {
                    if (!pathChunks.Contains(chunkpath)) pathChunks.Add(chunkpath);
                }
            }
            pathChunks.AddRange(_gridManager.GetNeighborsOfPath(pathChunks));
            return pathChunks;
        }
        private void ManageAttackBuilding(House house) {
            Cell centerCell = _gridManager.GetCellFromWorldPos(house.transform.position);
            List<Cell> investigatedCells = _gridManager.GetCellNeighborRange(centerCell, 5);
            List<Cell> homeCells = new List<Cell>();
            
            _gridManager.ColorAllDebugGridToColor(Color.white);
            foreach (var neighbor in investigatedCells) {
                if (neighbor.CheckCellColliderContain(house.gameObject)) {
                    homeCells.Add(neighbor);
                    neighbor.ColorDebugCell(Color.blue);
                    continue;
                }
                neighbor.ColorDebugCell(Color.green);
            }

            
            List<Chunk> totalChunks = GetChunksPathFromZombie(_selected, centerCell);
            if (totalChunks == null) return;
            Subgrid subgrid = new Subgrid();
            subgrid.GenerateSubGrid(totalChunks.ToArray(), _gridManager.Size, _gridManager.Offset);
            subgrid.StartAttackBuilding(homeCells.ToArray());
            
            foreach (var zombieAgent in _selected)
            {
                if (zombieAgent == null) continue;
                zombieAgent.SetNewSubGrid(subgrid);
            }



        }
        #endregion
        private void ManageBorderCameraMovement()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 dir = Vector3.zero;
            if( mousePosition.x<_pixelBorder) dir+= Vector3.left;
            if( mousePosition.x>Screen.width-_pixelBorder) dir+= Vector3.right;
            if( mousePosition.y<_pixelBorder) dir+= Vector3.back;
            if( mousePosition.y>Screen.height-_pixelBorder) dir+= Vector3.forward;
            StaticData.CameraMoveVector = dir;
        }
        private void ManagePressEscape() {
            if (_selected.Count > 0) {
                ClearSelection();
            }
            else {
                Debug.Log("Open Menu Pause 0");
                StaticEvents.SetGameOnPause();
            }
        }
        private void StaticEventsOnOnSubmitSelectionChange(object sender, List<GridAgent> e) {
            SetSelection(e);
        }
        private void AddGridAGentToSelection(object arg, GridAgent gridAgent) {
            if (gridAgent == null) return;
            _selected.Add(gridAgent);
            gridAgent.IsSelected = true;
            StaticEvents.ChangeSelection(_selected);
        }
        private bool CanBeSelectBox(Vector2 a, Vector2 b) {
            if (Mathf.Abs(a.x - b.x) < Metrics.SelectionBoxMinSize) return false;
            if (Mathf.Abs(a.y - b.y) < Metrics.SelectionBoxMinSize) return false;
            return true;
        }
        private void CheatSpawnZombie(){
            RaycastHit hit;
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                Cell cell = _gridManager.GetCellFromWorldPos(hit.point);
                ZombieAgent zombie = Instantiate(_prfZombieAgent, hit.point + new Vector3(0, 0.5f, 0),
                    Quaternion.identity);
                zombie.Generate(_gridManager);
            }
        }
        private void ManageSpawnVFXMoveOrder(Vector3 pos) {
            if (VFXPoolManager.Instance != null) {
                VfxPoolableMono vfx =VFXPoolManager.Instance.GetPooledVFXOfType(_vfxMoveOrderType);
                vfx.transform.position = pos;
            }
            else if (_prefabsVFXMoveOrder) {
                Instantiate(_prefabsVFXMoveOrder, pos, transform.rotation);
            }
        }
    }
    
}