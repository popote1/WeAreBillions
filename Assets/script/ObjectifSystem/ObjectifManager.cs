using System;
using System.Collections.Generic;
using script;
using UnityEngine;

public class ObjectifManager : MonoBehaviour {
    
    [SerializeField] private List<ObjectifAbstract> _objectifs;
    
    private ObjectifAbstract _currentObjectif;

    private void Start() {
        SetNewCurrentObjectif(_objectifs[0]);
    }

    private void SetNewCurrentObjectif(ObjectifAbstract objo) {
        _currentObjectif = objo;
        StaticEvents.ChangeCurrentObjectif(_currentObjectif);
        if (_currentObjectif == null) return;
        _currentObjectif.OnStartObjectif();
        
    }

    private void Update() {
        if (_currentObjectif == null) return;
        if (_currentObjectif.CheckIfObjectifIsComplet()) {
            _currentObjectif.OnEndObjectif();
            Debug.Log("Objectif Complet");
            if (_currentObjectif.DoWin) StaticEvents.EndGame(true);
            TryGoToNextObjectif();
        }
    }

    private void TryGoToNextObjectif() {
        int currentid = _objectifs.IndexOf(_currentObjectif);
        if (_objectifs.Count <= currentid + 1 || _objectifs[currentid + 1] == null) {
            SetNewCurrentObjectif(null);
            return;
        }
        SetNewCurrentObjectif(_objectifs[currentid+1]);
    }
}