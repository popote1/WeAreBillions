using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameratargetController : MonoBehaviour
{
    public float CameraScrollmoveSpeed;

    public Vector2 MaxPosition = new Vector2(10,10);
    public Vector2 MinPosition = new Vector2(0,0);
    [SerializeField] private LayerMask _groundLayer;
   

    // Update is called once per frame
    void Update() {
        if (InGameStatic.BlockCameraMovement) return;
            
        float vetical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        float y = 0;
        if (InGameStatic.AllowCameraHeight) y = GetHight();
        Vector3 MoveVector = (new Vector3(horizontal, 0, vetical)+InGameStatic.CameraMoveVector)*CameraScrollmoveSpeed* Time.deltaTime;
        Vector3 newPos = transform.position + MoveVector;
        
        transform.position = new Vector3(Mathf.Clamp(newPos.x, MinPosition.x, MaxPosition.x), y
            ,Mathf.Clamp(newPos.z,MinPosition.y, MaxPosition.y ));
    }

    private float GetHight() {
        RaycastHit hit;
        if(Physics.Raycast(new Ray(transform.position+ new Vector3(0,1000,0), Vector3.down), out hit, 2000, _groundLayer)){
            return hit.point.y;
        }
        return 0;
    }
}
