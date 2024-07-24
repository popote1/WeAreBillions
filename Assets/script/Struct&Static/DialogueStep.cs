using System;
using UnityEngine;

namespace script {
    [Serializable]
    public class DialogueStep
    {
        public string TxtDialogue;
        public Sprite SpriteDialogue;
        [Header("CameraTravel")]
        public bool UsCameraScroll;
        public float ScrollSpeed = 3;
        public Vector3 EndCameraPosition;
    }
}