using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallContaner : MonoBehaviour
{
    public string _tagHandle = "WalkBlocker";
    public LayerMask _layerMask = 4;
    public Material _material;
    
    [ContextMenu("ShowBlocks")]
    public void ShowAllChildrens() {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(i).GetComponent<MeshRenderer>().material = _material;
        }
    }
    [ContextMenu("HideBlocks")]
    public void HideAllChildrens() {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ApplyTagAndLayer() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.tag = _tagHandle;
            transform.GetChild(i).gameObject.layer = _layerMask;
        }
    }
}
