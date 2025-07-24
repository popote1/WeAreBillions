using System;
using script;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDRestartPauseMenu : MonoBehaviour
{
    public event Action OnPanelClose;

    [SerializeField]
    private UIAsyncSceneLoader _uiAsyncSceneLoader;
    [SerializeField] private Button _bpConfirm;
    [SerializeField] private Button _bpReturn;
    
    private void Awake() {
        _bpReturn.onClick.AddListener(ClosePanel);
        _bpConfirm.onClick.AddListener(UiRestartLevel);
    }
    
    public void OpenPanel() {
        gameObject.SetActive(true);
    }

    private void UiRestartLevel() {
        StaticEvents.SetPause(false);
        if (_uiAsyncSceneLoader != null) _uiAsyncSceneLoader.StartLoadingScene(SceneManager.GetActiveScene().name);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ClosePanel()
    {
        OnPanelClose?.Invoke();
        gameObject.SetActive(false);
    }
    public void ForceClose()=>gameObject.SetActive(false);
}