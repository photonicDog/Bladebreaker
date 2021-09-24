using System.Threading.Tasks;
using UnityEngine;

namespace AIModules {
    public class AIAttack : AIModuleBase {
        
        private Transform playerTransform;
        public AIAttack(float time, bool airAttack) : base(time) {
        }

        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            playerTransform = GameObject.FindWithTag("Player").transform;
            WaitForEnd();
            _entityAI.Stop();;
        }

        public override void Do() {
            _entityAI.Attack();
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