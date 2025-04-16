using System;
using System.Collections;
using System.Collections.Generic;
using script;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class AlertSystemManager : MonoBehaviour
{
    public static AlertSystemManager Instance;
    public event EventHandler<int> OnAlertLevelChange; 
    public float AlertScore {
        get => _alertScore;
    }

    public int AlertLevel {
        get => _alertLevel;
    }

    private float _alertScore=0;
    private int _alertLevel=0;

    [Header("Combat Increase")] 
    [SerializeField] private float _delayToOutCombat = 10;
    [SerializeField] private bool _isInCombat;
    [SerializeField] private float _inCombatIncrease=0.5f;
    [Header("LevelParameters")] 
    [SerializeField] private float _alertBurnDownRate =0.1f;

    [SerializeField] private AlertLevelData[] _alertLevelDatas;

    private AlertLevelData _currentAlertData;
    private float _inGameCombatTimer;
    private float _defaultMaxValue = 200;
    
    
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.LogWarning("Try To Have 2 AlertManager On The Scene");
        }

        _currentAlertData = _alertLevelDatas[0];
    }

    private void Update() {
        ManagerBurnDown();
        ManagerInGameCombatTimer();
        _currentAlertData.ManageEventsDuringLevel();
    }

    public float GetAlertProgress() {
        if (CanAlertIncrease()) {
            return (_alertScore -_currentAlertData.MinimumLevel)/ (_alertLevelDatas[_alertLevel + 1].MinimumLevel-_currentAlertData.MinimumLevel);
        }
        return (_alertScore - _currentAlertData.MinimumLevel) / _defaultMaxValue ;
    }
    
    [ContextMenu("IncreaseAlert")]
    public void DebugIncreaseAlertScore() {
        IncreaseAlertScore(50);
    }
    public void IncreaseAlertScore(float value)
    {
        _alertScore += value;
        if (CanAlertIncrease()&&_alertLevelDatas[_alertLevel+1].IsScoreBuggierThanMinimum(_alertScore)) {
            LevelUpAlert();
        }
    }

    public void FireShot() {
        _isInCombat = true;
        _inGameCombatTimer = _delayToOutCombat;
    }
    

    private void ManagerBurnDown() {
        if (!_isInCombat) {
            _currentAlertData.ManageBurnDown(_alertScore, _alertBurnDownRate, out _alertScore, out var levelDownAlert);
            if (_alertLevel > 0 && levelDownAlert)
            {
                LevelDownAlert();
            }
        }
        else {
            IncreaseAlertScore(_inCombatIncrease*Time.deltaTime);
        }
    }

    private void ManagerInGameCombatTimer() {
        if (_inGameCombatTimer <= 0) return;
        _inGameCombatTimer -= Time.deltaTime;
        if (_inGameCombatTimer <= 0) {
            _isInCombat = false;
            _inGameCombatTimer = 0;
        }
    }

    private void LevelDownAlert() {
        _alertLevel--;
        _currentAlertData = _alertLevelDatas[_alertLevel];
        OnAlertLevelChange?.Invoke(this, _alertLevel);
        StaticData.SetCurrentAlertLevel(_alertLevel);
    }

    private void LevelUpAlert() {
        _alertLevel++;
        _currentAlertData = _alertLevelDatas[_alertLevel];
        _currentAlertData.DoOnStartEvents();
        StaticData.SetCurrentAlertLevel(_alertLevel);
        OnAlertLevelChange?.Invoke(this, _alertLevel);
    }
    
    private bool CanAlertIncrease() {
        if (_alertLevelDatas.Length > _alertLevel + 1 && _alertLevelDatas[_alertLevel + 1] != null ) 
            return true;
        return false;
        
    }

    
}