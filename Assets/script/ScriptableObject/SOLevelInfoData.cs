using UnityEngine;

[CreateAssetMenu(fileName = "SOLevelInfoData", menuName = "Scriptable Objects/SOLevelInfoData")]
public class SOLevelInfoData : ScriptableObject
{
    public string MenuName;
    public string Subtile;
    [TextArea]public string Description;
    public Sprite Illustration;
    public string SceneName;
    public SOEndGameScoringArray SoEndGameScoring;
    public float _standardZombieSpawnChance = 1;
    public float _bruteZombieSpawnChance = 1;
    public float _engineerZombieSpawnChance = 1;
}