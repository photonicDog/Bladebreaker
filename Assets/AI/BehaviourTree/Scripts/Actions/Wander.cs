using AI.BehaviourTree.Scripts.Runtime;

namespace AI.BehaviourTree.Scripts.Actions {
    public class Wander : ActionNode {
        private int targetNode = 0;
        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (context.enemyModel.wanderNodes.Count <= 0) return State.Success;
            bool success = context.enemyModel.WalkTo(context.enemyModel.wanderNodes[targetNode]);
            if (success) {
                targetNode++;
                if (targetNode > context.enemyModel.wanderNodes.Count) {
                    targetNode = 0;
                }

                return State.Success;
            }

            return State.Running;
        }
    }
}
