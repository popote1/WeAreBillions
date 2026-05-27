using UnityEngine;

[CreateAssetMenu(fileName = "SoNewAutoQuarterObjectList", menuName = "Scriptable Objects/SoAutoQuarterObjectList")]
public class SoAutoQuarterObjectList : ScriptableObject
{
    public SoObjectList _objectListHouses;
    public SoObjectList _objectListCars;
    public SoObjectList _objectListPalissade;
    public SoObjectList _objectListInsignifiant;
    public SoObjectList _objectListWalkBlocker;
    public SoObjectList _objectListNoColliders;
}