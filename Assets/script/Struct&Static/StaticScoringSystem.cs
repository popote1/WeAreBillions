using System;
using script;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticScoringSystem {
   public static event EventHandler<int> OnScoreChange;
   public static int CurrentScore {
      get => _currentScoring;
      set {
         _currentScoring = value;
         OnScoreChange?.Invoke(null, _currentScoring);
      }
   }
   private static int _currentScoring;

   private static float AlertMod {
      get {
         switch (StaticData.CurrentAlertLvl) {
            case 0 : return Metrics.SMAlertLvl0;
            case 1 : return Metrics.SMAlertLvl1;
            case 2 : return Metrics.SMAlertLvl2;
            case 3 : return Metrics.SMAlertLvl3;
            case 4 : return Metrics.SMAlertLvl4;
            case 5 : return Metrics.SMAlertLvl5;
            default:return 1;
         }
      }
   }
   
   public static void AddCiviliansKill()=> CurrentScore += Mathf.FloorToInt(Metrics.SVCiviliansKill *AlertMod);
   public static void AddDefenderKill()=> CurrentScore += Mathf.FloorToInt(Metrics.SVDefenderKill *AlertMod);
   public static void AddBuildingDestroy()=> CurrentScore += Mathf.FloorToInt(Metrics.SVBuildingKill *AlertMod);

   public static void SaveRunData() {
      StatRunSave save = new StatRunSave();
      save.Score = CurrentScore;
      save.zombieCount = StaticData.ZombieCount;
      save.HordeMaxSize = StaticData.zombieMaxCount;
      save.CivilliansAlive = StaticData.CiviliansCounts;
      save.DefenderTrensform = StaticData.DefendersKill;
      save.BuildingDestroy = StaticData.BuildingsCount;
      save.Runtime = StaticData.GameTimer;
      save.Date = DateTime.Now.ToBinary();
      
      Debug.Log(" nouvelle run save a la date = "+ DateTime.Now +"\n ou dans la save ="+save.Date);
      
      StaticSaveSystem.SaveNewRunData(SceneManager.GetActiveScene().name, save);
      
   }
   
}
