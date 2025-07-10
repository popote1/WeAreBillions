using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelSelectionButton : MonoBehaviour
{
    public event EventHandler<SOLevelInfoData> OnLevelSelect;
    
    [SerializeField] private TMP_Text _txtLabel;
    [SerializeField] private Button _button;
    
    private SOLevelInfoData _levelData;
    
    public void Select() => _button.Select();
    private void SelectLevel() => OnLevelSelect?.Invoke(this, _levelData);
    public void SetUpButton(SOLevelInfoData data) {
        
        _levelData = data;
        _txtLabel.text = _levelData.MenuName;
    }
    
    private void Start()
    {
        _button.onClick.AddListener(SelectLevel);
    }

    
    
}