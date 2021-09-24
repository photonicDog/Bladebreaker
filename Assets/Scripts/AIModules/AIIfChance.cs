using UnityEngine;

namespace AIModules {
    public class AIIfChance : AIModuleConditional {
        private int failure;
        private float chance;
        public AIIfChance(float time, int target, int failure, float chance) : base(time, target) {
            this.failure = failure;
            this.chance = chance;
        }
        public override void Start(EntityAI _entityAI) {
            
            if (Random.Range(0f, 1f) < chance) {
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