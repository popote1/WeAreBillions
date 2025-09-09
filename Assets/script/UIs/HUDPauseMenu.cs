using System;
using script;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDPauseMenu : MonoBehaviour
{
    public event Action OnPanelClose;
    
    [Header("PauseMenu")]
    [SerializeField] private UIOptionMenu _uiOptionPanel;
    [SerializeField] private HUDRestartPauseMenu _hudRestartPauseMenu;
    [SerializeField] private HUDReturnToMainMenu _hudReturnToMainMenu;
    [SerializeField] private HUDStatistiquePauseMenu _hudStatistiquePauseMenu;
    [Space(10)]
    [SerializeField] private Button _bpResume;
    [SerializeField] private Button _bpOptions;
    [SerializeField] private Button _bpStatistics;
    [SerializeField] private Button _bpRestart;
    [SerializeField] private Button _bpMainMenu;
    


    private void Start() {
        _bpResume.onClick.AddListener(UIBpResume);
        _bpOptions.onClick.AddListener(UIBpOption);
        _bpStatistics.onClick.AddListener(UIBpStatistics);
        _bpRestart.onClick.AddListener(UIBpRestart);
        _bpMainMenu.onClick.AddListener(UIBpMainMenu);
       
        _uiOptionPanel.OnPanelClose+= UiOptionPanelOnOnPanelClose;
        _hudRestartPauseMenu.OnPanelClose+= HudRestartPauseMenuOnOnPanelClose;
        _hudReturnToMainMenu.OnPanelClose+= HudReturnToMainMenuOnOnPanelClose;
        _hudStatistiquePauseMenu.OnPanelClose+= HudStatistiquePauseMenuOnOnPanelClose;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        _bpResume.Select();
    }

    public void ForceClose() {
        _uiOptionPanel.ForceClose();
        _hudRestartPauseMenu.ForceClose();
        _hudReturnToMainMenu.ForceClose();
        _hudStatistiquePauseMenu.ForceClose();
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        _uiOptionPanel.OnPanelClose-= UiOptionPanelOnOnPanelClose;
        _hudRestartPauseMenu.OnPanelClose-= HudRestartPauseMenuOnOnPanelClose;
        _hudReturnToMainMenu.OnPanelClose-= HudReturnToMainMenuOnOnPanelClose;
        _hudStatistiquePauseMenu.OnPanelClose-= HudStatistiquePauseMenuOnOnPanelClose;
    }
    private void HudStatistiquePauseMenuOnOnPanelClose() {
        gameObject.SetActive(true);
        _bpStatistics.Select();
    }

    private void HudReturnToMainMenuOnOnPanelClose() {
        gameObject.SetActive(true);
        _bpMainMenu.Select();
    }

    private void HudRestartPauseMenuOnOnPanelClose() {
        gameObject.SetActive(true);
        _bpRestart.Select();
    }

    private void UiOptionPanelOnOnPanelClose(object sender, EventArgs e) {
        gameObject.SetActive(true);
        _bpOptions.Select();
    }
  
    private void UIBpMainMenu() {
        _hudReturnToMainMenu.OpenPanel();
        gameObject.SetActive(false);
    }

    private void UIBpRestart() {
        _hudRestartPauseMenu.OpenPanel();
        gameObject.SetActive(false);
    }

    private void UIBpStatistics() {
        _hudStatistiquePauseMenu.OpenPanel();
        gameObject.SetActive(false);
    }

    private void UIBpOption() {
        _uiOptionPanel.OpenOptionPanel();
        gameObject.SetActive(false);
    }

    private void UIBpResume() {
        OnPanelClose?.Invoke();
        gameObject.SetActive(false);
    }
    
}