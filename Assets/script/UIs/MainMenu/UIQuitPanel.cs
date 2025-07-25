using System;
using UnityEngine;
using UnityEngine.UI;

public class UIQuitPanel:MonoBehaviour
{
    public event EventHandler OnPanelClose;
    
    [SerializeField] private Button _bpQuiteGame;
    [SerializeField] private Button _bpRetourn;

    public void Open() {
        gameObject.SetActive(true);
        _bpRetourn.Select();
    }

    private void Start()
    {
        _bpQuiteGame.onClick.AddListener(UIButtonQuiteGame);
        _bpRetourn.onClick.AddListener(UIReturn);
    }

    private void UIButtonQuiteGame() {
        Application.Quit();
    }

    private void UIReturn() {
        gameObject.SetActive(false);
        OnPanelClose?.Invoke(this , EventArgs.Empty);
    }
    
    
}