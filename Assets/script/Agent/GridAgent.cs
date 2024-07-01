using System;
using System.Collections.Generic;
using script;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GridAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GridManager GridManager;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Rigidbody Rigidbody;

    [Header("Caracteristics")] 
    [SerializeField] protected int _maxHp;
    [SerializeField] protected Metrics.UniteType _uniteType;
    [SerializeField] private bool _canBeTransform=true;
    [SerializeField] private float _transformationTime = 3;
    [Header("Move Parameters")]
    [SerializeField] protected float _maxMoveSpeed=3;
    [SerializeField] protected float _moveSpeed=10;
    [SerializeField] protected float _speedModulator=0.05f;
    [SerializeField] protected float _stopDrag = 4f;
    [SerializeField] protected float _moveDrag = 0f;
    [Header("Wondering Parameters")] 
    [SerializeField] protected bool _usWondering;
    [SerializeField] protected float _wonderringDelayMin=1;
    [SerializeField] protected float _wonderringDelayMax=10;
    [SerializeField] protected int _wonderringdistance=3;
    [Space(0.5f)]
    public Subgrid Subgrid;
    public GameObject PSEmoteRedSquare;
    [Header("HeightOffSetting")] 
    public float HeightOffSetting;
    public LayerMask GroundLayer;
    [Header("Selection")] 
    public bool IsSelectable;
    public GameObject SelectionSprite;

    public GridActorStat Stat { get; private set; } = GridActorStat.Idle;
    public Metrics.UniteType UniteType { get => _uniteType; }
    
    public bool CanBetransform { get => _canBeTransform; }
    public float TransformTime { get => _transformationTime; }
    
    protected bool _isSelected;
    protected float _wonderingTimer;
    protected float _wonderingDelay;
    protected Cell _wonderingTarget;
    protected Collider _collider;
    protected int _hp;
    

    public enum GridActorStat {
        Idle, Move, Attack , Grabed, Transforming
    }
    
    public bool IsSelected {
        get => _isSelected;
        set {
            if (SelectionSprite) SelectionSprite.SetActive(value);
            _isSelected = value;
        }
    }

    protected virtual void Start() {
        _collider = GetComponent<Collider>();
        if (GridManager == null) GridManager = GridManager.Instance;
        if (GridManager == null) {
            Debug.LogWarning("GridManagerNot Found ", this);
        }
        if( _usWondering)_wonderingDelay = Random.Range(_wonderringDelayMin, _wonderringDelayMax);
    }


    public void Generate(GridManager gridManager) => GridManager = gridManager;
    
    protected virtual void Update() {
        ManageSelfElevation();
        ManageOrientation();
        ManageStat();

        _animator.SetFloat("Velocity", Rigidbody.velocity.magnitude/_maxMoveSpeed);
    }

    protected virtual void ManageStat() {
        switch (Stat) {
            case GridActorStat.Idle: ManageIdle(); break;
            case GridActorStat.Move: ManageMove(); break;
            case GridActorStat.Attack: ManageAttack(); break;
            case GridActorStat.Grabed: ManageGrabbed(); break;
            case GridActorStat.Transforming: ManageTransformation(); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    

    protected virtual void ManageSelfElevation() {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, 4, GroundLayer)) {
            transform.position = new Vector3(transform.position.x, hit.point.y + HeightOffSetting, transform.position.z);
        }
    }

    protected virtual void ManageOrientation()
    {
        if (Rigidbody.velocity.magnitude > 0.5f) {
            Vector3 f = Rigidbody.velocity;
            f.y = 0;
            transform.forward = f;
        }
    }
    protected virtual void FixedUpdate() {
        if (Subgrid!=null&&Stat == GridActorStat.Move) {
            Cell cell =Subgrid.GetCellFromWorldPos(transform.position);
            if (cell == null) {
                Cell currentPos = GridManager.GetCellFromWorldPos(transform.position);
                if (currentPos == null) {
                    PSEmoteRedSquare.SetActive(true);
                    Debug.LogWarning("GridAgent out of the Game Zone", this);
                    return;
                }
                ManagerRecalculationOrExtraPathToSubGrid(currentPos);
                return;
            }

            if (Subgrid.TargetCells[0].Pos==cell.Pos) {
                GetToMoveTarget();
               
                return;
            }
            if( PSEmoteRedSquare)PSEmoteRedSquare.SetActive(false);
            Rigidbody.AddForce(new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y) * GetMoveSpeed());
            Rigidbody.velocity -= Rigidbody.velocity * _speedModulator;
        }
    }

    protected virtual float GetMoveSpeed() {
        return _moveSpeed;
    }

    protected virtual void GetToMoveTarget() {
        Subgrid = Subgrid.NextSubGrid;
        if (Subgrid == null) ChangeStat( GridActorStat.Idle);
        
        if (_usWondering) RestWonderingTimers();
    }
    
    protected virtual void ManagerRecalculationOrExtraPathToSubGrid(Cell currentPos) {
        List<Chunk> path =GridManager.GetAStartPath(currentPos.Chunk,
            GridManager.GetCellFromPos(Subgrid.TargetPos).Chunk);
        foreach (var neighbor in GridManager.GetNeighborsOfPath(path)) {
            if (path.Contains(neighbor)) continue;
            path.Add(neighbor);
        }
        Subgrid.AddChunksToSubGrid(path.ToArray());
    }

    public virtual void SetNewSubGrid(Subgrid subgrid) {
        if (subgrid != null) {
            Subgrid = subgrid;
            ChangeStat(GridActorStat.Move);
        }
    }
    protected virtual void ManageIdle() {
        if (_usWondering)ManageWondering();
    }
    
    protected virtual void ManageMove(){}

    protected virtual void ManageAttack(){}

    protected virtual void ManageGrabbed(){}
    
    protected virtual void ManageTransformation() { }
    
    protected virtual void ChangeStat(GridActorStat stat) {
        if (Stat == GridActorStat.Attack && Subgrid != null && stat == GridActorStat.Idle) {
            ChangeStat(GridActorStat.Move);
            return;
        } 
        Stat = stat;

        if (Stat == GridActorStat.Move) Rigidbody.drag = _moveDrag;
        else Rigidbody.drag = _stopDrag;
    }

    public virtual void SetAsGrabbed()
    {
        _animator.SetBool("IsGrabbed", true);
        ChangeStat(GridActorStat.Grabed);
        _collider.enabled = false;
    }


    #region Wondering Parameters

    protected virtual void RestWonderingTimers() {
        _wonderingDelay = Random.Range(_wonderringDelayMin, _wonderringDelayMax);
        _wonderingTimer = 0;
    }
    protected virtual void ManageWondering() {
        _wonderingTimer += Time.deltaTime;
        if (_wonderingTimer >= _wonderingDelay) {
            SetNewWonderingTarget();
            _wonderingTimer = 0;
        }
    }
    protected virtual void SetNewWonderingTarget() {
        Cell currentcell = GridManager.GetCellFromWorldPos(transform.position);
            
        Cell[] possibleTargets =GridManager.GetBreathFirstCells(currentcell, _wonderringdistance);
        if (possibleTargets == null || possibleTargets.Length == 0) {
            Debug.LogWarning("Agents Don't find wondering target", this);
            return;
        }
        _wonderingTarget = possibleTargets[Random.Range(0, possibleTargets.Length)];
        SetNewMoveDestination(_wonderingTarget);
    }
    protected virtual void SetNewMoveDestination(Cell targetCell) {
        Cell currentcell = GridManager.GetCellFromWorldPos(transform.position);
        _wonderingTarget = targetCell;
        List<Chunk> totalChunks = GridManager.GetAStartPath(currentcell.Chunk , _wonderingTarget.Chunk);
        Subgrid newSub = new Subgrid();
        newSub.GenerateSubGrid(totalChunks.ToArray(), GridManager.Size, GridManager.Offset);
        newSub.StartCalcFlowfield(new[]{_wonderingTarget});
        SetNewSubGrid(newSub);
    }

    #endregion
    
}