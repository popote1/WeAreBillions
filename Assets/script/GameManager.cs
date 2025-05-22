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


    private void Start() {
        StaticData.SetZombiePrefabs(_prefabsZombieStandardAgent, _prefabsZombieBruteAgent, _prefabsZombieEngineerAgent);
    }

    void Update() {
        StaticData.ManageGameTimer(Time.deltaTime);
    }
    
    [ContextMenu("TestSave")]
    public void TestSave() {
        StaticSaveSystem.SaveGame();
    }
}
