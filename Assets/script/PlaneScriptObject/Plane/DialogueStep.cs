using System;
using UnityEngine;
using UnityEngine.Localization;

namespace script {
    [Serializable]
    public class DialogueStep
    {
        public LocalizedString LocalizedDialogue;
        [TextArea]public string TxtDialogue;
        public Sprite SpriteDialogue;
        public bool UsCameraScroll;
        public bool ReturnToBeginingCameraPos;
        public float ScrollSpeed = 3;
        public Vector3 EndCameraPosition;

        public string GetDialogue()
        {
            if (LocalizedDialogue.IsEmpty) return TxtDialogue;
            return LocalizedDialogue.GetLocalizedString();
        }
    }
}