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
        [SerializeField] private List<GridAgent> _agents;

        private void Start() {
            _bpButton.onClick.AddListener(SubmitSelection);
        }

        public void DisplaySelectionInformation(List<GridAgent> agents)
        {
            if (agents == null || agents[0] == null) return;
            _txtHeader.text = agents[0].AgentName;
            _txtCount.text = agents.Count.ToString();
            _imgportrait.texture = agents[0].Portrait.texture;
            gameObject.SetActive(true);
            _agents = agents;
        }

        public void SubmitSelection() {
            StaticEvents.SubmitSelection(_agents);
        }

    }
}