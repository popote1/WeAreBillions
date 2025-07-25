using System;
using System.Collections.Generic;
using System.Linq;
using script;
using UnityEngine;

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
        BestRun = new StatRunSave[Metrics.MaxSaveRunPerLevel];
        BestStats.Runtime = Mathf.Infinity;
    }

    public void AddNewRunSave(StatRunSave save) {
        UpdateBestRunSave(save);
        AddNewRun(save);
    }

    private void AddNewRun(StatRunSave save) {
        if(! CanAddSaveRun(save))return;
        List<StatRunSave> saves = BestRun.ToList();
        saves.Add(save);
        saves.Sort();
        for (int i = 0; i < 4; i++)
        {
            if (i >=saves.Count) break;
            BestRun[i] = saves[saves.Count - (1 + i)];
        }
    }

    private bool CanAddSaveRun(StatRunSave save) {
        int SaveCount = 0;
        int lowerScore = Int32.MaxValue;
        foreach (var run in BestRun) {
            if (run.Score > 0) {
                SaveCount++;
                if (lowerScore > run.Score) {
                    lowerScore = run.Score;
                }
            }
        }
        if (SaveCount < Metrics.MaxSaveRunPerLevel) return true;
        return (save.Score > lowerScore);
    }

    private void UpdateBestRunSave(StatRunSave save) {
        if (BestStats.Score < save.Score) BestStats.Score = save.Score;
        if (BestStats.zombieCount < save.zombieCount) BestStats.zombieCount = save.zombieCount;
        if (BestStats.HordeMaxSize < save.HordeMaxSize) BestStats.HordeMaxSize = save.HordeMaxSize;
        if (BestStats.CivilliansAlive < save.CivilliansAlive) BestStats.CivilliansAlive = save.CivilliansAlive;
        if (BestStats.BuildingDestroy < save.BuildingDestroy) BestStats.BuildingDestroy = save.BuildingDestroy;
        if (BestStats.DefenderTrensform < save.DefenderTrensform) BestStats.DefenderTrensform = save.DefenderTrensform;
        if (BestStats.Runtime > save.Runtime||BestStats.Runtime==0) BestStats.Runtime = save.Runtime;
    }

    
    
    
    
    
    
    
    
}