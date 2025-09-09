using UnityEngine;

public class IsignifiantSimple : InsignifiantProbs
{
    [SerializeField] protected GameObject[] _prfDebris;
    [SerializeField] protected Collider[] _colliders;

    protected override void Destroy() {
        DisableColliders(_colliders);
        SpawDebrie();
        base.Destroy();
        Destroy(gameObject);
    }

    private void SpawDebrie()
    {
        GameObject go = Instantiate(_prfDebris[Random.Range(0, _prfDebris.Length)]);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
    }

    
}