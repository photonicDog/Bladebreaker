namespace BladeBreaker.Gameplay.Core
{
    public interface IStats
    {
        public void ModifyHealth(int modify);
        void Die();
    }

}
