using script;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDEndGamePanel : MonoBehaviour
{
   [SerializeField] private TMP_Text _txtEndGameLable;
   [SerializeField] private TMP_Text _txtZombieCount;
   [SerializeField] private TMP_Text _txtHordeMaxSize;
   [SerializeField] private TMP_Text _txtCivilansAlive;
   [SerializeField] private TMP_Text _txtDefenderTransform;
   [SerializeField] private TMP_Text _txtBuildingDestroy;
   [SerializeField] private TMP_Text _txtRunTime;
   [SerializeField] private Button _bpMainMenu;
   [SerializeField] private Button _bpRestart;

   public void OpenEndGamePanel(bool isWin = true) {
      if (isWin) _txtEndGameLable.text = "You Win";
      else _txtEndGameLable.text = "You Lose";
      
      SetupButtons();
      SetUpValues();
      gameObject.SetActive(true);
   }

   private void SetUpValues() {
      _txtZombieCount.text = StaticData.zombieCount.ToString();
      _txtHordeMaxSize.text = StaticData.zombieMaxCount.ToString();
      _txtCivilansAlive.text = StaticData.CiviliansCounts.ToString();
      _txtDefenderTransform.text = StaticData.DefendersKill.ToString();
      _txtBuildingDestroy.text = StaticData.DestroyBuilding.ToString();
      _txtRunTime.text = StaticData.GetGameTime();
   }
   

   private void SetupButtons() {
      _bpRestart.onClick.AddListener(ClickOnRestart);
      _bpMainMenu.onClick.AddListener(ClickOnMainMenu);
   }

   private void ClickOnRestart() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   private void ClickOnMainMenu() {
      SceneManager.LoadScene(0);
   } 
}
