using UnityEngine;

namespace script.UIs
{
    public class HUDSelectionBoxDisplayer : MonoBehaviour {
        
        public RectTransform SelectionBox;
        public void SetSelectionBox(Vector2 pos, Vector2 size) {
            SelectionBox.gameObject.SetActive(true);
            SelectionBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            SelectionBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            SelectionBox.position = pos;
        }

        public void CloseSelectionBox() => SelectionBox.gameObject.SetActive(false);
    }
}