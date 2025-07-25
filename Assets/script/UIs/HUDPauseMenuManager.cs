using script;
using UnityEngine;

public class HUDPauseMenuManager : MonoBehaviour
{
    [SerializeField] private HUDPauseMenu _hudPauseMenu;
    


    private void Start()
    {
        StaticEvents.OnSetGameOnPause+= StaticDataOnOnSetGameOnPause;
        _hudPauseMenu.OnPanelClose+= HudPauseMenuOnOnPanelClose;
    }

    private void HudPauseMenuOnOnPanelClose() {
        StaticEvents.SetGameOnPause(false);
    }

    private void StaticDataOnOnSetGameOnPause(object sender, bool e) {
        if( e)OpenPauseMenu();
        else ClosePauseMenu();
    }
    public void OpenPauseMenu() {
        
        _hudPauseMenu.Open();
        
    }
    private void OnDestroy() {
        StaticEvents.OnSetGameOnPause-= StaticDataOnOnSetGameOnPause;
    }

    public void ClosePauseMenu() {
        _hudPauseMenu.ForceClose();
    }
}