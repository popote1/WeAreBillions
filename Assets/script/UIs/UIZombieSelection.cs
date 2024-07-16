using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace script.UIs {
    [Serializable]
    public class UIZombieSelection :MonoBehaviour {
        [SerializeField] private TMP_Text _txtHeader;
        [SerializeField] private TMP_Text _txtCount;
        [SerializeField] private RawImage _imgportrait;
        [SerializeField] private Button _bpButton;

        public void DisplaySelectionInformation(List<GridAgent> agents)
        {
            if (agents == null || agents[0] == null) return;
            _txtHeader.text = agents[0].name;
            _txtCount.text = agents.Count.ToString();
            gameObject.SetActive(true);
        }

    }
}