using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace AIModules {
    [Serializable]
    public abstract class AIModuleConditional : AIModuleBase {

        [NonSerialized, OdinSerialize][ShowInInspector] protected int target;
        [NonSerialized, OdinSerialize][ShowInInspector] protected int failure;
    }
}