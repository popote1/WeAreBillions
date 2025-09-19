using UnityEngine;

[CreateAssetMenu(fileName = "SoNewObjectList",menuName = "Scriptable Objects/SoObjectList")]
public class SoObjectList : ScriptableObject {
   [SerializeField] public GameObject[] _objects;
   public GameObject GetRandomObject() {
      return _objects[Random.Range(0, _objects.Length)];
   }

}
