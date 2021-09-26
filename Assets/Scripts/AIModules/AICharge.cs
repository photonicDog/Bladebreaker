using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    [Serializable]
    public class AICharge : AIModuleBase {
        private EntityMovement _em;
        private EntityAnimation _ea;

        private bool _attacking;
        private EntityAI _entityAI;
        
        private Transform playerTransform;
        [NonSerialized, OdinSerialize][ShowInInspector] private float attackRange;

        public override void Start(EntityAI _entityAI) {
            ended = false;
            _em = _entityAI.GetComponent<EntityMovement>();
            _ea = _entityAI.GetComponent<EntityAnimation>();
            this._entityAI = _entityAI;

            playerTransform = GameObject.FindWithTag("Player").transform;
            
            WaitForEnd();
        }

        public override void Do() {
            Vector2 playerVector = (playerTransform.position - _entityAI.transform.position);
            if (Mathf.Abs(playerVector.x) < attackRange && Mathf.Abs(playerVector.y) < attackRange) {
                _ea.Lunge();
                End();
            }
            
            _entityAI.Walk(playerVector.normalized * 2);
        }
        
        private async void WaitForEnd() {
            float ctime = Time.realtimeSinceStartup;

            while (ctime + time > Time.realtimeSinceStartup) {
                if (_attacking) break;
                await Task.Yield();
            }

            if (!_attacking) End();
        }

        public override void End() {
            ended = true;
        }
    }
}