using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoringDisplayPanel:MonoBehaviour
{
    [SerializeField] private TMP_Text _txtTitle;
    [SerializeField] private TMP_Text _txtSubTitile;
    [SerializeField] private TMP_Text _txtScore;
    [SerializeField] private TMP_Text _txtDate;
    [SerializeField] private TMP_Text _txtZombie;
    [SerializeField] private TMP_Text _txtSHordeMaxSize;
    [SerializeField] private TMP_Text _txtCiviliansAlive;
    [SerializeField] private TMP_Text _txtDefenderTransform;
    [SerializeField] private TMP_Text _txtBuildingDestroy;
    [SerializeField] private TMP_Text _txtRunTime;
    [SerializeField] private Button _return;

    private void Start() {
        _return.onClick.AddListener(delegate{gameObject.SetActive(false);});
    }
    

    public void DisplayPanel(SOLevelInfoData info, StatRunSave save) {
        _txtTitle.text = info.MenuName;
        _txtSubTitile.text = info.Subtile;
        _txtScore.text = save.Score.ToString();
        _txtDate.text = save.Date.ToString();
        _txtZombie.text = save.zombieCount.ToString();
        _txtSHordeMaxSize.text = save.HordeMaxSize.ToString();
        _txtCiviliansAlive.text = save.CivilliansAlive.ToString();
        _txtDefenderTransform.text = save.DefenderTrensform.ToString();
        _txtBuildingDestroy.text = save.BuildingDestroy.ToString();
        _txtRunTime.text = save.Runtime.ToString();

        gameObject.SetActive(true);
    }
}