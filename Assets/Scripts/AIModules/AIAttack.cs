using System.Threading.Tasks;
using UnityEngine;

namespace AIModules {
    public class AIAttack : AIModuleBase {
        
        private Transform playerTransform;
        private float attackRange;
        public AIAttack(float time, float attackRange) : base(time) {
            this.attackRange = attackRange;
        }

        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            playerTransform = GameObject.FindWithTag("Player").transform;
            WaitForEnd();
            _entityAI.Stop();;
        }

        public override void Do() {
            _entityAI.Attack();

            if ((playerTransform.position - _entityAI.transform.position).magnitude < attackRange) {
                End();
            }
        }
        
        private async void WaitForEnd() {
            float ctime = Time.realtimeSinceStartup;

            while (ctime + 3f > Time.realtimeSinceStartup) {
                await Task.Yield();
            }

            End();
        }

        public override void End() {
            ended = true;
        }
    }
}