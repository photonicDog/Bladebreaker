using Assets.Scripts.Types.Enums;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerStatsController : MonoBehaviour, IStats
    {
        public int CurrentLevelIndex;

        public byte MaxHealth;
        public byte Health;
        public byte MaxLives;
        public byte Lives;

        public int Score;
        public byte MaxMultiplier = 5;
        public byte Multiplier;
        public int MaxComboForMaxMultiplier = 15;
        public int Combo;
        public int MaxCombo;
        public float StageTimeInSeconds;
        public int Deaths;
        public int[] Secrets;
        public byte EnemiesDefeated;

        private WeaponType _weapon;
        private Inventory _inventory;
        private UIController _uiController;
        private RankingController _rankingController;
        private SaveDataManager _saveDataManager;
        private float _levelStartTime;

        void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _weapon = _inventory.currentWeaponType;
            _levelStartTime = Time.realtimeSinceStartup;
            Secrets = new int[3] { 0, 0, 0 };
        }

        private void Start() {
            _uiController = UIController.Instance;
            _rankingController = RankingController.Instance;
        }

        void Update()
        {
            if (Combo > 0 && _uiController.IsMeterEmpty() == true)
            {
                EndCombo();
            }
        }

        public void FinishLevel()
        {
            StageTimeInSeconds = Time.realtimeSinceStartup - _levelStartTime;
            Ranking ranking = _rankingController.FinishLevel();
            _saveDataManager.SaveData(CurrentLevelIndex, ranking, Score, StageTimeInSeconds, Deaths, MaxCombo, Secrets);
        }

        public void ChangeWeapon()
        {
            if (_weapon == WeaponType.None)
            {
                _uiController.DropWeapon();
            }
            else
            {
                _uiController.PickUpWeapon(_weapon);
            }
        }

        public void DefeatEnemy()
        {
            AddScore(100 * GetCurrentMultiplier());
            EnemiesDefeated += 1;
        }

        public void WeaponBreak()
        {
            AddScore(20 * GetCurrentMultiplier());
        }

        public void CollectSecret(int secretIndex)
        {
            AddScore(500);
            Secrets[secretIndex] = 1;
        }

        private void AddScore(int addedScore)
        {
            Score += addedScore;
            _uiController.ModifyScore(addedScore);
        }

        public void IncrementCombo()
        {
            Combo = ++Combo;
            _uiController.IncrementCombo();
            if (Combo < MaxCombo)
            {
                MaxCombo = Combo;
            }
        }

        public void EndCombo()
        {
            Combo = 0;
            AddScore(10 * Combo * GetCurrentMultiplier());
            _uiController.ResetCombo();
        }

        public int GetCurrentMultiplier()
        {
            // This maths scales the multiplier increment with the highest combo that counts for the multiplier
            // and the maximum multiplier the player should have itself
            return (int)Math.Min(math.floor(1 + Combo * (MaxMultiplier - 1 / MaxComboForMaxMultiplier)), MaxMultiplier);
        }

        public void ModifyHealth(int modify)
        {
            if (Health + modify > MaxHealth)
            {
                Health = MaxHealth;
                _uiController.RestoreAllHealth();
            }
            else if (Health + modify < 0)
            {
                Die();
            }
            else
            {
                Health += (byte)modify;

                if (modify < 0)
                {
                    _uiController.TakeDamage(modify / 4);
                }
                else
                {
                    _uiController.HealInRawHealth(modify);
                }
            }
        }

        public void GainLives(byte lives)
        {
            if (Lives + lives > MaxLives)
            {
                Lives = MaxLives;
                _uiController.MaxLives();
            } else
            {
                Lives += 1;
                _uiController.AddLife();
            }
        }

        public void Die()
        {
            Deaths += 1;
            if (Lives - 1 < 0)
            {
                GameOver();
            } else
            {
                Lives -= 1;
                _uiController.LoseLife();
                //TODO: Reset to checkpoint
            }
        }

        public void GameOver()
        {
            //TODO: Handle game over state
        }
    }
}
