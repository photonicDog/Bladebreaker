using System;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    public class AIWait : AIModuleBase {
        [NonSerialized, OdinSerialize][ShowInInspector] private bool roamWhileWaiting;
        public override void Start(EntityAI _entityAI) {
            ended = false;
            this._entityAI = _entityAI;
            _entityAI.Stop();
            //_entityAI.em.FullStop();
            WaitForEnd();
        }

        public override void Do() {
            if (roamWhileWaiting) {
                _entityAI.currentlyWandering = true;
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
            if (roamWhileWaiting) {
                _entityAI.currentlyWandering = false;
            }
            ended = true;
        }
    }
}