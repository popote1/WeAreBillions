using script;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDEndGamePanel : MonoBehaviour
{
   [SerializeField] private UIAsyncSceneLoader _uiAsyncSceneLoader;
   [Space(10)]
   [SerializeField] private UIFBScoreEndGame _uifbScoreEndZombieCount;
   [SerializeField] private UIFBScoreEndGame _uifbScoreHordeMaxSize;
   [SerializeField] private UIFBScoreEndGame _uifbScoreCiviliansAlive;
   [SerializeField] private UIFBScoreEndGame _uifbScoreDefenderTransform;
   [SerializeField] private UIFBScoreEndGame _uifbScoreBuildingDestroy;
   [SerializeField] private UIFBScoreEndGame _uifbScoreRunTime;
   [SerializeField] private UIFBScoreEndGame _uifbScoreTotalScore;
   
   [SerializeField] private TMP_Text _txtEndGameLable;
   [SerializeField] private Button _bpMainMenu;
   [SerializeField] private Button _bpRestart;
   
   [Space(10)] 
   [SerializeField] private SOEndGameScoringArray soEndGameScoringArray;
   [SerializeField] private UIEndGameScoringElement[] _endGameScoringElements;

   public void OpenEndGamePanel(bool isWin = true) {
      if (isWin) _txtEndGameLable.text = "You Win";
      else _txtEndGameLable.text = "You Lose";
      
      AddEndGameScoring();
      SetupButtons();
      SetUpValues();
      gameObject.SetActive(true);
      if( isWin) StaticScoringSystem.SaveRunData();
   }

   private void SetUpValues() {
      _uifbScoreEndZombieCount.StartDisplaying(StaticData.zombieCount);
      _uifbScoreHordeMaxSize.StartDisplaying(StaticData.zombieMaxCount);
      _uifbScoreCiviliansAlive.StartDisplaying(StaticData.CiviliansCounts);
      _uifbScoreBuildingDestroy.StartDisplaying(StaticData.DestroyBuilding);
      _uifbScoreDefenderTransform.StartDisplaying(StaticData.DefendersKill);
      _uifbScoreRunTime.StartDisplaying(Mathf.RoundToInt(StaticData.GameTimer));
      _uifbScoreTotalScore.StartDisplaying(StaticScoringSystem.CurrentScore);
      
      for (int i = 0; i < soEndGameScoringArray.EndGameScorings.Length; i++) {
         if (soEndGameScoringArray.EndGameScorings[i] == null || _endGameScoringElements[i] == null) return;
         _endGameScoringElements[i].DiplayScoringElement(soEndGameScoringArray.EndGameScorings[i]);
      }
   }
   private void AddEndGameScoring() {
      if (StaticData.SoLevelInfoDataArray!=null&&StaticData.SoLevelInfoDataArray.GetLevelInfoDataBySceneName(SceneManager.GetActiveScene().name) != null) {
         soEndGameScoringArray = StaticData.SoLevelInfoDataArray.GetLevelInfoDataBySceneName(
            SceneManager.GetActiveScene().name).SoEndGameScoring;
      }
      for (int i = 0; i < soEndGameScoringArray.EndGameScorings.Length; i++) {
         if (soEndGameScoringArray.EndGameScorings[i] == null) return;
         StaticScoringSystem.CurrentScore += soEndGameScoringArray.EndGameScorings[i].GetScore().Item2;
      }
   }
   private void SetupButtons() {
      _bpRestart.onClick.AddListener(ClickOnRestart);
      _bpMainMenu.onClick.AddListener(ClickOnMainMenu);
   }

   private void ClickOnRestart() {
      if (_uiAsyncSceneLoader != null) _uiAsyncSceneLoader.StartLoadingScene(SceneManager.GetActiveScene().name);
      else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   private void ClickOnMainMenu() {
      if (_uiAsyncSceneLoader != null) _uiAsyncSceneLoader.StartLoadingScene("MainMenu");
      else SceneManager.LoadScene("0");
   } 
}
