using System.IO;
using script;
using UnityEngine;

public static class StaticSaveSystem {
    public static GameSaveData _currentSave;


    public static void LoadGame() {
        
        string path = Application.persistentDataPath + "/Save/Save.txt";
        
        if (!File.Exists(path)) {
            Debug.LogWarning("Fichier de sauvegarde non trouver");
        }
        string data =File.ReadAllText(path);
        _currentSave = JsonUtility.FromJson<GameSaveData>(data);
        Debug.Log("Nouvelle Save Charger");
    }
    public static void SaveGame() {
        if (!_currentSave.IsSet) GenerateBaseSaveFile();

        if (!Directory.Exists(Application.persistentDataPath + "/Save")) {
            Debug.Log("Création du dossier de Save");
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");
        }
        
        string path = Application.persistentDataPath + "/Save/Save.txt";
        Debug.Log("Save à l'adresse "+path);
        string data = JsonUtility.ToJson(_currentSave);

        if (!File.Exists(path)) {
            Debug.Log("écriture d'un nouveau fichier de save");
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Write(data);
            }
        }
        else {
            Debug.Log("Modification du fichier de sauvegarde");
            File.WriteAllText(path, data);
        }
    }

    
    
    public static void GenerateBaseSaveFile() {
        Debug.Log("Génération de nouvelles datas de sauvegarde");
        GameSaveData newSaveData = new GameSaveData();
        newSaveData.AudioAmbianceVolume = StaticData.AudioVolumeAmbiances;
        newSaveData.AudioMasterVolume = StaticData.AudioVolumeMaster;
        newSaveData.AudioMusicVolume = StaticData.AudioVolumeMusic;
        newSaveData.AudioSFXVolume = StaticData.AudioVolumeSFX;
        
        newSaveData.CameraKeybordSpeed = StaticData.ControlCameraKeyboardSpeed;
        newSaveData.CameraPanningSpeed = StaticData.ControlCameraPanningSpeed;

        newSaveData.AllowCheatMenu = StaticData.GamePlayAllowCheatMenu;

        newSaveData.LevelsSaveData = new LevelSaveData[4];
        for (int i = 0; i <  newSaveData.LevelsSaveData.Length; i++) {
            newSaveData.LevelsSaveData[i].IsUnlock = false;
            newSaveData.LevelsSaveData[i].BestRun = new StatRunSave[5];
            newSaveData.LevelsSaveData[i].BestStats = new StatRunSave();
        }

        newSaveData.IsSet = true;
        _currentSave = newSaveData;

    }

    public static void ApplyCurrentSaves() {
        StaticData.AudioVolumeAmbiances = _currentSave.AudioAmbianceVolume;
        StaticData.AudioVolumeMaster = _currentSave.AudioMasterVolume;
        StaticData.AudioVolumeMusic = _currentSave.AudioMusicVolume;
        StaticData.AudioVolumeSFX = _currentSave.AudioSFXVolume;
        
        StaticData.ControlCameraKeyboardSpeed = _currentSave.CameraKeybordSpeed;
        StaticData.ControlCameraPanningSpeed = _currentSave.CameraPanningSpeed;

        StaticData.GamePlayAllowCheatMenu = _currentSave.AllowCheatMenu;
        StaticData.OnOptionUpdateInvoke();
    }
}