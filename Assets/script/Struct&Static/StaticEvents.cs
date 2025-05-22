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
    
        public static event EventHandler<bool> OnSetGameOnPause;
        public static event EventHandler<DialogueStep[]> OnPlayDialogue;
        public static event EventHandler<List<GridAgent>> OnSelectionChange;
        public static event EventHandler<List<GridAgent>> OnSubmitSelectionChange;
        
    
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
            StaticData.GamePause = !StaticData.GamePause;
            SetPause(StaticData.GamePause);
            
            OnSetGameOnPause?.Invoke(new object(), StaticData.GamePause);
        }
        public static void SetGameOnPause(bool value) {
            StaticData.GamePause = value;
            SetPause(StaticData.GamePause);
            OnSetGameOnPause?.Invoke(new object(), StaticData.GamePause);
        }
        
        public static void StartPlayingDialogue(DialogueStep[] dialogueSteps) {
            OnPlayDialogue?.Invoke(null,dialogueSteps);
        }

        public static void SubmitSelection(List<GridAgent> newSelection)
        {
            OnSubmitSelectionChange?.Invoke(null, newSelection);
        }

        public static void ChangeSelection(List<GridAgent> selection)
        {
            OnSelectionChange?.Invoke(null, selection);
        }

        public static void EndGame(bool isWin) {
            if (isWin) OnGameWin?.Invoke();
            else OnGameLose?.Invoke();
        }
    }
}
