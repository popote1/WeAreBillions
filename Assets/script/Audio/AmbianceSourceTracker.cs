using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class AmbianceSourceTracker : MonoBehaviour
{
    public Collider Area;
    public float BlendeDistance = 2;
    [SerializeField] private Transform _player;
    [SerializeField] private AudioSource _source;
    

    [Space(10)] 
    public Color DebugColor = Color.green;

    
    

    private void Update() {
        Vector3 closesPoint =Area.ClosestPoint(_player.position);
        float dist = Vector3.Distance(closesPoint, _player.position);
        if ( dist< BlendeDistance) {
            _source.transform.position = Vector3.Lerp(Area.transform.position, closesPoint, 1-dist/BlendeDistance);
            _source.volume = 1 - dist / BlendeDistance;
        }
        else {
            _source.transform.position = Area.transform.position;
            _source.volume = 0;
        }
        
        
    }
}
