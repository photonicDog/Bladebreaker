using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AIModules {
    [Serializable]
    public abstract class AIModuleBase {
        [NonSerialized, OdinSerialize][ShowInInspector] public float time;
        [NonSerialized, OdinSerialize][HideInInspector] public bool ended;
        [NonSerialized, OdinSerialize][HideInInspector] public EntityAI _entityAI;
        
        public abstract void Start(EntityAI _entityAI);
        public abstract void Do();
        public abstract void End();
    }
}


