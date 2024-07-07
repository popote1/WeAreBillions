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
        
        public  enum UniteType {
            Medium, Light, Heavy
        }
    }
}