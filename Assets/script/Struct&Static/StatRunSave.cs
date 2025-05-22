using System;
using UnityEngine;

[Serializable]
public struct StatRunSave 
{
    public DateTime Date;
    public int Score;
    public int zombieCount;
    public int HordeMaxSize;
    public int CivilliansAlive;
    public int DefenderTrensform;
    public int BuildingDestroy;
    public float Runtime;
}