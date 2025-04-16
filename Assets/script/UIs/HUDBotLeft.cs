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
        

        public void Start() {
            CleanAllSelection();
            _panelInfo.SetActive(false);
            _gameController.OnSelectionChange+= GameControllerOnOnSelectionChange;
        }

        private void OnDestroy()
        {
            _gameController.OnSelectionChange-= GameControllerOnOnSelectionChange;
        }

        private void GameControllerOnOnSelectionChange(object sender, List<GridAgent> e)
        {
            SetSelection(e);
        }

        public void DisplayInformation(GridAgent zombieAgent) {
            
            _txtInspectorHeader.text = zombieAgent.name;
            _txtInspectorHP.text = zombieAgent.HP.ToString();
            _txtInspectorSpeed.text = zombieAgent.MaxMoveSpeed.ToString();
            _txtInspectorAttack.text = ((ZombieAgent)zombieAgent).Attack.Damage.ToString();
            _txtInspectorKeyWord.text = zombieAgent.UniteTyp.ToString();
            _panelInfo.SetActive(true);
        }

        public void SetSelection(List<GridAgent> selection) {
            if (selection == null|| selection.Count==0) {
                CleanAllSelection();
                _panelInfo.SetActive(false);
                return;
            }
            if( selection[0]==null) return;
            _uiZombieSelections[0].DisplaySelectionInformation(selection);
            DisplayInformation(selection[0]);
        }
        

        private void CleanAllSelection() {
            foreach (var ui in _uiZombieSelections) {
                ui.gameObject.SetActive(false);
            }
        }
    }
}