using System;
using JetBrains.Annotations;
using script;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorManager : MonoBehaviour
{

    public static CursorStatEnum CursorStat;
    public enum CursorStatEnum {
        Idle, Select, UI, Drag, Attack
    }

    [SerializeField] private Vector2 _hotSpot = new Vector2(70, 120);
    [SerializeField] private Texture2D _textIdle;
    [SerializeField] private Texture2D _texSelect;
    [SerializeField] private Texture2D _texUI;
    [SerializeField] private Texture2D _texDrag;
    [SerializeField] private Texture2D _texAttack;

    private Camera _camera;
    void Start() {
        _camera = Camera.main;
        Cursor.SetCursor(_textIdle,new Vector2(1f,1),CursorMode.Auto);
    }

    public void ChangeCursorStat(UICursorManager.CursorStatEnum newStat)
    {
        if (newStat == CursorStat) return;
        CursorStat = newStat;

        switch (CursorStat) {
            case CursorStatEnum.Idle: Cursor.SetCursor(_textIdle,_hotSpot,CursorMode.Auto); break;
            case CursorStatEnum.Select: Cursor.SetCursor(_texSelect,_hotSpot,CursorMode.Auto); break;
            case CursorStatEnum.UI: Cursor.SetCursor(_texUI,_hotSpot,CursorMode.Auto); break;
            case CursorStatEnum.Drag: Cursor.SetCursor(_texDrag,_hotSpot,CursorMode.Auto); break;
            case CursorStatEnum.Attack: Cursor.SetCursor(_texAttack,_hotSpot,CursorMode.Auto); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    void Update()
    {
        if (StaticData.IsDraging) {
            ChangeCursorStat(CursorStatEnum.Drag);
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject()) {
            ChangeCursorStat(CursorStatEnum.UI);
            CursorStat = CursorStatEnum.UI;
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            if (hit.collider.CompareTag("Zombi")) {
                ChangeCursorStat(CursorStatEnum.Select);
                return;
            }
            else if (hit.collider.CompareTag("Defender") ||hit.collider.CompareTag("Destructible")) {
                ChangeCursorStat(CursorStatEnum.Attack);
                return;
            }
        }
        ChangeCursorStat(CursorStatEnum.Idle);
    }
}

