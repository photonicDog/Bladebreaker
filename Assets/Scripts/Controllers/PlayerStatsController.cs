using Assets.Scripts.Types.Enums;
using System;
using Assets.Scripts.Components;
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

        [Header("Audio SFX")]
        public AudioClip DamageSFX;
        public AudioClip HealSFX;
        public AudioClip PickUpSFX;

        private Weapon _weapon;
        private Inventory _inventory;
        private UIController _uiController;
        private RankingController _rankingController;
        private SaveDataManager _saveDataManager;
        private float _levelStartTime;
        private EntityAnimation _ea;
        private AudioController _audio;

        void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _ea = GetComponent<EntityAnimation>();
            _weapon = _inventory.weapon;
            _levelStartTime = Time.realtimeSinceStartup;
            Secrets = new int[3] { 0, 0, 0 };
        }

        private void Start() {
            _uiController = UIController.Instance;
            _rankingController = RankingController.Instance;
            _saveDataManager = SaveDataManager.Instance;
            _audio = AudioController.Instance;
            _inventory.SetWeapon(_weapon, true);
        }

        void Update()
        {
            if (Combo > 0 && _uiController.IsMeterEmpty() == true)
            {
                EndCombo();
            }
        }

        public void StartLevel() {
            if (SceneManager.Instance.currentWeapon != null) {
                _inventory.weapon = SceneManager.Instance.currentWeapon;
                _inventory.SetWeapon(_inventory.weapon, true);
            }
        }

        public void FinishLevel()
        {
            StageTimeInSeconds = Time.realtimeSinceStartup - _levelStartTime;
            Ranking ranking = _rankingController.FinishLevel(CurrentLevelIndex + 1, Secrets, this);
            _saveDataManager.SaveData(CurrentLevelIndex, ranking, Score, StageTimeInSeconds, Deaths, MaxCombo, EnemiesDefeated, Secrets);
            SceneManager.Instance.currentWeapon = _weapon;
        }

        public void ChangeWeapon(Weapon weapon) {
            if (weapon.WeaponType == WeaponType.None)
            {
                SetDurability(0);
                if (_weapon.WeaponType == WeaponType.None) return;
                _uiController.DropWeapon();
                _weapon = weapon;
            }
            else
            {
                _weapon = weapon;
                _uiController.PickUpWeapon(_weapon.WeaponType);
                SetDurability(weapon.Durability);
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

        public void SetDurability(int durability) {
            _uiController.SetDurability(durability);
        }

        public void EndCombo()
        {
            Combo = 0;
            AddScore(10 * Combo * GetCurrentMultiplier());
            _uiController.ResetCombo();
        }

        public void ResetScore() {
            Score = 0;
            _uiController.ResetScore();
        }

        public void ResetAll() {
            EndCombo();
            ResetScore();
            Lives = MaxLives;
            Health = MaxHealth;
            Multiplier = 0;
            StageTimeInSeconds = 0;
            Secrets = new int[3] { 0, 0, 0 };
            
            _uiController.ResetLife();
            _uiController.ResetMultiplier();
            _uiController.ResetScore();
            _uiController.RestoreAllHealth();
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
                _audio.PlayPlayerSFX(HealSFX);
                _uiController.RestoreAllHealth();
            }
            else if (Health + modify <= 0)
            {
                Die();
            }
            else
            {
                Health += (byte)modify;

                if (modify < 0)
                {
                    _audio.PlayPlayerSFX(DamageSFX);
                    _uiController.TakeDamage(modify / 4);
                }
                else
                {
                    _audio.PlayPlayerSFX(HealSFX);
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

        public void Die() {
            _ea.Die();
            Deaths += 1;
            _uiController.LoseAllHealth();
            _audio.StopMusic();
            if (Lives - 1 < 0)
            {
                GameOver();
            } else
            {
                Lives -= 1;
                _uiController.LoseLife();
                GameManager.Instance.DieAndReset(this);
            }
        }

        public void GameOver()
        {
            GameManager.Instance.GameOver(this);
        }
    }
}
