using System;

[Serializable]
public struct LevelSaveData{ 
    public string SceneName;
    public bool IsUnlock;
    public StatRunSave BestStats;
    public StatRunSave[] BestRun;
}