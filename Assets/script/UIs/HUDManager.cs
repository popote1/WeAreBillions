using System;
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
} 