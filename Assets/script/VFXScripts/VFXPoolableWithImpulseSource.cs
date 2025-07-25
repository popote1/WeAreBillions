using System;
using Unity.Cinemachine;
using UnityEngine;

public class VFXPoolableWithImpulseSource : VfxPoolableMono
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    private void OnEnable() {
        _impulseSource.GenerateImpulse();
    }
}