using System;
using UnityEngine;
using BladeBreaker.Gameplay.Combat;
using BladeBreaker.Core;
using BladeBreaker.Gameplay.Core;

namespace BladeBreaker.Gameplay.Player
{
    public class PlayerStats : Stats
    {
        public int CurrentLevelIndex;

        public byte MaxHealth;
        public byte MaxLives;
        public byte Lives;

        public int Score;
        public byte MaxMultiplier = 5;
        public byte Multiplier;
        public int MaxComboForMaxMultiplier = 15;
        public int Combo;
        public int MaxCombo;
        public float ComboProgress;
        public float StageTimeInSeconds;
        public int Deaths;
        public int[] Secrets;
        public byte EnemiesDefeated;

        private WeaponObject _weaponObject;
        private Inventory _inventory;
        private float _levelStartTime;

        public static PlayerStats Instance => _instance;
        private static PlayerStats _instance;

        //TODO: Connect all Actions to animator, ui, level, and audio

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance.gameObject);
                _instance = this;
            }

            _inventory = GetComponent<Inventory>();
            _weaponObject = _inventory.currentWeaponObject;
            _levelStartTime = Time.realtimeSinceStartup;
            Secrets = new int[3] { 0, 0, 0 };

            OnHeal += EventMessageBus.Instance.OnPlayerHeal;
            OnTakeDamage += EventMessageBus.Instance.OnPlayerTakeDamage;
            OnDie += EventMessageBus.Instance.OnPlayerDie;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private void Start() {
            _inventory.SetWeapon(_weaponObject, true);
        }

        void Update()
        {
            if (Combo > 0 && ComboProgress <= 0)
            {
                EndCombo();
            }
        }

        public void ChangeWeapon(WeaponObject weapon) {
            if (weapon.Weapon.WeaponType == WeaponType.None)
            {
                SetDurability(0);
                if (_weaponObject.Weapon.WeaponType == WeaponType.None) return;
                _weaponObject = weapon;
            }
            else
            {
                _weaponObject = weapon;
                SetDurability(weapon.Weapon.Durability);
            }

            EventMessageBus.Instance.OnChangeWeapon?.Invoke();
        }

        public void DefeatEnemy()
        {
            AddScore(100 * GetCurrentMultiplier());
            EnemiesDefeated += 1;
            EventMessageBus.Instance.OnDefeatEnemy?.Invoke();
        }

        public void WeaponBreak()
        {
            AddScore(20 * GetCurrentMultiplier());
            EventMessageBus.Instance.OnWeaponBreak?.Invoke();
        }

        public void CollectSecret(int secretIndex)
        {
            AddScore(500);
            Secrets[secretIndex] = 1;
            EventMessageBus.Instance.OnCollectSecret?.Invoke();
        }

        private void AddScore(int addedScore)
        {
            Score += addedScore;
            EventMessageBus.Instance.OnCollectSecret?.Invoke();
        }

        public void IncrementCombo()
        {
            Combo = ++Combo;
            if (Combo < MaxCombo)
            {
                MaxCombo = Combo;
            }
            EventMessageBus.Instance.OnIncrementCombo?.Invoke();
        }

        public void SetDurability(int durability) {
            EventMessageBus.Instance.OnSetDurability?.Invoke();
        }

        public void EndCombo()
        {
            Combo = 0;
            AddScore(10 * Combo * GetCurrentMultiplier());
            EventMessageBus.Instance.OnSetDurability?.Invoke();
        }

        public void ResetScore() {
            Score = 0;
            EventMessageBus.Instance.OnResetScore?.Invoke();
        }

        public void ResetAll() {
            EndCombo();
            ResetScore();
            Lives = MaxLives;
            Health = MaxHealth;
            Multiplier = 0;
            StageTimeInSeconds = 0;
            Secrets = new int[3] { 0, 0, 0 };

            EventMessageBus.Instance.OnResetAll?.Invoke();
        }

        public int GetCurrentMultiplier()
        {
            // This maths scales the multiplier increment with the highest combo that counts for the multiplier
            // and the maximum multiplier the player should have itself
            return (int)Math.Min(Mathf.Floor(1 + Combo * (MaxMultiplier - 1 / MaxComboForMaxMultiplier)), MaxMultiplier);
        }

        public override void ModifyHealth(int modify)
        {
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }

        public void GainLives(byte lives)
        {

            if (Lives + lives > MaxLives)
            {
                Lives = MaxLives;
            } else
            {
                Lives += 1;
            }
            EventMessageBus.Instance.OnGainLives?.Invoke();
        }

        public override void Die() {
            Deaths += 1;
            if (Lives - 1 < 0)
            {
                GameOver();
                return;
            } else
            {
                Lives -= 1;
                EventMessageBus.Instance.OnGainLives?.Invoke();
                //LevelManager.Instance.DieAndReset(this);
            }

            base.Die();
        }

        public void GameOver()
        {
            base.Die();
            EventMessageBus.Instance.OnGameOver?.Invoke();
        }
    }
}
