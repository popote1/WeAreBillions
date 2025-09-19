using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ElecticCableController : MonoBehaviour
{
    private Material _material;

    private void Start() {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void SetPowerCableOn(bool value) {
        if( _material==null)return;
        if( value) _material.SetInt("_IsPowered",1);
        else _material.SetInt("_IsPowered",0);
    }
}