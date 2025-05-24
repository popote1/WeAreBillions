using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class VFXBloodSpalterController : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 10;
    [SerializeField] private AnimationCurve _animationCurveAlphaCliping;
    [SerializeField] private DecalProjector _decalProjector;

    private Material _material;
    private float _timer;

    private void Awake() {
        _material = _decalProjector.material;
        _material = Instantiate(_material);
        _decalProjector.material = _material;
    }

    void Start() {
        InitiateDecal();
    }

    private void InitiateDecal() {
        _material.SetInt("_Tile", Random.Range(0,5));
        
        
        _timer = 0;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _material.SetFloat("_AlphaCliping", _animationCurveAlphaCliping.Evaluate(_timer/_lifeTime));
        if (_timer >= _lifeTime) {
            gameObject.SetActive(false);
        }
    }
    [ContextMenu("Test")]
    private void Test()
    {
        InitiateDecal();
        gameObject.SetActive(true);
    }
}
