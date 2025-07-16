using System;
using System.Collections;
using System.Collections.Generic;
using script;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private ZombieAgent _prefabsZombieStandardAgent;
    [SerializeField] private ZombieAgent _prefabsZombieBruteAgent;
    [SerializeField] private ZombieAgent _prefabsZombieEngineerAgent;

    

    private void Awake() {
        StaticEvents.OnGameLose += SetGameOnEndMode;
        StaticEvents.OnGameWin += SetGameOnEndMode;
        StaticEvents.OnLoadingComplet+= OnLoadingComplet;
    }

    private void Start() {
        StaticData.SetZombiePrefabs(_prefabsZombieStandardAgent, _prefabsZombieBruteAgent, _prefabsZombieEngineerAgent);
        
        
    }

    private void OnLoadingComplet() {
        StaticData.BlockControls = false;
        StaticData.BlockCameraMovement = false;
        StaticData.IsGameEnd = false;
    }

    private void OnDestroy() {
        StaticEvents.OnGameLose -= SetGameOnEndMode;
        StaticEvents.OnGameWin -= SetGameOnEndMode;  
        StaticEvents.OnLoadingComplet-= OnLoadingComplet;
    }

    void Update() {
        StaticData.ManageGameTimer(Time.deltaTime);
    }
    
    [ContextMenu("TestSave")]
    public void TestSave() {
        StaticSaveSystem.SaveGame();
    }

    public void SetGameOnEndMode() {
        StaticData.BlockControls = true;
        StaticData.BlockCameraMovement = true;
        StaticData.IsGameEnd = true;

    }
}
