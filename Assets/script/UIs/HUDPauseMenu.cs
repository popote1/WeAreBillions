using System;
using System.Collections;
using System.Collections.Generic;
using script;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDPauseMenu : MonoBehaviour
{
    [Header("PauseMenu")]
    [SerializeField] private GameObject _panelPauseMenu;
    [SerializeField] private Button _bpResume;
    [SerializeField] private Button _bpOptions;
    [SerializeField] private Button _bpStatistics;
    [SerializeField] private Button _bpRestart;
    [SerializeField] private Button _bpMaineMenu;
    [Header("Options")] 
    [SerializeField] private GameObject _panelOptions;
    [SerializeField] private Slider _sliderAudioMaster;
    [SerializeField] private Slider _sliderAudioMusic;
    [SerializeField] private Slider _sliderAudioSFX;
    [SerializeField] private Slider _sliderAudioAmbiance;
    [SerializeField] private Button _bpOptionReturn;
    [SerializeField] private AudioMixer _audioMixer;


    private void Start() {
        _bpResume.onClick.AddListener(UIBpResume);
        _bpOptions.onClick.AddListener(UIBpOption);
        _bpStatistics.onClick.AddListener(UIBpStatistics);
        _bpRestart.onClick.AddListener(UIBpRestart);
        _bpMaineMenu.onClick.AddListener(UIBpMainMenu);
        
        _sliderAudioMaster.onValueChanged.AddListener(UISliderAudioMasterChange);
        _sliderAudioMusic.onValueChanged.AddListener(UiSliderAudioMusicChange);
        _sliderAudioSFX.onValueChanged.AddListener(UISliderAudioSFXChange);
        _sliderAudioAmbiance.onValueChanged.AddListener(UISliderAudioAmbianceChange);
        _bpOptionReturn.onClick.AddListener(UIBpOptionReturn);
        
        StaticEvents.OnSetGameOnPause+= StaticDataOnOnSetGameOnPause;
        ClosePauseMenu();
    }

    private void OnDestroy() {
        StaticEvents.OnSetGameOnPause-= StaticDataOnOnSetGameOnPause;
    }

    private void StaticDataOnOnSetGameOnPause(object sender, bool e) {
        if( e)OpenPauseMenu();
        else ClosePauseMenu();
    }

    private void UIBpOptionReturn() {
        _panelOptions.SetActive(false);
        _bpOptions.Select();
    }


    private void UISliderAudioAmbianceChange(float value) {
        StaticData.AudioVolumeAmbiances = value;
        _audioMixer.SetFloat("AmbianceVolume", Mathf.Log10(value) * 20);
    }

    private void UISliderAudioSFXChange(float value) {
        StaticData.AudioVolumeSFX = value;
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    private void UiSliderAudioMusicChange(float value) {
        StaticData.AudioVolumeMusic = value;
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    private void UISliderAudioMasterChange(float value) {
        StaticData.AudioVolumeMaster = value;
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    private void UIBpMainMenu() {
        StaticEvents.SetPause(false);
        SceneManager.LoadScene(0);
    }

    private void UIBpRestart() {
        StaticEvents.SetPause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UIBpStatistics() {
        
    }

    private void UIBpOption() {
        _panelOptions.SetActive(true);
        _sliderAudioMaster.Select();
        _sliderAudioMaster.SetValueWithoutNotify(StaticData.AudioVolumeMaster);
        _sliderAudioMusic.SetValueWithoutNotify(StaticData.AudioVolumeMusic);
        _sliderAudioSFX.SetValueWithoutNotify(StaticData.AudioVolumeSFX);
        _sliderAudioAmbiance.SetValueWithoutNotify(StaticData.AudioVolumeAmbiances);
    }

    private void UIBpResume() {
        StaticEvents.SetGameOnPause(false);
    }

    public void OpenPauseMenu() {
        _panelPauseMenu.SetActive(true);
        _bpResume.Select();
    }

    public void ClosePauseMenu() {
        _panelPauseMenu.SetActive(false);
    }
    
}
