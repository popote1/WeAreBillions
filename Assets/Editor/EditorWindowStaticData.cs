using script;
using UnityEditor;
using UnityEngine;

public class EditorWindowStaticData : EditorWindow
{
    private Vector2 scrollPosition;
    [MenuItem("PopoteTools/StaticData")]
    public static void ShowWindow() {
        EditorWindowStaticData window = GetWindow<EditorWindowStaticData>();
        window.titleContent = new GUIContent(" Static Data");
    }

    private void OnGUI() {
        
        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition );

        //GUILayout.BeginScrollView(Vector2.one);
        DisplayInfo("Zombie Count", StaticData.zombieCount.ToString());
        DisplayInfo("ZombieMaxCount", StaticData.zombieMaxCount.ToString());
        DisplayInfo("Civilians", StaticData.CiviliansCounts.ToString());
        DisplayInfo("CiviliansKills", StaticData.CiviliansKills.ToString());
        DisplayInfo("Defenders", StaticData.DefendersCount.ToString());
        DisplayInfo("DefendersKills", StaticData.DefendersKill.ToString());
        DisplayInfo("BuildingDestroy", StaticData.DestroyBuilding.ToString());
        DisplayInfo("BuildingDestroy", StaticData.DestroyBuilding.ToString());
        DisplayInfo("DestroyElement", StaticData.DestroyElements.ToString());
        GUILayout.Space(10);
        DisplayInfo("CurrentAlertLevel", StaticData.DefendersCount.ToString());
        DisplayInfo("AlertMaxLvl", StaticData.AlertMaxLevel.ToString());
        GUILayout.Space(10);
        DisplayInfo("GameTime", StaticData.GameTimer.ToString());
        DisplayInfo("Time In Alert Level 1", StaticData.TimeInAlertLevel1.ToString());
        DisplayInfo("Time In Alert Level 2", StaticData.TimeInAlertLevel2.ToString());
        DisplayInfo("Time In Alert Level 3", StaticData.TimeInAlertLevel3.ToString());
        DisplayInfo("Time In Alert Level 4", StaticData.TimeInAlertLevel4.ToString());
        DisplayInfo("Time In Alert Level 5", StaticData.TimeInAlertLevel5.ToString());
        GUILayout.EndScrollView();
                
    }

    private void DisplayInfo(string label, string data) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        GUILayout.Label(data);
        GUILayout.EndHorizontal();
    }
}
