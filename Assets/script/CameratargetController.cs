using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameratargetController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _camera;
    public float CameraScrollmoveSpeed;

    public Vector2 MaxPosition = new Vector2(10,10);
    public Vector2 MinPosition = new Vector2(0,0);
    [SerializeField] private LayerMask _groundLayer;

    //[Space(10), Header("Zoom")] 
    //[SerializeField] private float _minZoom = 10;
    //[SerializeField]private float _maxZoom = 40;
    //[SerializeField]private float _zoomPower = 1;
    //private float zoomValue;
   

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

    //private void ManageZoom() {
    //    float zoom = Input.GetAxis("Mouse ScrollWheel");
    //    zoomValue += zoom * _zoomPower;
    //    zoomValue = Mathf.Clamp01(zoomValue);
    //    _camera.
    //}

    private float GetHight() {
        RaycastHit hit;
        if(Physics.Raycast(new Ray(transform.position+ new Vector3(0,1000,0), Vector3.down), out hit, 2000, _groundLayer)){
            return hit.point.y;
        }
        return 0;
    }
}
