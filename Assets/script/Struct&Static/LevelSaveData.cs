using System;

[Serializable]
public struct LevelSaveData {
    public bool IsUnlock;
    public StatRunSave BestStats;
    public StatRunSave[] BestRun;
}