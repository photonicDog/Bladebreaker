using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AIBehavior", menuName = "AI", order = 0)]
    public class AIBehavior : SerializedScriptableObject {

        [Header("Enemy Basics")]
        public bool canFly;

        [Header("Wander")]
        public bool canWander;

        [Header("Behavior Lists")] [ListDrawerSettings(ShowIndexLabels = true)] public List<AIAction> behaviorCycle;
    }
