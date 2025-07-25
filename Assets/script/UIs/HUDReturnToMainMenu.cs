using System;
using script;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDReturnToMainMenu : MonoBehaviour
{
    public event Action OnPanelClose;
    [SerializeField] private UIAsyncSceneLoader _uiAsyncSceneLoader;
    [SerializeField] private Button _bpConfirm;
    [SerializeField] private Button _bpReturn;


    private void Awake() {
        _bpReturn.onClick.AddListener(ClosePanel);
        _bpConfirm.onClick.AddListener(UIReturnToMainMenu);
    }

    private void UIReturnToMainMenu() {
        StaticEvents.SetPause(false);
        if (_uiAsyncSceneLoader != null) _uiAsyncSceneLoader.StartLoadingScene("MainMenu");
        else SceneManager.LoadScene("0");
    }

    public void OpenPanel() {
        gameObject.SetActive(true);
    }

    private void ClosePanel()
    {
        OnPanelClose?.Invoke();
        gameObject.SetActive(false);
    }
    public void ForceClose()=>gameObject.SetActive(false);
}