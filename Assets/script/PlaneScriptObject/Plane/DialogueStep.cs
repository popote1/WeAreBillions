using System;
using UnityEngine;

namespace script {
    [Serializable]
    public class DialogueStep
    {
        [TextArea]public string TxtDialogue;
        public Sprite SpriteDialogue;
        public bool UsCameraScroll;
        public bool ReturnToBeginingCameraPos;
        public float ScrollSpeed = 3;
        public Vector3 EndCameraPosition;
    }
}