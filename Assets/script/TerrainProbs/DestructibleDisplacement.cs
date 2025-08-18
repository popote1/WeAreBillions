using UnityEngine;

public class DestructibleDisplacement : Destructible
{
    [SerializeField] private Collider _collider;
    [Space(10), Header("Displacement Parameters")]
    [SerializeField] private Vector3 _positionChange;
    [SerializeField] private Vector3 _maxRotationPossible;
    [Space(10), Header("Additional VFX")] 
    [SerializeField] private GameObject[] _prfAdditionnalVFX;

    [SerializeField] private int _additionalVFXMin = 1;
    [SerializeField] private int _additionalVFXMax = 2;
    [SerializeField] private float _additionalVFXRange = 0.5f;
    public override void DestroyDestructible(GridAgent source = null)
    {
        if (prefabsFX) Instantiate(prefabsFX, transform.position, transform.rotation);
        _collider.enabled = false;
        Displace();
        SpawnAdditionalVFX();
        Destroy(gameObject);
    }
    private void Displace() {
        transform.position += _positionChange;
        transform.eulerAngles += new Vector3(
            Random.Range(-_maxRotationPossible.x, _maxRotationPossible.x)
            , Random.Range(-_maxRotationPossible.y, _maxRotationPossible.y)
            , Random.Range(-_maxRotationPossible.z, _maxRotationPossible.z));
    }

    private void SpawnAdditionalVFX() {
        int count = Random.Range(_additionalVFXMin, _additionalVFXMax + 1);
        if (count == 0 || _prfAdditionnalVFX == null || _prfAdditionnalVFX.Length == 0) return;

        for (int i = 0; i < count; i++) {
            Vector2 pos = Random.insideUnitCircle*_additionalVFXRange;
            Instantiate(_prfAdditionnalVFX[Random.Range(0, _prfAdditionnalVFX.Length)],
                transform.position + new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _additionalVFXRange);
    }
    [ContextMenu("SetupRefsTool")]
    private void SetupRefsTool() {
        _collider = GetComponent<Collider>();
    }
}