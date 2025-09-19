using script;
using UnityEngine;

public class DialogueLauncher: MonoBehaviour {
    [SerializeField] private DialogueStep[] _dialogueSteps;
    public void PlayDialgue() {
        StaticEvents.StartPlayingDialogue(_dialogueSteps);
    }
}