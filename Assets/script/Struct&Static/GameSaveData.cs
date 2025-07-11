using System;
using UnityEngine;

[Serializable]
public struct GameSaveData
{
    public bool IsSet;
    //Audio
    public float AudioMasterVolume;
    public float AudioMusicVolume;
    public float AudioSFXVolume;
    public float AudioAmbianceVolume;
    //Control
    public float CameraPanningSpeed;
    public float CameraKeybordSpeed;
    //GamePlay
    public bool AllowCheatMenu;

    public LevelSaveData[] LevelsSaveData;

    public LevelSaveData GetLevelSAveDataBySceneName(string name) {

        if (LevelsSaveData != null) { 
            foreach (var save in LevelsSaveData) {
                if (save.SceneName != name) continue;
                Debug.Log("LevelSaveData found");
                return save;
            }
        }
        Debug.Log("LevelSaveData NOT found");
        return new LevelSaveData("");
    }

}