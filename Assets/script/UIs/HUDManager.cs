using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace script.UIs
{
    public class HUDManager :MonoBehaviour
    {
        //public TMP_Text TxtZombieCount;
        public GameObject PanelWin;
        public GameObject PanelLose;
        [SerializeField] private Button _bpMenuPause;
        

        private void Start() {
            //StaticData.OnZombieGain += SetZombieValue;
            //StaticData.OnZombieLose += SetZombieValue;
            StaticData.OnGameWin += PlayWin;
            StaticData.OnGameLose += PlayLose;
            _bpMenuPause.onClick.AddListener(ClickOnPauseButton);
        }

        private void OnDestroy()
        {
            //StaticData.OnZombieGain -= SetZombieValue;
            //StaticData.OnZombieLose -= SetZombieValue;
            StaticData.OnGameWin -= PlayWin;
            StaticData.OnGameLose -= PlayLose;
        }

        //public void SetZombieValue() {
        //    TxtZombieCount.text = StaticData.ZombieCount.ToString();
        //}

        public void PlayWin() {
            PanelWin.SetActive(true);
        }

        public void PlayLose() {
            PanelLose.SetActive(true);
        }

        public void ReturnToMainMenu() {
            GridManager.Instance = null;
            SceneManager.LoadScene(0);
        }
        
        private void ClickOnPauseButton() {
            StaticData.SetGameOnPause(true);
        }
    }

    public class HUDDialogueManager() : MonoBehaviour {
        [SerializeField] private GameObject _hudDialoguePanel;
        [SerializeField] private TMP_Text _txtDiaglogue;
        [SerializeField] private RawImage _imgDialogue;

        private DialogueStep[] _currentDialogue;
        private int _currentSteop = 0;

        private float _cameraTravelTime;
        private float _cameraTimer;
        private void Start() {
            StaticData.OnPlayDialogue+= StaticDataOnOnPlayDialogue;
        }

        private void StaticDataOnOnPlayDialogue(object sender, DialogueStep[] e) {
            StartPlayerDialogue(e);
        }

        private void StartPlayerDialogue(DialogueStep[] dialogue) {
            _currentDialogue = dialogue;
            _currentSteop = 0;
            StartPlayingStep(_currentDialogue[_currentSteop]);
        }

        private void StartPlayingStep(DialogueStep step) {
            _hudDialoguePanel.SetActive(true);
            _txtDiaglogue.text = step.TxtDialogue;
            if(step.SpriteDialogue)_imgDialogue.texture = step.SpriteDialogue.texture;
        }

        private void Update() {
            if (_currentDialogue == null) return;
            //if (_currentDialogue[_currentSteop].UsCameraScroll)
        }
    }
} 