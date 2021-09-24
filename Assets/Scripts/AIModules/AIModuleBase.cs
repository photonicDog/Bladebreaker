namespace AIModules {
    public abstract class AIModuleBase {
        public float time;
        public bool ended;
        public EntityAI _entityAI;

        protected AIModuleBase(float time) {
            this.time = time;
        }

        public abstract void Start(EntityAI _entityAI);
        public abstract void Do();
        public abstract void End();
    }
}


