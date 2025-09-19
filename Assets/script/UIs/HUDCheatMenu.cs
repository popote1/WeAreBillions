using System;
using script;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HUDCheatMenu : MonoBehaviour
{
    [SerializeField] private Transform _panelCheat;
    [SerializeField] private Button _BpCheat;
    [SerializeField] private Toggle _toggleCameraLock;
    [SerializeField] private Toggle _toggleZombieSpawning;
    [SerializeField] private Button _bpWin;
    [SerializeField] private Button _bpLose;
    [SerializeField] private TMP_Text _txtFPSCounter;

    private void Awake() {
        StaticData.OptionsUpdate+= StaticDataOnOptionsUpdate;
        StaticDataOnOptionsUpdate(this, EventArgs.Empty);
        
        _toggleCameraLock.onValueChanged.AddListener(UIChangeCameraLock);
        _toggleZombieSpawning.onValueChanged.AddListener(UIChangeZombieSpawning);
        _bpWin.onClick.AddListener(UIWinGame);
        _bpLose.onClick.AddListener(UILoseGame);
        _BpCheat.onClick.AddListener(UIOpenPanel);
    }

    private void OnDestroy() {
        StaticData.OptionsUpdate -= StaticDataOnOptionsUpdate;
    }

    private void Update() {
        _txtFPSCounter.text = "FPS:"+((int)(1 / Time.unscaledDeltaTime)).ToString();
    }

    private void StaticDataOnOptionsUpdate(object sender, EventArgs e) {
        if (StaticData.GamePlayAllowCheatMenu) {
            _BpCheat.gameObject.SetActive(true);
            _toggleCameraLock.SetIsOnWithoutNotify(StaticData.BlockCameraMovement);
            _toggleZombieSpawning.SetIsOnWithoutNotify(StaticData.CheatEnableZombieSpawning);
        }
        else {
            _BpCheat.gameObject.SetActive(false);
            _panelCheat.gameObject.SetActive(false);
            StaticData.CheatEnableZombieSpawning = false;
            StaticData.BlockCameraMovement = false;
        }
    }

    private void UIChangeCameraLock(bool value) => StaticData.BlockCameraMovement = value;
    private void UIChangeZombieSpawning(bool value) => StaticData.CheatEnableZombieSpawning = value;
    private void UIWinGame() => StaticEvents.EndGame(true);
    private void UILoseGame() => StaticEvents.EndGame(false);
    private void UIOpenPanel() => _panelCheat.gameObject.SetActive(!_panelCheat.gameObject.activeSelf);
    
}
