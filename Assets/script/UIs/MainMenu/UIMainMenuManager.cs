using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuManager : MonoBehaviour {
    [SerializeField] private Button _bpLevel;
    [SerializeField] private Button _bpOption;
    [SerializeField] private Button _bpCredit;
    [SerializeField] private Button _bpQuite;

    [Space(10)] 
    [SerializeField] private UIOptionMenu _optionPanel;
    
    private  void Start() {
        _bpLevel.onClick.AddListener(UIButtonLevel);
        _bpOption.onClick.AddListener(UIButtonOption);
        _bpCredit.onClick.AddListener(UIButtonCredit);
        _bpQuite.onClick.AddListener(UIButtonQuite);
        
        _optionPanel.OnPanelClose+= OptionPanelOnOnPanelClose;
    }

    private void OptionPanelOnOnPanelClose(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _bpOption.Select();
    }

    private void UIButtonLevel() {
        
    }
    private void UIButtonOption() {
        _optionPanel.OpenOptionPanel();
        gameObject.SetActive(false);
    }
    private void UIButtonCredit() {
        
    }
    private void UIButtonQuite() {
        Application.Quit();
    }
    
}