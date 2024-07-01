using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PanelAudioVolumes : MonoBehaviour
{

    [SerializeField] private Slider _sliderGeneral;
    [SerializeField] private Slider _sliderFBX;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderAmbiance;
    [SerializeField] private AudioMixer _audioMixer;
    
    void Start() {
        _sliderGeneral.onValueChanged.AddListener(ChangeGeneralVolume);
        _sliderFBX.onValueChanged.AddListener(ChangeSFXVolume);
        _sliderMusic.onValueChanged.AddListener(ChangeMusicVolume);
        _sliderAmbiance.onValueChanged.AddListener(ChangeAmbianceVolume);
    }
    
    private void ChangeGeneralVolume(float value) {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
    private void ChangeSFXVolume(float value) {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
    private void ChangeMusicVolume(float value) {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
    private void ChangeAmbianceVolume(float value) {
        _audioMixer.SetFloat("AmbianceVolume", Mathf.Log10(value) * 20);
    }
}
