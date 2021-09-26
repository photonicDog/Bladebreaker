using System;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    public class AIJump : AIModuleBase {
        private EntityMovement _em;
        private EntityAnimation _ea;
        
        private bool midairChecking = false;
        
        [NonSerialized, OdinSerialize][ShowInInspector] private float jumpDistanceX;
        [NonSerialized, OdinSerialize][ShowInInspector] private float jumpDistanceY;

        public override void Start(EntityAI _entityAI) {
            ended = false;
            _em = _entityAI.GetComponent<EntityMovement>();
            _ea = _entityAI.GetComponent<EntityAnimation>();

            _em.jump = true;
            _em.PushEntity(new Vector2(jumpDistanceX * _em._facing, jumpDistanceY));
            JumpDelay();
        }

        public override void Do() {
            if (!_em.midair && midairChecking) {
                End();
            }
        }

        async void JumpDelay() {
            float ctime = Time.realtimeSinceStartup;

            while (ctime + time > Time.realtimeSinceStartup) {
                await Task.Yield();
            }
            midairChecking = true;
        }

        public override void End() {
            midairChecking = false;
            _em.Stop();
            ended = true;
        }
    }
}