using System.Collections.Generic;
using UnityEngine;

public class AlertGridAgentSpawner : AlertEvent
{

    
    
    [SerializeField] private float _radius;
    [SerializeField] private GridAgent[] _agentToSpawn;
    
    [SerializeField] private UnitStartOrder _unitStartOrder;

    public float Radius => _radius;
    
    public override void DoEvent() {
        SpawnAndOrder();
    }

    public Vector3 GetMoveTarget() {
        if (_unitStartOrder != null && _unitStartOrder.Targets.Length > 0 && _unitStartOrder.Targets[0] != null) {
            return _unitStartOrder.Targets[0];
        }
        return transform.position;
    }
    [ContextMenu("Spanw And Order")]
    private void SpawnAndOrder() {
        List<GridAgent> agens = SpawnAgent();
        if (_unitStartOrder == null) return;
        _unitStartOrder.GiveAgentMoveOrder(agens);
        
    }

    [ContextMenu("Spawn Agents")]
    private void DebugSpawnAgents() => SpawnAgent();
    private List<GridAgent> SpawnAgent() {
        List<GridAgent> agents = new List<GridAgent>();
        foreach (GridAgent agent in _agentToSpawn) {
            if (agent == null) return null;
            Vector3 pos = transform.position +
                          new Vector3(Random.Range(-_radius, _radius), 0, Random.Range(-_radius, _radius));
            agents.Add(Instantiate(agent, pos, transform.rotation));
        }
        return agents;
    }
    
    

    private void OnDrawGizmos() {
        if (EditorControlStatics.DisplayGizmos) {
            Color col = Color.blue;
            col.a = 0.4f;
            Gizmos.color = col; 
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }

    
}
