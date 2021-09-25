using UnityEngine;

namespace AIModules {
    public class AIRetreat : AIModuleBase {
        private float retreatDistance;
        
        private Transform playerTransform;
        public AIRetreat(float time, float retreatDistance) : base(time) {
            this.retreatDistance = retreatDistance;
        }

        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            playerTransform = GameObject.FindWithTag("Player").transform;
            _entityAI.Walk(Vector2.zero);
        }

        public override void Do() {
            Vector2 playerVector = (playerTransform.position - _entityAI.transform.position);
            if (Mathf.Abs(playerVector.x) > retreatDistance || Mathf.Abs(playerVector.y) > retreatDistance) {
                End();
            }
            
            if (_entityAI.AI.canFly) {
                
            }
            
            else {
                _entityAI.Walk(-playerVector.normalized);
            }
        }

        public override void End() {
            ended = true;
        }
    }
}