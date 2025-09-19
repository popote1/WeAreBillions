using System;
using script;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DestroyTargetEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _onAllTargetDestroy;
    [SerializeField] private GridAgent[] _targetToDestroy;
    [SerializeField] private House[] _destructiblesToDestroy;
    [SerializeField] private bool _isWinOnComplet;

    public GridAgent[] TargetToDestroy=>  _targetToDestroy;
    public House[] DestructiblesToDestroy => _destructiblesToDestroy;
    

    private int _agentCount= 0;
    private int _buildingCound = 0;
    void Start() {
        foreach (var target in _targetToDestroy) {
            if (target ==null)continue;
            _agentCount++;
            target.OnGridAgentDestroy+= TargetOnOnGridAgentDestroy;
        }
        foreach (var target in _destructiblesToDestroy)
        { if (target ==null)continue;
            _buildingCound ++;
            target.OnDestructibleDestroy+= TargetOnOnDestructibleDestroy;
        }
    }

    private void TargetOnOnDestructibleDestroy(object sender, EventArgs e) {
        _buildingCound--;
        CheckAllTargetDestroy();
    }

    private void TargetOnOnGridAgentDestroy(object sender, EventArgs e) {
        _agentCount--;
        CheckAllTargetDestroy();
    }

    private void CheckAllTargetDestroy() {
        if (_agentCount <= 0 && _buildingCound <= 0) {
            if (_isWinOnComplet)StaticEvents.EndGame(true);
            _onAllTargetDestroy?.Invoke();
        }
    }
    
}

