using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace script.UIs
{
    public class HUDDialogueManager: MonoBehaviour {
        [SerializeField] private GameObject _hudDialoguePanel;
        [SerializeField] private TMP_Text _txtDiaglogue;
        [SerializeField] private RawImage _imgDialogue;
        [SerializeField] private CameratargetController _cameratargetController;

        private DialogueStep[] _currentDialogue;
        private int _currentStep = 0;

        private float _cameraTravelTime;
        private float _cameraTimer;
        private Vector3 _camStarPos;
        private Vector3 _camBeginingPos;
        private void Start() {
            StaticData.OnPlayDialogue+= StaticDataOnOnPlayDialogue;
            _hudDialoguePanel.SetActive(false);
        }

        private void OnDestroy() {
            StaticData.OnPlayDialogue-= StaticDataOnOnPlayDialogue;
        }

        private void StaticDataOnOnPlayDialogue(object sender, DialogueStep[] e) {
            StartPlayerDialogue(e);
        }

        private void StartPlayerDialogue(DialogueStep[] dialogue) {
            _currentDialogue = dialogue;
            _currentStep = 0;
            _camBeginingPos = _cameratargetController.transform.position;
            StaticData.SetPause(true);
            StaticData.BlockControls = true;
            StartPlayingStep(_currentDialogue[_currentStep]);
        }

        private void StartPlayingStep(DialogueStep step) {
            _hudDialoguePanel.SetActive(true);
            _txtDiaglogue.text = step.TxtDialogue;
            if(step.SpriteDialogue)_imgDialogue.texture = step.SpriteDialogue.texture;
            if (step.UsCameraScroll) StartCameraScroll(step);
        }

        private void StartCameraScroll(DialogueStep step) {
            StaticData.BlockCameraMovement = true;
            if (step.ReturnToBeginingCameraPos) 
                _cameraTravelTime = Vector3.Distance(_camBeginingPos,_cameratargetController.transform.position)/step.ScrollSpeed;
            else 
                _cameraTravelTime = Vector3.Distance(step.EndCameraPosition,_cameratargetController.transform.position)/step.ScrollSpeed;
            
            _camStarPos = _cameratargetController.transform.position;
            _cameraTimer = 0;
        }

        private void Update() {
            if (_currentDialogue == null) return;
            if (_currentDialogue[_currentStep].UsCameraScroll) ManagerCameraScroll();
            if (Input.GetButtonDown("Fire1"))NextDialogueStep();
        }

        private void ManagerCameraScroll() {
            _cameraTimer += Time.unscaledDeltaTime;
            if (_cameraTimer >= _cameraTravelTime) {
                _cameraTimer = _cameraTravelTime;
            }
            if(_currentDialogue[_currentStep].ReturnToBeginingCameraPos )
                _cameratargetController.transform.position =Vector3.Lerp(_camStarPos,_camBeginingPos, _cameraTimer/_cameraTravelTime);
            else
                _cameratargetController.transform.position = Vector3.Lerp(_camStarPos,_currentDialogue[_currentStep].EndCameraPosition, _cameraTimer/_cameraTravelTime);
        }

        private void NextDialogueStep() {
            if (_currentDialogue.Length-1 <=_currentStep) {
                CloseDialogue();
                return;
            }
            _currentStep++;
            StartPlayingStep(_currentDialogue[_currentStep]);
        }

        private void CloseDialogue()
        {
            _currentDialogue = null;
            _hudDialoguePanel.SetActive(false);
            StaticData.BlockCameraMovement = false;
            StaticData.SetPause(false);
            StaticData.BlockControls = false;
        }
    }
}