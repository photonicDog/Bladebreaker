using AI.BehaviourTree.Scripts.Runtime;
using BladeBreaker.Gameplay.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI.BehaviourTree.Scripts.Actions {
    public class Attack : ActionNode {
        public bool charge = false;
        [ShowIf("charge")] public bool popUpCharge = false;
        [ShowIf("charge")] public float chargeForce = 1.0f;
        public float attackRange = 1f;

        public float failThroughTimer;
        private float _failThrough;
        
        private Vector3 _chargeDirection;
        private bool _waitingOnHitFeedback = false;

        private enum HitFeedbackState {
            WAIT,
            HIT,
            MISS
        };

        private HitFeedbackState _hitFeedbackState;
        
        protected override void OnStart() {
            if (charge) {
                _chargeDirection = blackboard.player.transform.position - context.transform.position;
                if (!popUpCharge) _chargeDirection *= new Vector2(1, 0);
                context.entityMovement.PushEntity(_chargeDirection * chargeForce);
            }

            blackboard.player.GetComponent<Harmable>().OnHit += FeedbackCheck;
        }

        protected override void OnStop() {
            blackboard.player.GetComponent<Harmable>().OnHit -= FeedbackCheck;
        }

        protected override State OnUpdate() {
            //State is waiting to determine if the attack hit or not

            _failThrough += Time.deltaTime;

            if (_failThrough >= failThroughTimer) {
                _failThrough = 0f;
                return State.Failure;
            }
            
            //if (_waitingOnHitFeedback) return State.Running;
            //if (_hitFeedbackState == HitFeedbackState.HIT) return State.Success;
            
            if (Vector2.Distance(context.transform.position, blackboard.player.transform.position) <=
                attackRange) {
                context.entityAnimation.Attack();
                _waitingOnHitFeedback = true;
                return State.Success;
            }
            
            return State.Failure;
        }

        protected void FeedbackCheck(bool hit) {
            _waitingOnHitFeedback = hit;
            _hitFeedbackState = hit?HitFeedbackState.HIT:HitFeedbackState.MISS;
        }
    }
}