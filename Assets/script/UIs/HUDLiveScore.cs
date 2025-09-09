using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace script.UIs
{
    public class HUDLiveScore :MonoBehaviour {

        [SerializeField] private TMP_Text _txtScore;
        [Space(10)]
        [SerializeField] private float _tweeningTimeScoreChange =0.5f;
        [SerializeField] private float _tweeningSizeScoreChange =1.5f;
        [SerializeField] private AnimationCurve _animationCurveScoreChange = AnimationCurve.Linear(0,0,1,0);
        [Space(10)]
        [SerializeField] private float _ScoreIncreaseMaxSpeed = 10;

        private float _displayScore;
        private float _currentScore;
        private void Start() {
            StaticScoringSystem.OnScoreChange+= StaticScoringSystemOnOnScoreChange;
        }

        private void OnDestroy() {
            StaticScoringSystem.OnScoreChange -= StaticScoringSystemOnOnScoreChange;
        }

        private void StaticScoringSystemOnOnScoreChange(object sender, int newScore) {
            _currentScore = newScore;
            DoScoreTween();
        }

        private void Update() {
            if (_displayScore != _currentScore) {
                if (_displayScore < _currentScore) {
                    _displayScore += _ScoreIncreaseMaxSpeed * Time.deltaTime;
                }
                else {
                    _displayScore = _currentScore;
                }

                _txtScore.text = Mathf.FloorToInt(_displayScore).ToString();
            }
        }

        [ContextMenu("TestTween")]
        private void DoScoreTween() {
            _txtScore.transform.localScale = Vector3.one;
            _txtScore.transform.DOPause();
            _txtScore.transform.DOScale(_tweeningSizeScoreChange, _tweeningTimeScoreChange)
                .SetEase(_animationCurveScoreChange);
        }
    }
}