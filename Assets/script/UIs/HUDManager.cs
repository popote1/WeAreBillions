using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace script.UIs
{
    public class HUDManager :MonoBehaviour
    {
        //public TMP_Text TxtZombieCount;
        //public GameObject PanelWin;
        //public GameObject PanelLose;
        [SerializeField] private Button _bpMenuPause;
        [SerializeField] private HUDEndGamePanel _hudEndGamePanel;
        

        private void Start() {
            //StaticData.OnZombieGain += SetZombieValue;
            //StaticData.OnZombieLose += SetZombieValue;
            StaticEvents.OnGameWin += PlayWin;
            StaticEvents.OnGameLose += PlayLose;
            _bpMenuPause.onClick.AddListener(ClickOnPauseButton);
        }

        private void OnDestroy()
        {
            //StaticData.OnZombieGain -= SetZombieValue;
            //StaticData.OnZombieLose -= SetZombieValue;
            StaticEvents.OnGameWin -= PlayWin;
            StaticEvents.OnGameLose -= PlayLose;
        }

        //public void SetZombieValue() {
        //    TxtZombieCount.text = StaticData.ZombieCount.ToString();
        //}

        public void PlayWin() {
            _hudEndGamePanel.OpenEndGamePanel();
        }

        public void PlayLose() {
            _hudEndGamePanel.OpenEndGamePanel(false);
        }

        public void ReturnToMainMenu() {
            GridManager.Instance = null;
            SceneManager.LoadScene(0);
        }
        
        private void ClickOnPauseButton() {
            StaticEvents.SetGameOnPause(true);
        }
        
    }
} 