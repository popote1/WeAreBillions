using System.Linq;
using DG.Tweening;
using script;
using TMPro;
using UnityEngine;

public class HUDAlertAnnoncer : MonoBehaviour
{
    [SerializeField] private TMP_Text _txtLevel;
    [SerializeField] private CanvasGroup _canvasGroupMainPanel;
    [SerializeField] private float _rotationAnimationTime = 1;
    [SerializeField] private AnimationCurve _animationCurveRotationAnimation = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] private float _delayBeforeFadeOut = 2;
    [SerializeField] private float _fadeOutTime =2f;
    [Space(10), Header("Stars")]
    [SerializeField] private GameObject[] _stars;
    [SerializeField] private float _starsAnimationSize = 2;
    [SerializeField] private float _starsAnimationTime = 0.75f;
    [SerializeField] private AnimationCurve _animationCurveStarsAnimation = AnimationCurve.EaseInOut(0,0,1,1);
    
    void Start() {
        _canvasGroupMainPanel.alpha = 0;
        StaticEvents.OnAlertLevelChange+= StaticEventsOnOnAlertLevelChange;
    }

    private void StaticEventsOnOnAlertLevelChange(object sender, int lvl) {
        PlayNewLevel(lvl);
    }
    

    [ContextMenu("Test with 5 starts")]
    private void TestWith5Start() {
        PlayNewLevel(5);
    }
    [ContextMenu("Test with 1 starts")]
    private void TestWith1Start() {
        PlayNewLevel(1);
    }

    private void PlayNewLevel(int value)
    {
        _txtLevel.text = "Niveau " + value;
        float delay = 0;
        _canvasGroupMainPanel.DOPause();
        _canvasGroupMainPanel.transform.localEulerAngles = new Vector3(-90,0, 0);
        _canvasGroupMainPanel.alpha = 1;
        _canvasGroupMainPanel.transform.DOLocalRotate(Vector3.zero, _rotationAnimationTime).SetEase(_animationCurveRotationAnimation);
        delay += _rotationAnimationTime;
        for (int i = 0; i < _stars.Length; i++) {
            if (i < value)
            {
                _stars[i].SetActive(true);
                delay += _starsAnimationTime;
                _stars[i].transform.DOPause();
                _stars[i].transform.DOScale(_starsAnimationSize, _starsAnimationTime)
                    .SetDelay(delay - _starsAnimationTime).SetEase(_animationCurveStarsAnimation);
            }
            else
            {
                _stars[i].SetActive(false); 
            }
        }

        delay += _delayBeforeFadeOut;
        _canvasGroupMainPanel.DOFade(0, _fadeOutTime).SetDelay(delay);
    }
    
}
