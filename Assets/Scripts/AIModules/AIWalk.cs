using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    public class AIWalk : AIModuleBase {
        [NonSerialized, OdinSerialize][ShowInInspector] private float runSpeed;
        [NonSerialized, OdinSerialize][ShowInInspector] private float stopDistance;

        private Transform playerTransform;

        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            playerTransform = GameObject.FindWithTag("Player").transform;
            _entityAI.Walk(Vector2.zero);
        }

        public override void Do() {
            Vector2 playerVector = (playerTransform.position - _entityAI.transform.position);
            if (Mathf.Abs(playerVector.x) < stopDistance && Mathf.Abs(playerVector.y) < stopDistance) {
                End();
            }
            
            if (_entityAI.AI.canFly) {
                
            }
            
            else {
                _entityAI.Walk(playerVector.normalized * runSpeed);
            }


        }

        public override void End() {
            ended = true;
        }
    }
}