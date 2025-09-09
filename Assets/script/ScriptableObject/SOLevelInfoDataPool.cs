using UnityEngine;

[CreateAssetMenu(fileName = "newSOLevelInfoDataArray",menuName = "Scriptable Objects/SOLevelInfoDataArray" )]
    public class SOLevelInfoDataArray : ScriptableObject {
        public SOLevelInfoData[] Levels;

        public SOLevelInfoData GetLevelInfoDataBySceneName(string name) {
            foreach (var level in Levels) {
                if (level == null || level.SceneName != name) continue;
                return level;
            }
            return null;
        }
    }
