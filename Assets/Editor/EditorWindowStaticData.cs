using script;
using UnityEditor;
using UnityEngine;

public class EditorWindowStaticData : EditorWindow
{
    private Vector2 scrollPosition;
    private float _lineHeight = 20;
    private float _labelwidth = 150;
    private float _contentWidth = 30;
    [MenuItem("PopoteTools/StaticData")]
    public static void ShowWindow() {
        EditorWindowStaticData window = GetWindow<EditorWindowStaticData>();
        window.titleContent = new GUIContent(" Static Data");
    }

    
    private void OnGUI() {
        Vector2 pos = Vector2.zero;
        DisplayInfoRect("Is Game End", StaticData.IsGameEnd.ToString(),ref pos);
        DisplayInfoRect("Is Game Pause", StaticData.IsGamePause.ToString(),ref pos);
        DisplayInfoRect("Is Controls Blocks", StaticData.BlockControls.ToString(),ref pos);
        pos.y += 10;
        DisplayInfoRect("Camera Move Vector", StaticData.CameraMoveVector.ToString(),ref pos); 
        DisplayInfoRect("Camera Move Block", StaticData.BlockCameraMovement.ToString(),ref pos);
        DisplayInfoRect("Camera Allow height", StaticData.AllowCameraHeight.ToString(),ref pos);
        pos.y += 10;
        DisplayInfoRect("Cheat Menu Active", StaticData.GamePlayAllowCheatMenu.ToString(),ref pos);
        DisplayInfoRect("Cheat Allow Zombie Spawn", StaticData.CheatEnableZombieSpawning.ToString(),ref pos);
        DisplayInfoRect("Is Draging", StaticData.IsDraging.ToString(),ref pos);
        
        
        pos.x += _contentWidth + _labelwidth + 20;
        pos.y = 0;
        //GUILayout.BeginScrollView(Vector2.one);
        DisplayInfoRect("Zombie Count", StaticData.zombieCount.ToString(),ref pos);
        DisplayInfoRect("ZombieMaxCount", StaticData.zombieMaxCount.ToString(),ref pos);
        DisplayInfoRect("Civilians", StaticData.CiviliansCounts.ToString(),ref pos);
        DisplayInfoRect("CiviliansKills", StaticData.CiviliansKills.ToString(),ref pos);
        DisplayInfoRect("Defenders", StaticData.DefendersCount.ToString(),ref pos);
        DisplayInfoRect("DefendersKills", StaticData.DefendersKill.ToString(),ref pos);
        DisplayInfoRect("BuildingDestroy", StaticData.DestroyBuilding.ToString(),ref pos);
        DisplayInfoRect("BuildingDestroy", StaticData.DestroyBuilding.ToString(),ref pos);
        DisplayInfoRect("DestroyElement", StaticData.DestroyElements.ToString(),ref pos);
        pos.y += 10;
        DisplayInfoRect("CurrentAlertLevel", StaticData.DefendersCount.ToString(),ref pos);
        DisplayInfoRect("AlertMaxLvl", StaticData.AlertMaxLevel.ToString(),ref pos);
        pos.y += 10;
        DisplayInfoRect("GameTime", StaticData.GameTimer.ToString(),ref pos);
        DisplayInfoRect("Time In Alert Level 1", StaticData.TimeInAlertLevel1.ToString(),ref pos);
        DisplayInfoRect("Time In Alert Level 2", StaticData.TimeInAlertLevel2.ToString(),ref pos);
        DisplayInfoRect("Time In Alert Level 3", StaticData.TimeInAlertLevel3.ToString(),ref pos);
        DisplayInfoRect("Time In Alert Level 4", StaticData.TimeInAlertLevel4.ToString(),ref pos);
        DisplayInfoRect("Time In Alert Level 5", StaticData.TimeInAlertLevel5.ToString(),ref pos);
                
    }

    private void DisplayInfo(string label, string data) {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label(label);
        GUILayout.Label(data);
        GUILayout.EndHorizontal();
    }
    private void DisplayInfoRect(string label, string data, ref Vector2 pos) {
        GUILayout.BeginHorizontal();
        Rect labelRec = new Rect(pos, new Vector2(_labelwidth, _lineHeight));
        Rect ContentRec = new Rect(pos+new Vector2(_labelwidth, 0), new Vector2(_contentWidth, _lineHeight));
        GUI.Label(labelRec, new GUIContent(label));
        GUI.Label(ContentRec, new GUIContent(data));
        pos.y += _lineHeight;
        GUILayout.EndHorizontal();
    }
}
