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
        public static bool BlockControls = false;
        public static bool GamePause = false;

        public static float AudioVolumeMaster = 1;
        public static float AudioVolumeMusic = 1;
        public static float AudioVolumeSFX = 1;
        public static float AudioVolumeAmbiances = 0.5f;

        public static List<ZombieAgent> AllZombies=> _allZombieAgents;
        public static int GridAgentsCounts { get {
                if (_allGridAgents == null) return 0;
                return _allGridAgents.Count;
            }
        }
        public static int zombieCount => _zombieCount;
        public static int zombieMaxCount => _zombieMaxCount;
        public static int CiviliansCounts => _civiliansCounts;
        public static int DefendersCount => _defendersCount;
        public static int DefendersKill => _defendersKill;
        public static int BuildingsCount => _buildingsCounts;
        public static int DestroyBuilding => _destroyBuildingsCounts;
        public static int DestroyElements => _destroyElements;
        public static int CurrentAlertLvl = _currentAlertLVL;
        public static int AlertMaxLevel = _alertMaxLVL;
        public static float GameTimer => _gameTimer;
        public static float TimeInAlertLevel1 => _timeInAlertLVL1;
        public static float TimeInAlertLevel2 => _timeInAlertLVL2;
        public static float TimeInAlertLevel3 => _timeInAlertLVL3;
        public static float TimeInAlertLevel4 => _timeInAlertLVL4;
        public static float TimeInAlertLevel5 => _timeInAlertLVL5;
        



        private static List<ZombieAgent> _allZombieAgents;
        private static List<GridAgent> _allGridAgents;
        private static List<House> _allHouses;
        private static int _zombieCount = 0;
        private static int _zombieMaxCount = 0;
        private static int _civiliansCounts = 0;
        private static int _defendersKill = 0; 
        private static int _defendersCount = 0;
        private static int _buildingsCounts = 0;
        private static int _destroyBuildingsCounts = 0;
        private static int _destroyElements = 0;
        private static int _currentAlertLVL = 0;
        private static int _alertMaxLVL = 0;
        private static float _gameTimer = 0;
        private static float _timeInAlertLVL1 = 0;
        private static float _timeInAlertLVL2 = 0;
        private static float _timeInAlertLVL3 = 0;
        private static float _timeInAlertLVL4 = 0;
        private static float _timeInAlertLVL5 = 0;

        public static event EventHandler<bool> OnSetGameOnPause;
        public static event EventHandler<DialogueStep[]> OnPlayDialogue;

        public static string GetGameTime() {
            int sec = Mathf.FloorToInt(_gameTimer % 60);
            int minute = Mathf.FloorToInt((_gameTimer/60) % 60);
            return minute + " Min  " + sec + " Sec";
        }
        
        public static void HouseDestroy() =>_destroyBuildingsCounts++;
        public static void DestroyElement() => _destroyElements++; 
        
        public static void SetGameOnPause() {
            GamePause = !GamePause;
            SetPause(GamePause);
            
            OnSetGameOnPause?.Invoke(new object(), GamePause);
        }
        public static void SetGameOnPause(bool value) {
            GamePause = value;
            SetPause(GamePause);
            OnSetGameOnPause?.Invoke(new object(), GamePause);
        }

        public static void SetCurrentAlertLevel(int lvl)
        {
            _currentAlertLVL = lvl;
            if (_alertMaxLVL < _currentAlertLVL) _alertMaxLVL = _currentAlertLVL;
        }

        public static void StartPlayingDialogue(DialogueStep[] dialogueSteps) {
            OnPlayDialogue?.Invoke(null,dialogueSteps);
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
            switch (_currentAlertLVL) {
                case 1 : _timeInAlertLVL1 += deltaTime; break;
                case 2 : _timeInAlertLVL2 += deltaTime; break;
                case 3 : _timeInAlertLVL3 += deltaTime; break;
                case 4 : _timeInAlertLVL4 += deltaTime; break;
                case 5 : _timeInAlertLVL5 += deltaTime; break;
            }
        }

        public static void AddDefenderKill()
        {
            _defendersKill++;
        }


        private static void ManagerGridAgentsChange(GridAgent gridAgent, bool added) {
            int change = -1;
            if (added) change =  +1;

            if (gridAgent is ZombieAgent) _zombieCount += change;
            else if (gridAgent is Defender) _defendersCount += change;
            else if (gridAgent is CivillianAgent) _civiliansCounts += change;

            if (_zombieCount > _zombieMaxCount) _zombieMaxCount = _zombieCount;
        }
    }
}