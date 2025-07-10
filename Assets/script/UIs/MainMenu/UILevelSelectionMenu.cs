using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelSelectionMenu : MonoBehaviour
{
    public event EventHandler OnPanelClose;
    
    [SerializeField]  private SOLevelInfoDataArray _levelInfoDataArray;
    [SerializeField] private UIAsyncSceneLoader _asyncSceneLoader;
    [Space(10)]
    [SerializeField] private Transform _transformButtonHolder;
    [SerializeField] private UILevelSelectionButton _prfSelectionButton;
    [SerializeField] private Button _bpReturn;
    [SerializeField] private Button _bpStart;
    [Space(5)]
    [SerializeField] private TMP_Text _txtTitle;
    [SerializeField] private TMP_Text _txtSubTitile;
    [SerializeField] private TMP_Text _txtDescription;
    [SerializeField] private Image _imgLevel;

    private UILevelSelectionButton[] _selectionButtons;
    private SOLevelInfoData _selectedLevelInfoData;

    private void Start() {
        _bpReturn.onClick.AddListener(ClosePanel);
        _bpStart.onClick.AddListener(StartLevel);
        SetUpLevelButton();
        SelectLevel(_levelInfoDataArray.Levels[0]);
        _selectionButtons[0].Select();
    }

    private void SetUpLevelButton() {
        _selectionButtons = new UILevelSelectionButton[_levelInfoDataArray.Levels.Length];
        for (int i = 0; i < _levelInfoDataArray.Levels.Length; i++) {
            if (_levelInfoDataArray.Levels[i] == null) continue;
            _selectionButtons[i] = Instantiate(_prfSelectionButton, _transformButtonHolder);
            _selectionButtons[i].SetUpButton(_levelInfoDataArray.Levels[i]);
            _selectionButtons[i].OnLevelSelect+= OnOnLevelSelect;
        }
    }

    private void OnOnLevelSelect(object sender, SOLevelInfoData e) => SelectLevel(e);
    

    private void StartLevel()
    {
        if (_selectedLevelInfoData == null || _selectedLevelInfoData.SceneName == null) {
            Debug.LogWarning("Pas d'info valide pour charger la scène, manque de SOLevelInfoData ou de nom de scène",
                this);
            return;
        }
        _asyncSceneLoader.StartLoadingScene(_selectedLevelInfoData.SceneName);
    }

    private void ClosePanel() {
        OnPanelClose?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        foreach (var button in _selectionButtons) {
            if (button==null)continue;
            button.OnLevelSelect -= OnOnLevelSelect;
        }
    }

    public void OpenPanel() {
        gameObject.SetActive(true);
    }
    public void SelectLevel(SOLevelInfoData data) {
        if( data==null) {
            Debug.LogWarning("Pas de LevelInfoData Détécter !");
            return;
        }
        
        _selectedLevelInfoData = data;

        _txtTitle.text = _selectedLevelInfoData.MenuName;
        _txtSubTitile.text = _selectedLevelInfoData.Subtile;
        _txtDescription.text = _selectedLevelInfoData.Description;
        _imgLevel.sprite = _selectedLevelInfoData.Illustration;
    }
}