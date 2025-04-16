using System;
using script;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{

    [SerializeField] private Button _bpWin;
    [SerializeField] private Button _bpLose;
    void Start()
    {
        _bpWin.onClick.AddListener(ClickOnWin);
        _bpLose.onClick.AddListener(ClickOnLose);
    }

    private void Update()
    {
        
    }


    private void ClickOnWin() {
        Debug.Log("ClickOn Win");
        //StaticData.OnGameWin?.Invoke();
    }
    
    private void ClickOnLose() {
        Debug.Log("ClickOn lose");
        //StaticData.OnGameLose?.Invoke();
    }
}
