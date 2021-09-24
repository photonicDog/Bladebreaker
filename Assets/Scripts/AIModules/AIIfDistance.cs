using UnityEngine;

namespace AIModules {
    public class AIIfDistance : AIModuleConditional {
        private int failure;
        private float distance;

        private Transform playerTransform;

        public AIIfDistance(float time, int target, int failure, float distance) : base(time, target) {
            this.failure = failure;
            this.distance = distance;
        }
        public override void Start(EntityAI _entityAI) {
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