using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    public class AIIfDistance : AIModuleConditional {
        [NonSerialized, OdinSerialize][ShowInInspector] private float distance;

        private Transform playerTransform;
        
        public override void Start(EntityAI _entityAI) {
            ended = false;
            playerTransform = GameObject.FindWithTag("Player").transform;

            float currentDistance = (_entityAI.transform.position - playerTransform.position).magnitude;
            if (currentDistance < distance) {
                _entityAI._behaviorQueueIndex = target;
            }
            else {
                _entityAI._behaviorQueueIndex = failure;
            }

            End();
        }

        public override void Do() {
            throw new System.NotImplementedException();
        }

        public override void End() {
            ended = true;
        }

    }
}