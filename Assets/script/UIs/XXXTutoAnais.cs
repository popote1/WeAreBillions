using UnityEngine;

public class XXXTutoAnais : MonoBehaviour
{
    public string MonMessage = "Mon Message par defautl";
    [SerializeField] private GameObject _monObjet;
    
    private void Start() {
        
    }
    
    private void Update() {
    }

   


    public void GoToDestination() {
        Debug.Log(MonMessage);
        _monObjet.transform.localScale +=  Vector3.one;
    }

    public void ActiveLObjet() {
        _monObjet.SetActive(true);
    }

    public void DesactiveLObjet() {
        _monObjet.SetActive(false);
    }
}
