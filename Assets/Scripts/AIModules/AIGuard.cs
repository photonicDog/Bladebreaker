using System.Threading.Tasks;
using UnityEngine;

namespace AIModules {
    public class AIGuard : AIModuleConditional {
        private int target;
        private bool guardHit;
        public AIGuard(float time, int target, bool plusOnBlock) : base(time, target) {
            this.target = target;
        }

        private Transform playerTransform;
        
        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            playerTransform = GameObject.FindWithTag("Player").transform;
            _entityAI.Stop();
            _entityAI.em.FullStop();
            _entityAI.ea.Guard(true);
            WaitForEnd();
        }

        public override void Do() {
            if (guardHit) {
                _entityAI._behaviorQueueIndex = target;
                _entityAI.ea.Guard(false);
                End();
            }
        }

        public void GuardHit() {
            guardHit = true;
        }
        
        private async void WaitForEnd() {
            float ctime = Time.realtimeSinceStartup;

            while (ctime + time > Time.realtimeSinceStartup) {
                await Task.Yield();
            }

            End();
        }

        public override void End() {
            _entityAI.ea.Guard(false);
            if (!guardHit) _entityAI._behaviorQueueIndex++;
            ended = true;
        }
    }
}