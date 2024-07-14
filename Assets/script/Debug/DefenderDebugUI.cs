using UnityEngine;
using UnityEngine.UI;

public class DefenderDebugUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _imgAlertCalling;
    [SerializeField] private Defender _defender;

    private bool _isAlertCalling;

    public void Start()
    {
        _defender.OnChangeAlertCallingStat+= DefenderOnOnChangeAlertCallingStat;
    }

    private void DefenderOnOnChangeAlertCallingStat(object sender, bool value) {
        _isAlertCalling = value;
        _imgAlertCalling.enabled = value;
    }

    void Update()
    {
        if (!_isAlertCalling) return;
        _canvas.transform.localEulerAngles = new Vector3(0,180-_defender.transform.eulerAngles.y, 0);
        _imgAlertCalling.fillAmount = _defender.AlertCallProgress;
    }
}