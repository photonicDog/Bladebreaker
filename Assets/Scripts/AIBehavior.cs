using System;
using System.Collections.Generic;
using AIModules;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "AIBehavior", menuName = "AI", order = 0)]
    public class AIBehavior : SerializedScriptableObject {

        [Header("Enemy Basics")]
        public bool canFly;

        [Header("Wander")]
        public bool canWander;
        
        [NonSerialized, OdinSerialize] [Header("Behavior Lists NEW")] [ListDrawerSettings(ShowIndexLabels = true)] public List<AIModuleBase> behaviorCycle;
    }
