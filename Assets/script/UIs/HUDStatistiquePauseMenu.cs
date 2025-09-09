using System;
using script;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDStatistiquePauseMenu : MonoBehaviour
{
    public event Action OnPanelClose;
    [SerializeField] private TMP_Text _txtTitle;
    [SerializeField] private TMP_Text _txtSubTitle;
    [SerializeField] private TMP_Text _txtScore;
    [SerializeField] private TMP_Text _txtZombies;
    [SerializeField] private TMP_Text _txtMaxHordeSize;
    [SerializeField] private TMP_Text _txtCiviliansLeft;
    [SerializeField] private TMP_Text _txtDefendersTransforms;
    [SerializeField] private TMP_Text _txtBuildingDestroy;
    [SerializeField] private TMP_Text _txtRunTime;
    [SerializeField] private Button _bpReturn;

    private void Awake()
    {
        _bpReturn.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel() {
        gameObject.SetActive(true);
        DisplayCurrentStats();
    }

    private void DisplayCurrentStats() {
        if (StaticData.SoLevelInfoDataArray != null) {
            SOLevelInfoData levelData =
                StaticData.SoLevelInfoDataArray.GetLevelInfoDataBySceneName(SceneManager.GetActiveScene().name);
            _txtTitle.text = levelData.MenuName;
            _txtSubTitle.text = levelData.Subtile;
        }

        _txtScore.text = StaticScoringSystem.CurrentScore.ToString();
        _txtZombies.text = StaticData.zombieCount.ToString();
        _txtMaxHordeSize.text = StaticData.zombieMaxCount.ToString();
        _txtCiviliansLeft.text = StaticData.CiviliansCounts.ToString();
        _txtDefendersTransforms.text = StaticData.DefendersKill.ToString();
        _txtBuildingDestroy.text = StaticData.DestroyBuilding.ToString();
        _txtRunTime.text = StaticData.GetGameTime();

    }

    private void ClosePanel() {
        OnPanelClose?.Invoke();
        gameObject.SetActive(false);
    }

    public void ForceClose()=>gameObject.SetActive(false);
    
}