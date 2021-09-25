using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Random = UnityEngine.Random;

namespace AIModules {
    public class AIIfChance : AIModuleConditional {
        [NonSerialized, OdinSerialize][ShowInInspector] private float chance;
        public override void Start(EntityAI _entityAI) {
            
            if (Random.Range(0f, 1f) < chance) {
                _entityAI._behaviorQueueIndex = target;
            }
            else {
                _entityAI._behaviorQueueIndex = failure;
            }

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