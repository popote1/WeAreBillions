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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Rest")]
    public void Rest()
    {
        transform.localPosition = Vector3.zero;
    }

    [ContextMenu("StartDestruction")]
    public void StartDestruction() {
        Instantiate(_prfVfxDestruction, transform.position, transform.rotation);
        transform.DOShakeRotation(_duration, _strength, _vibrato);
        transform.DOMoveY(transform.position.y - _buildingheight, _duration);
    }
}
