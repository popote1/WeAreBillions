using System;
using script;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuManager : MonoBehaviour
{

    [SerializeField] private SOLevelInfoDataArray _soLevelInfoDataArray;
    [Space(10)]
    [SerializeField] private Button _bpLevel;
    [SerializeField] private Button _bpScoring;
    [SerializeField] private Button _bpOption;
    [SerializeField] private Button _bpCredit;
    [SerializeField] private Button _bpQuite;

    [Space(10)]
    [SerializeField] private UILevelSelectionMenu _levelSelection;
    [SerializeField] private UIScoringPanel _scoringPanel;
    [SerializeField] private UIOptionMenu _optionPanel;
    [SerializeField] private UIQuitPanel _quitPanel;
    
    
    private  void Start()
    {
        StaticData.SetSoLevelInfoDataArray(_soLevelInfoDataArray);
        StaticSaveSystem.SetupCurrentSave();
        _bpLevel.onClick.AddListener(UIButtonLevel);
        _bpScoring.onClick.AddListener(UIButtonScoring);
        _bpOption.onClick.AddListener(UIButtonOption);
        _bpCredit.onClick.AddListener(UIButtonCredit);
        _bpQuite.onClick.AddListener(UIButtonQuite);
        
        _scoringPanel.OnPanelClose+= ScoringPanelOnOnPanelClose; 
        _optionPanel.OnPanelClose+= OptionPanelOnOnPanelClose;
        _levelSelection.OnPanelClose += LevelSelectionPanelClose;
        _quitPanel.OnPanelClose+= QuitPanelOnOnPanelClose;
    }

    


    private void LevelSelectionPanelClose(object sender, EventArgs e) {
        gameObject.SetActive(true);
        _bpLevel.Select();
    }
    private void ScoringPanelOnOnPanelClose(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _bpScoring.Select();
    }
    private void OptionPanelOnOnPanelClose(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _bpOption.Select();
    }
    private void QuitPanelOnOnPanelClose(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _bpQuite.Select();
    }

    

    private void UIButtonLevel() {
        gameObject.SetActive(false);
        _levelSelection.OpenPanel();
    }
    private void UIButtonScoring()
    {
        _scoringPanel.Open();
        gameObject.SetActive(false);
    }
    private void UIButtonOption() {
        _optionPanel.OpenOptionPanel();
        gameObject.SetActive(false);
    }

    
    private void UIButtonCredit() {
        
    }
    private void UIButtonQuite() {
        _quitPanel.Open();
    }
    
}