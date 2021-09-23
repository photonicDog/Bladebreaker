using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityAI : SerializedMonoBehaviour {
    

    private Vector3 startPosition;
    
    [FoldoutGroup("AI Basics")]
    public Vector3 wanderRadiusCenter;
    public float wanderRadiusBounds;
    
    public Vector3 leashRadiusCenter;
    public float leashRadiusBounds;

    public float detectionRadius;

    
    public bool logicEnabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Wait(float time) {
        
    }

    void Attack() {
        
    }

    void Charge() {
        
    }

    void Retreat() {
        
    }

    void Guard() {
        
    }

    void Wander() {
        
    }
}
