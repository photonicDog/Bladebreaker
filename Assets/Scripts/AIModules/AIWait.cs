using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AIModules {
    public class AIWait : AIModuleBase {
        private bool roamWhileWaiting;
        public AIWait(float time, bool roamWhileWaiting) : base(time) {
            this.roamWhileWaiting = roamWhileWaiting;
        }

        public override void Start(EntityAI _entityAI) {
            this._entityAI = _entityAI;
            WaitForEnd();
        }

        public override void Do() {
            if (roamWhileWaiting) {
                _entityAI.currentlyWandering = true;
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
            if (roamWhileWaiting) {
                _entityAI.currentlyWandering = false;
            }
            ended = true;
        }
    }
}