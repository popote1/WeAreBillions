using System;
using script;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIOptionMenu : MonoBehaviour
{
    public event EventHandler OnPanelClose;
    
    [SerializeField] private AudioMixer _audioMixer;
    [Space(10)]
    [SerializeField] private Button _bpGame;
    [SerializeField] private Button _bpAudio;
    [SerializeField] private Button _bpReturn;
    [Space(10)]
    [SerializeField] private Transform _panelGame;
    [SerializeField] private Slider _sliderCameraKeybordSpeed;
    [SerializeField] private Slider _sliderCameraPanningSpeed;
    [SerializeField] private Toggle _toggleAllowCheatMenu;
    [Space(10)] 
    [SerializeField] private Transform _panelAudio;
    [SerializeField] private Slider _sliderAudioMaster;
    [SerializeField] private Slider _sliderAudioAmbiance;
    [SerializeField] private Slider _sliderAudioMusic;
    [SerializeField] private Slider _sliderAudioSFX;
    
    private OptionPanelType _activePanel;
    private enum OptionPanelType {
        Game, Audio
    }
    
    public void OpenOptionPanel() {
        gameObject.SetActive(true);
        LoadCurrentOptions();
        ChangePanelType(OptionPanelType.Game);
        
    }

    private void LoadCurrentOptions() { 
        StaticSaveSystem.ApplyCurrentOptionSaves();
        _sliderAudioAmbiance.SetValueWithoutNotify(StaticData.AudioVolumeAmbiances);
        _sliderAudioMaster.SetValueWithoutNotify(StaticData.AudioVolumeMaster);
        _sliderAudioMusic.SetValueWithoutNotify(StaticData.AudioVolumeMusic);
        _sliderAudioSFX.SetValueWithoutNotify(StaticData.AudioVolumeSFX);
        
        _sliderCameraKeybordSpeed.SetValueWithoutNotify(StaticData.ControlCameraKeyboardSpeed);
        _sliderCameraPanningSpeed.SetValueWithoutNotify(StaticData.ControlCameraPanningSpeed);
        _toggleAllowCheatMenu.SetIsOnWithoutNotify(StaticData.GamePlayAllowCheatMenu);
    }
    
    
    private void ChangePanelType(OptionPanelType type)
    {
        _activePanel = type;
        _panelAudio.gameObject.SetActive(false);
        _panelGame.gameObject.SetActive(false);
        switch (_activePanel)
        {
            case OptionPanelType.Game:
                _panelGame.gameObject.SetActive(true);
                _sliderCameraKeybordSpeed.Select();
                break;
            case OptionPanelType.Audio:
                _panelAudio.gameObject.SetActive(true);
                _sliderAudioMaster.Select();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Awake() {
        _sliderCameraKeybordSpeed.onValueChanged.AddListener(UISetCameraKeyboardSpeed);
        _sliderCameraPanningSpeed.onValueChanged.AddListener(UISetCameraPanningSpeed);
        _toggleAllowCheatMenu.onValueChanged.AddListener(UISetAllowCheatMenu);
        
        _sliderAudioMaster.onValueChanged.AddListener(UISliderAudioMasterChange);
        _sliderAudioAmbiance.onValueChanged.AddListener(UISliderAudioAmbianceChange);
        _sliderAudioMusic.onValueChanged.AddListener(UiSliderAudioMusicChange);
        _sliderAudioSFX.onValueChanged.AddListener(UISliderAudioSFXChange);
        
        _bpGame.onClick.AddListener(UIOpenOptionGame);
        _bpAudio.onClick.AddListener(UIOpenOptionAudio);
        _bpReturn.onClick.AddListener(UICloseOption);
    }

    private void UIOpenOptionGame() => ChangePanelType(OptionPanelType.Game);
    private void UIOpenOptionAudio() => ChangePanelType(OptionPanelType.Audio);

    private void UICloseOption() {
        OnPanelClose?.Invoke(this, EventArgs.Empty);
        StaticSaveSystem.SaveNewOptionsData();
        gameObject.SetActive(false);
    }
    private void UISetCameraKeyboardSpeed(float value) => StaticData.ControlCameraKeyboardSpeed = value;
    private void UISetCameraPanningSpeed(float value) => StaticData.ControlCameraPanningSpeed = value;
    private void UISetAllowCheatMenu(bool value) => StaticData.GamePlayAllowCheatMenu = value;

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
}