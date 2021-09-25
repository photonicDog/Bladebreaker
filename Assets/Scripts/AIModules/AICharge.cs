using UnityEngine;

namespace AIModules {
    public class AICharge : AIModuleBase {
        private EntityMovement _em;
        private EntityAnimation _ea;
        private float chargeDistanceX;
        private float chargeDistanceY;
        
        public AICharge(float time, float chargeDistanceX, float chargeDistanceY) : base(time) {
            this.chargeDistanceX = chargeDistanceX;
            this.chargeDistanceY = chargeDistanceY;
        }

        public override void Start(EntityAI _entityAI) {
            _em = _entityAI.GetComponent<EntityMovement>();
            _ea = _entityAI.GetComponent<EntityAnimation>();
            
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