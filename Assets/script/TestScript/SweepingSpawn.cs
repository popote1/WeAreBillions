using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SweepingSpawn : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
