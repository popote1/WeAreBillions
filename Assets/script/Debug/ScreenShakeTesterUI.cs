using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShakeTesterUI : MonoBehaviour
{
    [SerializeField] private Button _bp1;
    [SerializeField] private Button _bp2;
    [SerializeField] private Button _bp3;
    [SerializeField] private Button _bp4;
    
    [SerializeField] private CinemachineImpulseSource _impulse1;
    [SerializeField] private CinemachineImpulseSource _impulse2;
    [SerializeField] private CinemachineImpulseSource _impulse3;
    [SerializeField] private CinemachineImpulseSource _impulse4;

    private void Start() {
        _bp1.onClick.AddListener(Impulse1);
        _bp2.onClick.AddListener(Impulse2);
        _bp3.onClick.AddListener(Impulse3);
        _bp4.onClick.AddListener(Impulse4);
    }

    private void Impulse1() {
        _impulse1.GenerateImpulse();
    }
    private void Impulse2() {
        _impulse2.GenerateImpulse();
    }
    private void Impulse3() {
        _impulse3.GenerateImpulse();
    }
    private void Impulse4() {
        _impulse4.GenerateImpulse();
    }
}
