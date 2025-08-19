using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class VFXBuildingDestruction : MonoBehaviour
{
    [SerializeField] private float _duration=3;
    [SerializeField] private float _strength = 10;
    [SerializeField] private int _vibrato = 20;
    [SerializeField] private float _buildingheight =3;
    [SerializeField] private GameObject _prfVfxDestruction;

    [SerializeField] private AnimationCurve _CurveBuildingdestruiction = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [Header("Ruins Parameters")]
    [SerializeField] private GameObject _prfRuines;
    [SerializeField] private float _ruineBaseSpawnYOffSet =-3 ;
    [SerializeField] private float _ruinBaseSpawnSpeed = 2;
    [SerializeField] private float _ruinBaseDelay = 2;

    [Header("SmokeStrips")] 
    [SerializeField] private VFXControllerSmokeStripe _prfVfxController;
    [SerializeField] private float _vfxStripSpawnRange=1;
    [SerializeField] private float _vfxStripSpawnCountMin=1;
    [SerializeField] private float _vfxStripSpawnCountMax=4;
    
    
    private Vector3 _originalPos;
    private Quaternion _originalRot;
    
    [ContextMenu("Rest")]
    public void Rest()
    {
        transform.localPosition = Vector3.zero;
    }

    [ContextMenu("StartDestruction")]
    public void StartDestruction()
    {
        _originalPos = transform.position;
        _originalRot = transform.rotation;
        ManageSpawnVFXBuildingColaps();
        transform.DOShakeRotation(_duration, _strength, _vibrato);
        transform.DOMoveY(transform.position.y - _buildingheight, _duration).SetEase(_CurveBuildingdestruiction);
        SpawnVfXStrips();
        Invoke("SpawnRuin", _ruinBaseDelay);
    }

    private void SpawnRuin() {
        if (_prfRuines!=null) {
            GameObject go = Instantiate(_prfRuines, _originalPos + new Vector3(0, _ruineBaseSpawnYOffSet, 0),
                _originalRot);
            go.transform.DOMove(_originalPos, _ruinBaseSpawnSpeed);
        }
    }

    private void SpawnVfXStrips() {
        for (int i = 0; i < Random.Range(_vfxStripSpawnCountMin, _vfxStripSpawnCountMax); i++) {
            Vector3 pos = new Vector3(Random.Range(-_vfxStripSpawnRange, _vfxStripSpawnRange), 0,
                Random.Range(-_vfxStripSpawnRange, _vfxStripSpawnRange)) + _originalPos;
            ManageSpawnVFXSmokeStrip(pos);
        }
    }
    private void ManageSpawnVFXBuildingColaps() {
        if (VFXPoolManager.Instance != null) {
            VfxPoolableMono vfx =VFXPoolManager.Instance.GetPooledVFXOfType(VFXPoolManager.VFXPooledType.buildingColaps);
            vfx.transform.position = transform.position;
        }
        else if (_prfVfxDestruction) {
            Instantiate(_prfVfxDestruction, transform.position, transform.rotation);
        }
    }
    private void ManageSpawnVFXSmokeStrip(Vector3 pos) {
        if (VFXPoolManager.Instance != null) {
            VfxPoolableMono vfx =VFXPoolManager.Instance.GetPooledVFXOfType(VFXPoolManager.VFXPooledType.SmokeStripe);
            vfx.transform.position = pos;
        }
        else if (_prfVfxDestruction) {
            Instantiate(_prfVfxController, pos, Quaternion.identity);
        }
    }
}
