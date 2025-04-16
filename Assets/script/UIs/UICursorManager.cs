using UnityEngine;

public class UICursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D DefaultCursor;
    [SerializeField] private Texture2D OverZombieCursor;

    private Camera _camera;
    void Start() {
        _camera = Camera.main;
        Cursor.SetCursor(DefaultCursor,new Vector2(0f,0),CursorMode.Auto);
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            if (hit.collider.CompareTag("Zombi")) {
                Cursor.SetCursor(OverZombieCursor,Vector2.zero,CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(DefaultCursor,new Vector2(0,0),CursorMode.Auto);
                
            }
        }
    }
}
