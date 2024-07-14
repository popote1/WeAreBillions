using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;


namespace script {
    public static class StaticData {
        public static Action OnGameWin;
        public static Action OnGameLose;

        public static Action OnZombieGain;
        public static Action OnZombieLose;

        public static int ZombieCount;
        
        public static Vector3 CameraMoveVector = Vector3.zero;
        public static bool BlockCameraMovement = false;
        public static bool AllowCameraHeight = true;
        public static bool GamePause = false;

        public static List<ZombieAgent> AllZombies=> _allZombieAgents;
        public static int zombieCount => _zombieCount;
        public static int CiviliansCounts => _civiliansCounts;
        public static int DefendersCount => _defendersCount;
        public static int BuildingsCount => _buildingsCounts;
        public static int DestroyBuilding => _destroyBuildingsCounts;
        public static int DestroyElements => _destroyElements;
        public static float GameTimer => _gameTimer;



        private static List<ZombieAgent> _allZombieAgents;
        private static List<GridAgent> _allGridAgents;
        private static List<House> _allHouses;
        private static int _zombieCount = 0;
        private static int _civiliansCounts = 0;
        private static int _defendersCount = 0;
        private static int _buildingsCounts = 0;
        private static int _destroyBuildingsCounts = 0;
        private static int _destroyElements = 0;
        private static float _gameTimer = 0;

        public static event EventHandler OnPressEscape;

        public static void HouseDestroy() =>_destroyBuildingsCounts++;
        public static void DestroyElement() => _destroyElements++; 
        
        public static void PressEscape(object sender) {
            OnPressEscape?.Invoke(sender, EventArgs.Empty);
            GamePause = !GamePause;
            Debug.Log("Games Pause ="+GamePause);
            SetPause(GamePause);
        }

        public static void ZombieLose() {
            ZombieCount--;
            OnZombieLose?.Invoke();
            if( ZombieCount<=0) OnGameLose?.Invoke();
        }

        public static void AddZombie(ZombieAgent zombie) {
            if (_allZombieAgents == null) _allZombieAgents = new List<ZombieAgent>();
            _allZombieAgents.Add(zombie);
        }

        public static void RemoveZombie(ZombieAgent zombie) {
            _allZombieAgents.Remove(zombie);
        }

        public static void AddGridAgent(GridAgent gridAgent) {
            if (_allGridAgents == null) _allGridAgents = new List<GridAgent>();
            _allGridAgents.Add(gridAgent);
            ManagerGridAgentsChange(gridAgent, true);
        }

        public static void RemoveGridAgent(GridAgent gridAgent) {
            _allGridAgents.Remove(gridAgent);
            ManagerGridAgentsChange(gridAgent , false);
        }

        public static void AddBuilding(House house) {
            if (_allHouses == null) _allHouses = new List<House>();
            _allHouses.Add(house);
            _buildingsCounts++;
        }

        public static void SetPause(bool value) {
            if (value) Time.timeScale = 0;
            else Time.timeScale = 1;
        }

        public static void ManageGameTimer(float deltaTime) {
            _gameTimer += deltaTime;
        }


        private static void ManagerGridAgentsChange(GridAgent gridAgent, bool added) {
            int change = -1;
            if (added) change =  +1;

            if (gridAgent is ZombieAgent) _zombieCount += change;
            else if (gridAgent is Defender) _defendersCount += change;
            else if (gridAgent is CivillianAgent) _civiliansCounts += change;
        }
    }
}