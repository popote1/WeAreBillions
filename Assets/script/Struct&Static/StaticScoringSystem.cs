using System;
using script;
using UnityEngine;

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
   public static void AddDefenderKill()=> CurrentScore += Mathf.FloorToInt(Metrics.SVCiviliansKill *AlertMod);
   public static void AddBuildingDestroy()=> CurrentScore += Mathf.FloorToInt(Metrics.SVCiviliansKill *AlertMod);
   
}
