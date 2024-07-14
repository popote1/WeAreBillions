using System.Collections;
using System.Collections.Generic;
using script;
using UnityEngine;

public class GameManager : MonoBehaviour {
    void Update() {
        StaticData.ManageGameTimer(Time.deltaTime);
    }
}
