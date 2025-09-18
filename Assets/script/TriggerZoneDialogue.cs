using script;
using UnityEngine;

public class TriggerZoneDialogue : MonoBehaviour {
    [SerializeField] private bool _wasPlay;
    [SerializeField] private DialogueStep[] _dialogueSteps;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombi")) {
            _wasPlay = true;
            StaticEvents.StartPlayingDialogue(_dialogueSteps);
            gameObject.SetActive(false);
        }
    }
}