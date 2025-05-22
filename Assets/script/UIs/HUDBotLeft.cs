using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace script.UIs
{
    public class HUDBotLeft : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [Header("InspectionPanel")] 
        [SerializeField] private GameObject _panelInfo;
        [SerializeField] private TMP_Text _txtInspectorHeader;
        [SerializeField] private TMP_Text _txtInspectorHP;
        [SerializeField] private TMP_Text _txtInspectorSpeed;
        [SerializeField] private TMP_Text _txtInspectorAttack;
        [SerializeField] private TMP_Text _txtInspectorKeyWord;
        [Header("ZombieSelection")] 
        [SerializeField]private UIZombieSelection[] _uiZombieSelections;
        
        private Dictionary<string, List<GridAgent>> _selectedAgents;
        

        public void Start() {
            CleanAllSelection();
            _panelInfo.SetActive(false);
            StaticEvents.OnSelectionChange+= GameControllerOnOnSelectionChange;
        }

        private void OnDestroy()
        {
            StaticEvents.OnSelectionChange-= GameControllerOnOnSelectionChange;
        }

        private void GameControllerOnOnSelectionChange(object sender, List<GridAgent> e)
        {
            SetSelection(e);
        }

        public void DisplayInformation(GridAgent zombieAgent) {
            
            _txtInspectorHeader.text = zombieAgent.AgentName;
            _txtInspectorHP.text = zombieAgent.HP.ToString();
            _txtInspectorSpeed.text = zombieAgent.MaxMoveSpeed.ToString();
            _txtInspectorAttack.text = ((ZombieAgent)zombieAgent).Attack.Damage.ToString();
            _txtInspectorKeyWord.text = zombieAgent.UniteType.ToString();
            _panelInfo.SetActive(true);
        }

        public void SetSelection(List<GridAgent> selection) {
            _selectedAgents = new Dictionary<string, List<GridAgent>>();

            foreach (var agent in selection) {
                if (_selectedAgents.ContainsKey(agent.AgentName)) {
                    _selectedAgents[agent.AgentName].Add(agent);
                }
                else {
                    _selectedAgents.Add(agent.AgentName, new List<GridAgent>());
                    _selectedAgents[agent.AgentName].Add(agent);
                }
            }
            
            
            if (selection == null|| selection.Count==0) {
                CleanAllSelection();
                _panelInfo.SetActive(false);
                return;
            }

            int i = 0;
            foreach (var selections in _selectedAgents)
            {
                _uiZombieSelections[i].DisplaySelectionInformation(selections.Value);
                i++;
            }
            
            if( _selectedAgents!=null&& _selectedAgents.Count==1) DisplayInformation(selection[0]);
            //if( selection[0]==null) return;
            //_uiZombieSelections[0].DisplaySelectionInformation(selection);
            //DisplayInformation(selection[0]);
        }
        

        private void CleanAllSelection() {
            foreach (var ui in _uiZombieSelections) {
                ui.gameObject.SetActive(false);
            }
        }
    }
}