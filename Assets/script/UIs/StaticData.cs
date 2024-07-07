using System;
using System.Collections.Generic;

namespace script {
    public static class StaticData {
        public static Action OnGameWin;
        public static Action OnGameLose;

        public static Action OnZombieGain;
        public static Action OnZombieLose;

        public static int ZombieCount;

        public static List<ZombieAgent> AllZombies {
            get => _allZombieAgents;
        }

        private static List<ZombieAgent> _allZombieAgents;

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
    }
}