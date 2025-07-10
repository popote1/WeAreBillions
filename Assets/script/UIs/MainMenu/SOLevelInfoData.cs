using UnityEngine;

[CreateAssetMenu(fileName = "SOLevelInfoData", menuName = "Scriptable Objects/SOLevelInfoData")]
public class SOLevelInfoData : ScriptableObject
{
    public string MenuName;
    public string Subtile;
    [TextArea]public string Description;
    public Sprite Illustration;
    public string SceneName;
}
