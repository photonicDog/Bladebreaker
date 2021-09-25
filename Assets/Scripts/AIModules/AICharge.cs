using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    [Serializable]
    public class AICharge : AIModuleBase {
        private EntityMovement _em;
        private EntityAnimation _ea;
        [NonSerialized, OdinSerialize][ShowInInspector] private float chargeDistanceX;
        [NonSerialized, OdinSerialize][ShowInInspector] private float chargeDistanceY;

        public override void Start(EntityAI _entityAI) {
            _em = _entityAI.GetComponent<EntityMovement>();
            _ea = _entityAI.GetComponent<EntityAnimation>();

            _ea.SpoofDash(time);
            
            _em.PushEntity(new Vector2(chargeDistanceX * _em._facing, chargeDistanceY));
            End();
        }

        public override void Do() {
            throw new System.NotImplementedException();
        }

        public override void End() {
            ended = true;
        }
    }
}