using script;
using UnityEngine;

public class Tank : Defender
{

    [SerializeField] protected float _attackExplosionRadius =1;
    [Range(0, 1)] [SerializeField] protected float _attackSplashDamage =0.5f; 

    [Space(10)]
    [Header("MainBody")]
    [SerializeField] private float _orientationSpeed =1;
    [SerializeField] private Transform _raycaterA;
    [SerializeField] private Transform _raycaterB;
    [SerializeField] private Transform _raycaterC;
    [SerializeField] private Transform _raycaterD;
    [SerializeField] private float _yOffset =0.5f;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _turretHead;
    [SerializeField] private float _turretRotationSpeed =50f;
    [SerializeField] private float _turretDotProductFireAngle = 0.9f;
    protected override void Update() {
        base.Update();
        ManageBodyOrientation();
    }

    protected override void ManageVelocityToAnimator() {}

    protected override void ManageOrientation() {
        if (Subgrid != null && Stat == GridActorStat.Move) {
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

            Vector3 targetDir =new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y);
            Vector3 dir = transform.forward;
            dir.y = 0;
            Vector3 right = transform.right;
            right.y = 0;
            
            float mod  = Vector3.Dot(right.normalized, targetDir);
            mod = mod / Mathf.Abs(mod);
            float angle = Vector3.Angle(targetDir, dir.normalized);
            float angle2 = Mathf.Clamp(angle, -_orientationSpeed* Time.deltaTime, _orientationSpeed * Time.deltaTime);
            
            
            transform.Rotate(Vector3.up, angle2*mod);
            Vector3 rot = transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            transform.eulerAngles = rot;


        }

        //if (Rigidbody.linearVelocity.magnitude > 0.5f) {
        //    Vector3 f = Rigidbody.linearVelocity;
        //    f.y = 0;
        //    transform.forward = f;
        //}
    }
    protected override void FixedUpdate() {
        ManageMovement();
    }

    protected void ManageMovement() {
        if (Subgrid != null && Stat == GridActorStat.Move)
        {
            Cell cell = Subgrid.GetCellFromWorldPos(transform.position);
            if (cell == null)
            {
                Cell currentPos = GridManager.GetCellFromWorldPos(transform.position);
                if (currentPos == null)
                {
                    PSEmoteRedSquare.SetActive(true);
                    Debug.LogWarning("GridAgent out of the Game Zone", this);
                    return;
                }

                ManagerRecalculationOrExtraPathToSubGrid(currentPos);
                return;
            }

            if (Subgrid.TargetCells[0].Pos == cell.Pos)
            {
                GetToMoveTarget();
                return;
            }

            if (PSEmoteRedSquare) PSEmoteRedSquare.SetActive(false);
            Vector3 targetDir = new Vector3(cell.DirectionTarget.x, 0, cell.DirectionTarget.y);
            float dot = Vector3.Dot(targetDir, transform.forward);

            float mod = 0;
            if (dot > 0.5)
            {
                mod = (dot - 0.5f) * 2f;
            }

            Rigidbody.AddForce(transform.forward * GetMoveSpeed() * mod);
            Rigidbody.linearVelocity -= Rigidbody.linearVelocity * _speedModulator;
        }
    }

    protected void ManageBodyOrientation() {
        Vector3 a, b, c, d, ab, bc, cd, da, center, forward, right, top ;
        a= b= c= d=Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(_raycaterA.position, Vector3.down, out hit, Mathf.Infinity,GroundLayer))  a = hit.point;
        if (Physics.Raycast(_raycaterB.position, Vector3.down, out hit, Mathf.Infinity, GroundLayer)) b = hit.point;
        if (Physics.Raycast(_raycaterC.position, Vector3.down, out hit, Mathf.Infinity, GroundLayer)) c = hit.point;
        if (Physics.Raycast(_raycaterD.position, Vector3.down, out hit, Mathf.Infinity, GroundLayer)) d = hit.point;
        
        ab = Vector3.Lerp(a, b, 0.5f);
        bc = Vector3.Lerp(b, c, 0.5f);
        cd = Vector3.Lerp(c, d, 0.5f);
        da = Vector3.Lerp(d, a, 0.5f);
        center = (a + b + c + d) / 4;

        right = bc-da;
        forward = ab - cd;
        top = Vector3.Cross(forward, right);
        
        center.y +=_yOffset;
        _body.position = center;
        _body.rotation = Quaternion.LookRotation(forward.normalized, top.normalized);;
    }
    
    private void ManageLookingForTarget() {
        
        if (_triggerZoneDetector.Zombis.Count <= 0) return;
        _target =_triggerZoneDetector.GetTheClosest();
        ChangeStat(GridActorStat.Attack);

    }

    protected override void ManageIdle() {
        base.ManageIdle();
        ManageLookingForTarget();
    }

    protected override void ManageMove() {
        base.ManageMove();
        ManageLookingForTarget();
    }

    public override void SetAsGrabbed(bool grabed = true) {
    }

    protected override void ManageAttack() {
        if (!IsCurrentTargetValid()) ManageLookingForTarget();
        if (_target == null) {
            ChangeStat(GridActorStat.Idle);
            return;
        }
        ManageTurretOrientation();
        if (_attackTimer != 0) {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer < 0) _attackTimer = 0;
        }
        if (CanFire()) ManagerFire();    
        
    }

    protected void ManageTurretOrientation() {
        Vector3 targetDir = _target.transform.position - _turretHead.transform.position;
        Vector3 dir = _turretHead.transform.forward;
        targetDir.y = 0;
        dir.y = 0;
        Vector3 right = _turretHead.transform.right;
        right.y = 0;
            
        float mod  = Vector3.Dot(right.normalized, targetDir);
        mod = mod / Mathf.Abs(mod);
        float angle = Vector3.Angle(targetDir, dir.normalized);
        float angle2 = Mathf.Clamp(angle, -_turretRotationSpeed* Time.deltaTime, _turretRotationSpeed * Time.deltaTime);
        _turretHead.transform.Rotate(Vector3.up, angle2*mod);
    }

    protected virtual bool CanFire()
    {
        if (_attackTimer > 0) return false;
        if (Vector3.Distance(_target.transform.position, transform.position) > _attackRange) return false;
        Vector3 targetDir = _target.transform.position - _turretHead.transform.position;
        Vector3 dir = _turretHead.transform.forward;
        targetDir.y = 0;
        dir.y = 0;
        if (Vector3.Dot(targetDir.normalized, dir.normalized) < _turretDotProductFireAngle) return false;
        return true;
    }

    protected virtual bool IsCurrentTargetValid() {
        if (_target == null) return false;
        if (Vector3.Distance(_target.transform.position, transform.position) > _attackRange) return false;
        return true;
    }
    protected void ManagerFire() {
        _pSAttack.Play();
        _attackTimer = _attack.AttackSpeed;

        Collider[] colls =Physics.OverlapSphere(_target.transform.position, _attackExplosionRadius);
        foreach (var col in colls) {
            if (col.GetComponent<IDestructible>() != null) {
                col.GetComponent<IDestructible>().TakeDamage(_attack.GetDamage(Metrics.UniteType.Heavy));
                continue;
            }

            if (col.GetComponent<GridAgent>() != null) {
                GridAgent agent = col.GetComponent<GridAgent>();
                if (agent == _target) {
                    agent.TakeDamage(_attack.GetDamage(agent.UniteType));
                    continue;
                }
                agent.TakeDamage(Mathf.RoundToInt(_attack.GetDamage(agent.UniteType)*_attackSplashDamage));
            }
        }
        
        //_target.KillAgent();

    }
}
