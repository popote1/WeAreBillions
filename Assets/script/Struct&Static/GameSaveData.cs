using System;

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

}