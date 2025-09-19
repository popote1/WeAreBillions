using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class UIFBDropDownSpriteChange : MonoBehaviour , IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Dropdown _dropdown;
    [SerializeField] private Image _imgToSwap;

    [Space(5)] [Header("Sprites")]
    [SerializeField] private Sprite _idleSprite;
    [SerializeField] private Sprite _ExtendedSprite;
    [SerializeField] private Sprite _HoverSprite;
    
    void Start() {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.onValueChanged.AddListener( OnValueChange);
    }
    
    public void OnValueChange(int value)
    {
        if (_imgToSwap == null) return;
        _imgToSwap.sprite = _idleSprite;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (_imgToSwap == null) return;
        _imgToSwap.sprite = _ExtendedSprite;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_imgToSwap == null) return;
        _imgToSwap.sprite = _HoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_imgToSwap == null) return;
        _imgToSwap.sprite = _idleSprite;
    }
}
