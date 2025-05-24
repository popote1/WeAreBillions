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
    public string AgentName;
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
    public float MaxMoveSpeed => _maxMoveSpeed;
    public float HP => _maxHp;

    protected bool _isSelected;
    protected float _wonderingTimer;
    protected float _wonderingDelay;
    protected Cell _wonderingTarget;
    protected Collider _collider;
    protected int _hp;
    

    public enum GridActorStat {
        Idle, Move, Attack , Grabed, Transforming, CallingAlert
    }
    public bool IsSelected {
        get => _isSelected;
        set {
            if (SelectionSprite) SelectionSprite.SetActive(value);
            _isSelected = value;
        }
    }
    public void Generate(GridManager gridManager) => GridManager = gridManager;

    public virtual void KillAgent() {
        Destroy(gameObject);
    }
    protected virtual void Start() {
        _collider = GetComponent<Collider>();
        if (GridManager == null) GridManager = GridManager.Instance;
        if (GridManager == null) {
            Debug.LogWarning("GridManagerNot Found ", this);
        }
        if( _usWondering)_wonderingDelay = Random.Range(_wonderringDelayMin, _wonderringDelayMax);
        StaticData.AddGridAgent(this);
    }
    protected virtual void OnDestroy() {
        StaticData.RemoveGridAgent(this);
    }
    protected virtual void Update() {
        ManageSelfElevation();
        ManageOrientation();
        ManageStat();
        ManageVelocityToAnimator();
        
    }

    protected virtual void ManageVelocityToAnimator() {
        _animator.SetFloat("Velocity", Rigidbody.linearVelocity.magnitude/_maxMoveSpeed);
    }
    
    public virtual void TakeDamage(int damage) {
        _hp -= damage;
        if (_hp <= 0) {
            KillAgent();
        }
        _animator.SetBool("TakeDamage", true);
    }
    protected virtual void ManageStat() {
        switch (Stat) {
            case GridActorStat.Idle: ManageIdle(); break;
            case GridActorStat.Move: ManageMove(); break;
            case GridActorStat.Attack: ManageAttack(); break;
            case GridActorStat.Grabed: ManageGrabbed(); break;
            case GridActorStat.Transforming: ManageTransformation(); break;
            case GridActorStat.CallingAlert: ManageAlertCalling(); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    protected virtual void ManageSelfElevation() {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position+Vector3.up*3, Vector3.down), out hit, 6, GroundLayer)) {
            transform.position = new Vector3(transform.position.x, hit.point.y + HeightOffSetting, transform.position.z);
        }
    }
    protected virtual void ManageOrientation()
    {
        if (Rigidbody.linearVelocity.magnitude > 0.5f) {
            Vector3 f = Rigidbody.linearVelocity;
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
            Rigidbody.linearVelocity -= Rigidbody.linearVelocity * _speedModulator;
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
    protected virtual void ManageAlertCalling(){}
    protected virtual void ChangeStat(GridActorStat stat) {
        if ((Stat == GridActorStat.Attack||Stat == GridActorStat.CallingAlert) && Subgrid != null && stat == GridActorStat.Idle) {
            ChangeStat(GridActorStat.Move);
            return;
        } 
        Stat = stat;

        if (Stat == GridActorStat.Move) Rigidbody.linearDamping = _moveDrag;
        else Rigidbody.linearDamping = _stopDrag;
    }
    public virtual void SetAsGrabbed(bool grabed = true) {
        if (grabed) ChangeStat(GridActorStat.Grabed);
        else ChangeStat(GridActorStat.Idle);
        _animator.SetBool("IsGrabbed", grabed);
        _collider.enabled = !grabed;
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