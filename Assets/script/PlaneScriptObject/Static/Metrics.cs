using System;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using UnityEngine;

namespace script
{
    public static class Metrics {
        public const int chunkSize = 10;

        public static Vector3 cellColliderBockSize = new Vector3(0.5f, 20, 0.5f);
        public static bool UsDebugCellGroundOffsetting = true;
        //FlowField Parameters
        public const int FlowFieldCellPerFrame = 2000;
        public const int MoveCost=10;
        public const int DiagonalMoveCost = 14;
        public const int FreeWalkMoveCos = 1;
        public const int BlockMoveCostMoveCost = 1000;
        public const int DestructibleMoveCost = 20;

        public const float ZombieSpawnOffset =  0.5f;
        
        // Scoring Parameters
        public const int SVCiviliansKill = 10;
        public const int SVDefenderKill = 15;
        public const int SVBuildingKill = 25;

        public const int SMAlertLvl0 = 1;
        public const float SMAlertLvl1 = 1.25f;
        public const float SMAlertLvl2 = 1.5f;
        public const int SMAlertLvl3 = 2;
        public const float SMAlertLvl4 = 2.5f;
        public const int SMAlertLvl5 = 3;
        
        //SaveSystem const
        public const int MaxSaveRunPerLevel =5;
        
        public  enum UniteType {
            Medium, Light, Heavy
        }
        public enum ConditionOperator {
            Greater, GreaterOrEqual, Equal, SmallerOrEqual, Smaller 
        }
        
    }
}