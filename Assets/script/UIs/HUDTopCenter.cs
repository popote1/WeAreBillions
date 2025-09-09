using UnityEngine;
using UnityEngine.UI;

namespace script.UIs
{
    public class HUDTopCenter : MonoBehaviour {
        [SerializeField] private Image _imgTransformationBar;
        [SerializeField] private Image _imgAlertBar;
        [SerializeField] private Transform _starHolder;

        private AlertSystemManager _alertSystemManager;

        private void Start()
        {
            _alertSystemManager= AlertSystemManager.Instance;
            if (_alertSystemManager == null) {
                Debug.LogWarning("Alert System not found", this);
            }
            StaticEvents.OnAlertLevelChange+= AlertSystemManagerOnOnAlertLevelChange;
            AlertSystemManagerOnOnAlertLevelChange(this ,0);
        }

        private void AlertSystemManagerOnOnAlertLevelChange(object sender, int e) {
            for (int i = 0; i < _starHolder.childCount; i++) {
                _starHolder.GetChild(i).gameObject.SetActive(i<e);
            }
        }

        private void Update()
        {
            ManageZombieTransformrtionBar();
            ManageAlertData();
        }

        private void ManageZombieTransformrtionBar() {
            _imgTransformationBar.fillAmount = StaticData.ZombieCount / (float)StaticData.GridAgentsCounts;
        }

        private void ManageAlertData() {
            if (_alertSystemManager != null) {
                _imgAlertBar.fillAmount = _alertSystemManager.GetAlertProgress();
            }
        }
    }
}