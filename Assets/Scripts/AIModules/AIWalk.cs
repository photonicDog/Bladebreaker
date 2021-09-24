using UnityEngine;

namespace AIModules {
    public class AIWalk : AIModuleBase {
        private float runSpeed;
        private float stopDistance;

        private Transform playerTransform;
        public AIWalk(float time, float runSpeed, float stopDistance) : base(time) {
            this.runSpeed = runSpeed;
            this.stopDistance = stopDistance;
        }

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