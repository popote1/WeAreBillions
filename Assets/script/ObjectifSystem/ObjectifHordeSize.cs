using System;
using script;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ObjectifHordeSize : ObjectifAbstract
{
    [SerializeField] private int _hordeSize;
    [SerializeField]private UnityEvent OnObjectifStart;
    [SerializeField]private UnityEvent OnObjectifComplet;
    public override void OnStartObjectif() {
        OnObjectifStart?.Invoke();
    }

    public override void OnEndObjectif() {
        OnObjectifComplet?.Invoke();
    }

    public override bool CheckIfObjectifIsComplet() {
        return StaticData.zombieCount >= _hordeSize;
    }
}