using System;

[Serializable]
public struct StatRunSave : IComparable
{
    public long Date;
    public int Score;
    public int zombieCount;
    public int HordeMaxSize;
    public int CivilliansAlive;
    public int DefenderTrensform;
    public int BuildingDestroy;
    public float Runtime;
    
    public int CompareTo(object obj) {
        if (obj == null) return 1;
        StatRunSave otherSave = (StatRunSave)obj ;
        return this.Score.CompareTo(otherSave.Score);
    }
}