namespace AIModules {
    public abstract class AIModuleConditional : AIModuleBase {
        protected int target;
        protected AIModuleConditional(float time, int target) : base(time) {
            this.target = target;
        }
    }
}