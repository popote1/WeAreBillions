using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIFBScoreEndGame : MonoBehaviour
{
    
    [SerializeField] private TMP_Text _txtDisplay;
    [SerializeField] private float _delayBeforeAnim = 0;
    [SerializeField] private float _animation1Time = 2;
    [SerializeField] private float _aniamtion2TIme = 0.5f;
    [SerializeField] private AnimationCurve _animation2Curve;
    [SerializeField] private float _animation2Scale = 2;
    [SerializeField] private bool _displayDate;

    private AnimationStats _animationStats = AnimationStats.idle;
    private int _targetValue;
    private float _timer;
    
    private enum  AnimationStats {
        idle, delay, display
    }
    
    public void StartDisplaying(int targetValue, float delay = -1) {
        if (delay != -1) {
            _delayBeforeAnim = delay;
        }

        _targetValue = targetValue;

        _timer = 0;
        _animationStats = AnimationStats.delay;
        _txtDisplay.text = "";
    }
   
    void Update()
    {
        switch (_animationStats)
        {
            case AnimationStats.idle: break;
            case AnimationStats.delay:ManageDelay(); break;
            case AnimationStats.display:ManageDisplay(); break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ManageDelay() {
        _timer += Time.deltaTime;
        if (_timer >= _delayBeforeAnim) {
            _timer = 0;
            _animationStats = AnimationStats.display;
        }
    }

    private void ManageDisplay() {
        _timer += Time.deltaTime;

        DisplayValue(Mathf.RoundToInt(Mathf.Lerp(0, _targetValue, _timer / _animation1Time)));
        
        if (_timer >= _animation1Time) {
            _timer = 0;
            DisplayValue(_targetValue);
            StartEndAnimation();
            _animationStats = AnimationStats.idle;

        }
        
    }

    private void DisplayValue(int value) {
        if (_displayDate) {
            int sec = Mathf.FloorToInt(value % 60);
            int minute = Mathf.FloorToInt((value/60) % 60);
            _txtDisplay.text=minute + " Min  " + sec + " Sec";
        }
        else
        {
            _txtDisplay.text = value.ToString();
        }
    }

    [ContextMenu("TextFinal Aniamtion")]
    private void StartEndAnimation() {
        _txtDisplay.transform.localScale = Vector3.one;
        _txtDisplay.transform.DOScale(_animation2Scale, _aniamtion2TIme).SetEase(_animation2Curve);
    }
}
