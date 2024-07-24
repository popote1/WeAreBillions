using script;
using script.UIs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerZoneEndLevel : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombi"))
        {
            SceneManager.LoadScene(0);
        }
    }
}