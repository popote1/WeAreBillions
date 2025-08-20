using UnityEngine;

public class Sniper : Defender
{
    [Header("Sniper Ref")] 
    [SerializeField] private LineRenderer _lineRendererAim;
    [Header("Sound")]
    public AudioSource AudioSource;
    public AudioClip[] ShotingSound;

    
    private float _timer;
    protected virtual void Update() {
        _animator.SetBool("HaveTarget", _target!= null);
        ManageAimLineRenderer();
        base.Update();
    }
    protected override void ManageStat() {
        if (Stat == GridActorStat.CallingAlert) ManageAlertCalling();
        base.ManageStat();
        if (Stat == GridActorStat.Idle || Stat == GridActorStat.Move) {
            ManageLookingForTarget();
        }
    }
    protected override void ManageAttack() {
        if (_target == null) ManageLookingForTarget();
        if (_target == null) {
            ChangeStat(GridActorStat.Idle);
            return;
        }
        ManagerFire();
    }
    protected void ManagerFire() {
        if (_attackRange < Vector3.Distance(transform.position, _target.transform.position)) {
            _target = null;
            return;
        }

        transform.forward = _target.transform.position - transform.position;
        _timer += Time.deltaTime;
        if (_timer >= _attack.AttackSpeed) {
            _target.TakeDamage(_attack.GetDamage(_target.UniteType));
            AlertSystemManager.Instance?.FireShot();
            _pSAttack.Play();
            //AudioSource.clip = ShotingSound[Random.Range(0, ShotingSound.Length)];
            //AudioSource.Play();
            _timer = 0;
        }
    }
    protected virtual void ManageLookingForTarget() {
        _triggerZoneDetector.CheckOfNull();
            
        if (_triggerZoneDetector.Zombis.Count <= 0) return;
        if (CheckForAlertCalling()) {
            StartAlertCalling();
            
            return;
        }
        _target =GetTheClosest(_triggerZoneDetector.Zombis);
        ChangeStat(GridActorStat.Attack);

    }

    private void ManageAimLineRenderer() {
        _lineRendererAim.enabled = _target != null;
        if (_target != null) {
            _lineRendererAim.positionCount = 2;
            _lineRendererAim.SetPosition(0, _pSAttack.transform.position);
            _lineRendererAim.SetPosition(1, _target.transform.position);
        }
    }
}