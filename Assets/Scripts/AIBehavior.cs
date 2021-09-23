using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AIBehavior", menuName = "AI", order = 0)]
    public class AIBehavior : SerializedScriptableObject {
        [Header("Basics")]
        public float detectionRange;

        [Header("Enemy Basics")]
        public bool canFly;
        public float hurtsOnTouch;
        
        [Header("Roaming")]
        public bool canRoam;
        public float roamRange;
        public bool approach;

        [Header("Behavior Lists")]
        public List<AIModule> longRangeAction;
        public List<AIAction> closeAction;

    }
