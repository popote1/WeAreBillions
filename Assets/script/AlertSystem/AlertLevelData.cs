using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[Serializable]
public class AlertLevelData {
    [SerializeField] private float _minimumLevel = 100;
    [SerializeField] private bool _canGoUnderLevel = false;
    [Header("On Level Start")]
    [SerializeField] private AlertEvent[] _eventOnLevelStart;

    [Header("DuringLevel")] 
    
    [SerializeField] private float _minimumTimeForEvent=30f;
    [SerializeField] private float _maxTimeForEvent=60f;
    [SerializeField] private AlertEvent[] _eventsDuringLevel;
    

    private float _timeForEvent;
    private float _timerEventsDuringLevel;

    public float MinimumLevel => _minimumLevel;


    public void GenerateNewTimeForEvent() => _timeForEvent = Random.Range(_minimumTimeForEvent, _maxTimeForEvent);

    public void ManageBurnDown(float alertScore, float burnDown, out float newAlertScore, out bool levelDownAlert) {
        if (alertScore > 0) {
            alertScore -= Time.deltaTime * burnDown;
        }
        newAlertScore = alertScore;
        levelDownAlert = false;  
        
        if (alertScore < _minimumLevel) {
            if (_canGoUnderLevel) levelDownAlert = true;
            else newAlertScore = _minimumLevel;
        }
    }
    public bool IsScoreBuggierThanMinimum(float score) {
        if (score >= _minimumLevel) {
            return true;
        }
        return false;
    }

    public void DoOnStartEvents() {
        _timerEventsDuringLevel = 0;
        foreach (var startEvent in _eventOnLevelStart) {
            if (startEvent == null) continue;
            startEvent.DoEvent();
        }
    }

    public void ManageEventsDuringLevel() {
        if (_eventsDuringLevel.Length <= 0) return;
        
        _timerEventsDuringLevel += Time.deltaTime;
        if (_timerEventsDuringLevel >= _timeForEvent) {
            AlertEvent alert =_eventsDuringLevel[Random.Range(0,_eventsDuringLevel.Length)];
                if (alert!=null)alert.DoEvent();
            _timerEventsDuringLevel = 0;
            GenerateNewTimeForEvent();
        }
    }
    
    

}