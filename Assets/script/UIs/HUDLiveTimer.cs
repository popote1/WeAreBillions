using script;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class HUDLiveTimer : MonoBehaviour
{

    [SerializeField]private TMP_Text _txtTimer;
    private void Update() {
        _txtTimer.text = StaticData.GetGameTime();
    }
}