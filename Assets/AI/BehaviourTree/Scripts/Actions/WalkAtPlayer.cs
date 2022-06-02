using AI.BehaviourTree.Scripts.Runtime;
using BladeBreaker.Gameplay.Player;
using UnityEngine;

namespace AI.BehaviourTree.Scripts.Actions {
    public class WalkAtPlayer : ActionNode {

        public float speedVariance = 1f;
        public float distanceUntilStop = 2f;
        public float distanceUntilFailure = 12f;
        protected override void OnStart() {
            if (blackboard.player == null) {
                blackboard.player = PlayerStats.Instance;
            }
        }

        protected override void OnStop() {
            context.enemyModel.Stop();
        }

        protected override State OnUpdate() {
            Vector3 playerPosition = blackboard.player.transform.position;
            context.enemyModel.Walk(playerPosition * speedVariance);
            blackboard.distanceFromPlayer = context.enemyModel.DistanceTo(playerPosition);
            
            if (blackboard.distanceFromPlayer > distanceUntilFailure)
                return State.Failure;
            if (blackboard.distanceFromPlayer < distanceUntilStop)
                return State.Success;
            return State.Running;
        }
    }
}