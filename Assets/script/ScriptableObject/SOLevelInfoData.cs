using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "SOLevelInfoData", menuName = "Scriptable Objects/SOLevelInfoData")]
public class SOLevelInfoData : ScriptableObject
{
    public string MenuName;
    public LocalizedString LocalizedMenuName;
    public string Subtile;
    public LocalizedString LocalizedSubtile;
    public LocalizedString LocalizedDescription;
    [TextArea]public string Description;
    public Sprite Illustration;
    public string SceneName;
    public SOEndGameScoringArray SoEndGameScoring;
    public float _standardZombieSpawnChance = 1;
    public float _bruteZombieSpawnChance = 1;
    public float _engineerZombieSpawnChance = 1;

    public string GetMenuName=> LocalizedMenuName.GetLocalizedString();
    public string GetSubTitle=> LocalizedSubtile.GetLocalizedString();
    public string GetDescription=> LocalizedDescription.GetLocalizedString();
    
}