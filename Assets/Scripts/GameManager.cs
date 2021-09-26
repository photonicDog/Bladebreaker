using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Types.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class GameManager : SerializedMonoBehaviour {

    public Dictionary<WeaponType, WeaponObject> WeaponData;
    public Dictionary<WeaponType, GameObject> WeaponDrops;

    public List<Checkpoint> checkpoints;
    public int checkpointMarker;

    public IStats playerStats;

    private static GameManager _instance;
    public static GameManager Instance {
        get { return _instance; }
    }

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
