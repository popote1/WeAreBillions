using script;
using UnityEngine;

public class DebugEndGameTrigger : MonoBehaviour
{

    [SerializeField] private bool _doWin;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombi")) {
            
            if( _doWin) StaticEvents.OnGameWin?.Invoke();
            else StaticEvents.OnGameLose?.Invoke();
        }
    }
}
