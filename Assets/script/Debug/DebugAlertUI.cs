using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugAlertUI : MonoBehaviour
{
    private AlertSystemManager _alertSystemManager;

    [SerializeField] private TMP_Text _txtLevel;
    [SerializeField] private TMP_Text _txtScore;
    private void Start() {
        _alertSystemManager = AlertSystemManager.Instance;
    }

    private void Update() {
        _txtLevel.text = _alertSystemManager.AlertLevel.ToString();
        _txtScore.text = Mathf.Round(_alertSystemManager.AlertScore).ToString();
    }
}
