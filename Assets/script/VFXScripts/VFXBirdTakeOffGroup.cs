using UnityEngine;

public class VFXBirdTakeOffGroup : MonoBehaviour
{
    [SerializeField] private VFXBirdTakeoff[] _birds;
    [SerializeField] private Vector3 _dir;
    [SerializeField] private float _maxDelay = 0.5f;
    


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            StartBirdFly();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ResetBirds();
        }
    }

    [ContextMenu("StartFly")]
    private void StartBirdFly() {
        foreach (var bird in _birds) {
            if (bird == null) continue;
            bird.SetReadyToFlyAway(_dir, Random.Range(0,_maxDelay));
        }
    }
    [ContextMenu("RestBird")]
    private void ResetBirds() {
        foreach (var bird in _birds) {
            if (bird == null) continue;
            bird.ResetDefaultPos();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GridAgent>())
        {
            Vector3 dir = transform.position - other.transform.position;
            _dir = dir.normalized;
            _dir.y = 0;
            Quaternion qua = Quaternion.LookRotation(_dir, Vector3.up);
            _dir = qua.eulerAngles;
            Debug.Log("Dir = "+_dir);
            StartBirdFly();
        }
    }
}