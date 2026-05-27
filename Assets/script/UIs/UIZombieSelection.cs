using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace script.UIs {
    [Serializable]
    public class UIZombieSelection :MonoBehaviour
    {

        public event EventHandler<string> OnPanelClose; 
        [SerializeField] private TMP_Text _txtHeader;
        [SerializeField] private TMP_Text _txtCount;
        [SerializeField] private RawImage _imgportrait;
        [SerializeField] private Button _bpButton;
        [SerializeField] private List<GridAgent> _agents;

        private string _agentName;
        private void Start() {
            _bpButton.onClick.AddListener(SubmitSelection);
            StaticEvents.OnStartLoading += OnStartLoading;
        }

        private void OnDestroy() {
            StaticEvents.OnStartLoading -= OnStartLoading;
        }

        private void OnStartLoading() {
            if (_agents == null) return;
            foreach (var agent in _agents) {
                if (agent == null) continue;
                agent.OnGridAgentDestroy-= AgentOnOnGridAgentDestroy;
            }
        }

        public void DisplaySelectionInformation(List<GridAgent> agents) {
            if (agents == null || agents[0] == null) return;
            _txtHeader.text = agents[0].AgentName;
            _agentName = agents[0].AgentName;
            _txtCount.text = agents.Count.ToString();
            _imgportrait.texture = agents[0].Portrait.texture;
            gameObject.SetActive(true);
            _agents = agents;
            foreach (var agent in _agents) {
                agent.OnGridAgentDestroy+= AgentOnOnGridAgentDestroy;
            }
        }

        private void AgentOnOnGridAgentDestroy(object sender, GridAgent e) {
            if (_agents.Contains(e)) {
                e.OnGridAgentDestroy -= AgentOnOnGridAgentDestroy;
                _agents.Remove(e);
                _txtCount.text = _agents.Count.ToString();
                if (_agents.Count<=0) closePanel();
            }
        }


        public void SubmitSelection() {
            StaticEvents.SubmitSelection(_agents);
        }

        private void closePanel() {
            OnPanelClose?.Invoke(this,_agentName );
            if (gameObject!=null)gameObject.SetActive(false);
        }

    }
}