using UnityEngine;

public class TestTankToTerrain : MonoBehaviour
{
    [SerializeField] private Transform _raycaterA;
    [SerializeField] private Transform _raycaterB;
    [SerializeField] private Transform _raycaterC;
    [SerializeField] private Transform _raycaterD;
    [SerializeField] private float _yOffset =0.5f;
    [SerializeField] private Transform _displayCube;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        Vector3 a, b, c, d, ab, bc, cd, da, center, forward, right, top ;
        a= b= c= d=ab= bc= cd= da =center =Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(_raycaterA.position, Vector3.down, out hit)) {
            a = hit.point;
            Debug.DrawLine(_raycaterA.position, a, Color.green);
        }
        if (Physics.Raycast(_raycaterB.position, Vector3.down, out hit)) {
            b = hit.point;
            Debug.DrawLine(_raycaterB.position, b, Color.green);
        }
        if (Physics.Raycast(_raycaterC.position, Vector3.down, out hit)) {
            c = hit.point;
            Debug.DrawLine(_raycaterC.position, c, Color.green);
        }
        if (Physics.Raycast(_raycaterD.position, Vector3.down, out hit)) {
            d = hit.point;
            Debug.DrawLine(_raycaterD.position, d, Color.green);
        }

        ab = Vector3.Lerp(a, b, 0.5f);
        bc = Vector3.Lerp(b, c, 0.5f);
        cd = Vector3.Lerp(c, d, 0.5f);
        da = Vector3.Lerp(d, a, 0.5f);
        center = (a + b + c + d) / 4;
        Debug.DrawLine(center, ab, Color.yellow);
        Debug.DrawLine(center, bc, Color.yellow);
        Debug.DrawLine(center, cd, Color.yellow);
        Debug.DrawLine(center, da, Color.yellow);

        right = bc-da;
        forward = ab - cd;
        top = Vector3.Cross(forward, right);
        Debug.DrawLine(center, center+forward.normalized, Color.blue);
        Debug.DrawLine(center, center+right.normalized, Color.red);
        Debug.DrawLine(center, center+top, Color.green);
        
        Vector3 crossForward = Vector3.Cross(right, top);
        center.y +=_yOffset;
        //_displayCube.transform.up = Vector3.Cross(forward.normalized, right.normalized);
        _displayCube.position = center;
        _displayCube.rotation = Quaternion.LookRotation(forward.normalized, top.normalized);;
    }
}
