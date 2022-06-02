using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BladeBreaker.Gameplay.Core
{
    public class EventMessageBus
    {
        public static EventMessageBus Instance => _instance;
        private static EventMessageBus _instance;

        //LEVEL STUFF
        public Action OnStartLevel;
        public Action OnFinishLevel;
        public Action<int> OnSwitchLevels;
        public Action OnPause;
        public Action OnEndGame;
        public Action OnSave;

        //PLAYER STUFF
        public Action OnPlayerTakeDamage;
        public Action OnPlayerHeal;
        public Action OnPlayerDie;
        public Action OnChangeWeapon;
        public Action OnDefeatEnemy;
        public Action OnWeaponBreak;
        public Action OnCollectSecret;
        public Action OnAddScore;
        public Action OnIncrementCombo;
        public Action OnSetDurability;
        public Action OnEndCombo;
        public Action OnResetScore;
        public Action OnResetAll;
        public Action OnGainLives;
        public Action<float> OnModifyHealth;
        public Action OnGameOver;
    }
}
