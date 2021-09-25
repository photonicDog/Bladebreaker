using UnityEngine;

namespace AIModules {
    public class AIGoto : AIModuleConditional {


        public override void Start(EntityAI _entityAI) {
            _entityAI._behaviorQueueIndex = target;
            End();
        }

        public override void Do() {
            throw new System.Exception("Should not DO!");
        }

        public override void End() {
            ended = true;
        }
    }
}