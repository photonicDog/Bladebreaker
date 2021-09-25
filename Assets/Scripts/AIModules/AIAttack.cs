using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    
    [Serializable]
    public class AIAttack : AIModuleBase {
        
        private Transform playerTransform;
        [NonSerialized, OdinSerialize][ShowInInspector] private float attackRange;

        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            playerTransform = GameObject.FindWithTag("Player").transform;
            WaitForEnd();
            _entityAI.Stop();;
        }

        public override void Do() {
            _entityAI.Attack();

            if ((playerTransform.position - _entityAI.transform.position).magnitude > attackRange) {
                End();
            }
        }
        
        private async void WaitForEnd() {
            float ctime = Time.realtimeSinceStartup;

            while (ctime + time > Time.realtimeSinceStartup) {
                await Task.Yield();
            }

            End();
        }

        public override void End() {
            ended = true;
        }
    }
}