using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRunSelectionButton : MonoBehaviour
{

    public event EventHandler<StatRunSave> OnSelect; 
    
    [SerializeField] private TMP_Text _txtScore;
    [SerializeField] private TMP_Text _txtDate;
    [SerializeField] private Button _button;
    
    private StatRunSave _runSave;

    public void Select() => _button.Select();
    public void SetUpButton(StatRunSave data) {
        _runSave = data;
        _txtScore.text = _runSave.Score.ToString();
        _txtDate.text = _runSave.Date.ToString();
    }
    
    private void Start() {
        _button.onClick.AddListener(SelectLevel);
    }

    private void SelectLevel() => OnSelect?.Invoke(this, _runSave);
}