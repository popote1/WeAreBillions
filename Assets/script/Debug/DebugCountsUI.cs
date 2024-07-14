using System.Collections;
using System.Collections.Generic;
using script;
using TMPro;
using UnityEngine;

public class DebugCountsUI : MonoBehaviour {
    [SerializeField] private TMP_Text _txtTimer;
    [SerializeField] private TMP_Text _txtZombies;
    [SerializeField] private TMP_Text _txtCivilians;
    [SerializeField] private TMP_Text _txtDefenders;
    [SerializeField] private TMP_Text _txtBuildings;
    [SerializeField] private TMP_Text _txtDestroyBuildings;
    [SerializeField] private TMP_Text _txtDestroyElements;
    
    void Update() {
        _txtTimer.text =GetTimer();
        _txtZombies.text = StaticData.ZombieCount.ToString();
        _txtCivilians.text = StaticData.CiviliansCounts.ToString();
        _txtDefenders.text = StaticData.DefendersCount.ToString();
        _txtBuildings.text = StaticData.BuildingsCount.ToString();
        _txtDestroyBuildings.text = StaticData.DestroyBuilding.ToString();
        _txtDestroyElements.text = StaticData.DestroyElements.ToString();
    }

    private string GetTimer() {
        int secondes = Mathf.RoundToInt(StaticData.GameTimer) % 60;
        int minutes = Mathf.RoundToInt(StaticData.GameTimer) / 60;
        return minutes + ":" + secondes;
    }
}
