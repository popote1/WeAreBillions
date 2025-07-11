using System;

[Serializable]
public struct LevelSaveData{ 
    public string SceneName;
    public bool IsUnlock;
    public StatRunSave BestStats;
    public StatRunSave[] BestRun ;

    public LevelSaveData(string sceneName ) {
        SceneName = sceneName;
        IsUnlock = false;
        BestStats = new StatRunSave();
        BestRun = new StatRunSave[0];
    }
}