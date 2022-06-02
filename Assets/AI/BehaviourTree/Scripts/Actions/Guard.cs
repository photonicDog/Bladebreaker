using AI.BehaviourTree.Scripts.Runtime;
using UnityEngine;

namespace AI {
    public class Guard : ActionNode
    {
        public float duration = 1;
        float startTime;
        
        protected override void OnStart() {
            context.enemyModel.Stop();
            context.entityMovement.FullStop();
            context.entityAnimation.Guard(true);
        }

        protected override void OnStop() {
            context.entityAnimation.Guard(false);
        }

        protected override State OnUpdate() {
            if (Time.time - startTime > duration) {
                return State.Success;
            }
            return State.Running;
        }
    }
}
