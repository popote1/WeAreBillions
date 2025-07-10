using System;
using script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoringPanel : MonoBehaviour
{

    public event EventHandler OnPanelClose;
    
    [SerializeField] private SOLevelInfoDataArray _levelInfoDataArray;
    [SerializeField] private Transform _transformButtonLevelHolder;
    [SerializeField] private UILevelSelectionButton _prfSelectionButtonLevel;
    [SerializeField] private Button _bpReturn;
    [Space(5)]
    [SerializeField] private TMP_Text _txtTitle;
    [SerializeField] private TMP_Text _txtSubTitile;
    [SerializeField] private TMP_Text _txtScore;
    [SerializeField] private TMP_Text _txtZombie;
    [SerializeField] private TMP_Text _txtSHordeMaxSize;
    [SerializeField] private TMP_Text _txtCiviliansAlive;
    [SerializeField] private TMP_Text _txtDefenderTransform;
    [SerializeField] private TMP_Text _txtBuildingDestroy;
    [SerializeField] private TMP_Text _txtRunTime;
    [Space(5)]
    [SerializeField] private Transform _transformButtonRunHolder;
    [SerializeField] private UIRunSelectionButton _prfRunSelectionButton;
    [SerializeField] private UIScoringDisplayPanel _scoringDisplayPanel;

    private LevelSaveData _currentSave;
    private SOLevelInfoData _currentLevel;
    private UILevelSelectionButton[] _levelSelectionButtons;
    private UIRunSelectionButton[] _runSelectionButtons;
    
    
    private void ButtonOnOnLevelSelect(object sender, SOLevelInfoData level) => ChangeLevelInfoData(level);
    private void ButtonOnSelectRun(object sender, StatRunSave runsave) =>_scoringDisplayPanel.DisplayPanel(_currentLevel, runsave);
    
    public void Open() {
        gameObject.SetActive(true);
    }

    private void Start() {
        SetUpScoringPanel();
        _bpReturn.onClick.AddListener(ClosePanel);
    }

    private void SetUpScoringPanel() {
        for (int i = 0; i < _levelInfoDataArray.Levels.Length; i++)
        {
            if (_levelInfoDataArray.Levels[i] == null) continue;
            UILevelSelectionButton button = Instantiate(_prfSelectionButtonLevel, _transformButtonLevelHolder);
            button.SetUpButton(_levelInfoDataArray.Levels[i]);
            button.OnLevelSelect+= ButtonOnOnLevelSelect;
        }
    }

    private void ClosePanel() {
        gameObject.SetActive(false);
        OnPanelClose?.Invoke(this, EventArgs.Empty);
    }

    
    private void ChangeLevelInfoData(SOLevelInfoData level) {
        _currentLevel = level;
        _currentSave = StaticSaveSystem._currentSave.GetLevelSAveDataBySceneName(level.SceneName);
        DisplayBestScore();
        SetUpBestSavesButtons();
    }

    private void SetUpBestSavesButtons() {
        if (_runSelectionButtons != null) {
            foreach (var button in _runSelectionButtons) {
                Destroy(button.gameObject);
            }
        }
        _runSelectionButtons = new UIRunSelectionButton[_currentSave.BestRun.Length];
        for (int i = 0; i < _currentSave.BestRun.Length; i++) {
            _runSelectionButtons[i] = Instantiate(_prfRunSelectionButton, _transformButtonRunHolder);
            _runSelectionButtons[i].SetUpButton(_currentSave.BestRun[i]);
            _runSelectionButtons[i].OnSelect+= ButtonOnSelectRun;
        }
    }
    
    private void DisplayBestScore() {
        _txtTitle.text = _currentLevel.MenuName;
        _txtSubTitile.text = _currentLevel.Subtile;
        _txtScore.text = _currentSave.BestStats.Score.ToString();
        _txtZombie.text = _currentSave.BestStats.zombieCount.ToString();
        _txtSHordeMaxSize.text = _currentSave.BestStats.HordeMaxSize.ToString();
        _txtCiviliansAlive.text = _currentSave.BestStats.CivilliansAlive.ToString();
        _txtDefenderTransform.text = _currentSave.BestStats.DefenderTrensform.ToString();
        _txtBuildingDestroy.text = _currentSave.BestStats.BuildingDestroy.ToString();
        _txtRunTime.text = _currentSave.BestStats.Runtime.ToString();
    }
}