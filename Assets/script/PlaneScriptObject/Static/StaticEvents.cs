using System;
using System.Collections.Generic;
using UnityEngine;

namespace script
{
    public class StaticEvents 
    {
        public static Action OnGameWin;
        public static Action OnGameLose;

        public static Action OnZombieGain;
        public static Action OnZombieLose;

        public static Action OnLoadingComplet;
        
        public static event EventHandler<bool> OnSetGameOnPause;
        public static event EventHandler<DialogueStep[]> OnPlayDialogue;
        public static event EventHandler<List<GridAgent>> OnSelectionChange;
        public static event EventHandler<List<GridAgent>> OnSubmitSelectionChange;
        public static event EventHandler<GridAgent> OnAddAgentToSelection;
        public static event EventHandler<int> OnAlertLevelChange;
    
        public static void ZombieLose() {
            StaticData.ZombieCount--;
            OnZombieLose?.Invoke();
            if( StaticData.ZombieCount<=0) OnGameLose?.Invoke();
        }
        
        public static void SetPause(bool value) {
            if (value) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
        public static void SetGameOnPause() {
            StaticData.IsGamePause = !StaticData.IsGamePause;
            SetPause(StaticData.IsGamePause);
            
            OnSetGameOnPause?.Invoke(new object(), StaticData.IsGamePause);
        }
        public static void SetGameOnPause(bool value) {
            StaticData.IsGamePause = value;
            SetPause(StaticData.IsGamePause);
            OnSetGameOnPause?.Invoke(new object(), StaticData.IsGamePause);
        }
        
        public static void StartPlayingDialogue(DialogueStep[] dialogueSteps) => OnPlayDialogue?.Invoke(null,dialogueSteps);
        public static void SubmitSelection(List<GridAgent> newSelection) => OnSubmitSelectionChange?.Invoke(null, newSelection);
        public static void ChangeSelection(List<GridAgent> selection)=> OnSelectionChange?.Invoke(null, selection);
        public static void AddAgentToSelection(GridAgent agent) => OnAddAgentToSelection?.Invoke(null, agent);
        public static void ChangeAlertLevel(int lvl) => OnAlertLevelChange?.Invoke(null, lvl);

        public static void EndGame(bool isWin) {
            if (isWin) OnGameWin?.Invoke();
            else OnGameLose?.Invoke();
        }
    }
}
