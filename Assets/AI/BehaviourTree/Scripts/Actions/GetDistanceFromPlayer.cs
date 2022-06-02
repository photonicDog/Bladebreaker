using AI.BehaviourTree.Scripts.Runtime;
using BladeBreaker.Gameplay.Player;
using UnityEngine;

namespace AI.BehaviourTree.Scripts.Actions {
    public class GetDistanceFromPlayer : ActionNode
    {
        protected override void OnStart() {
            if (blackboard.player == null) {
                blackboard.player = PlayerStats.Instance;
            }
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            float dist = Vector3.Distance(blackboard.player.transform.position, context.transform.position);
            blackboard.distanceFromPlayer = dist;
            return State.Success;
        }
    }
}
