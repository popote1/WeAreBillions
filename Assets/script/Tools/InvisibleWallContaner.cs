using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallContaner : MonoBehaviour
{
    [SerializeField] private Material _material;
    [ContextMenu("ShowBlocks")]
    private void ShowAllChildrens() {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            transform.GetChild(i).GetComponent<MeshRenderer>().material = _material;
        }
    }
    [ContextMenu("HideBlocks")]
    private void HideAllChildrens() {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
