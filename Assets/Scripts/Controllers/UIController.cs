using Assets.Scripts.Controllers.UI;
using Assets.Scripts.Types.Enums;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    //NOTE: NEVER USE ANY UI CONTROLLER METHOD OUTSIDE OF THIS CLASS!
    //If you need function to exist you can make it yourself in here but all UI traffic must go through here
    public class UIController : MonoBehaviour
    {
        public Transform WeaponBox;
        public Transform Health;
        public Transform Durability;
        public Transform Lives;
        public Transform Score;
        public Transform ComboTimer;
        public Transform ComboCount;
        public Transform Multiplier;

        private WeaponBoxController _weaponBoxController;
        private HealthController _healthController;
        private DurabilityController _durabilityController;
        private LivesController _livesController;
        private ScoreController _scoreController;
        private MultiplierController _multiController;
        private ComboCountController _comboCountMultiplier;
        private ComboTimerController _comboTimerController;

        private static UIController _instance;
        public static UIController Instance { get { return _instance; } }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        // Use this for initialization
        void Start()
        {
            _weaponBoxController = WeaponBox.GetComponent<WeaponBoxController>();
            _healthController = Health.GetComponent<HealthController>();
            _durabilityController = Durability.GetComponent<DurabilityController>();
            _livesController = Lives.GetComponent<LivesController>();
            _scoreController = Score.GetComponent<ScoreController>();
            _multiController = Multiplier.GetComponent<MultiplierController>();
            _comboCountMultiplier = ComboCount.GetComponent<ComboCountController>();
            _comboTimerController = ComboTimer.GetComponent<ComboTimerController>();
        }

        public void PickUpWeapon(WeaponType weapon)
        {
            _weaponBoxController.SwitchWeapon(weapon);
        }

        public void DropWeapon()
        {
            _weaponBoxController.UnequipWeapon();
        }

        public void ModifyScore(int newScore)
        {
            if (newScore < 0)
            {
                _scoreController.RemoveScore(newScore);
            } else
            {
                _scoreController.AddScore(newScore);
            }
        }

        public void ResetScore()
        {
            _scoreController.SetScore(0);
        }

        public void SetMultiplier(byte multiplier)
        {
            _multiController.SetMultiplier(multiplier);
        }

        public void ResetMultiplier()
        {
            _multiController.ResetMultiplier();
        }

        public void AddLife()
        {
            _livesController.AddLife();
        }

        public void LoseLife()
        {
            _livesController.LoseLife();
        }

        public void ResetLife()
        {
            _livesController.SetLives(3);
        }

        public void TakeDamage(int damageInHearts)
        {
            _healthController.LoseHearts(damageInHearts);
        }

        public void Heal(int healthInHearts)
        {
            _healthController.HealHearts(healthInHearts);
        }

        public void LoseAllHealth()
        {
            _healthController.Die();
        }

        public void RestoreAllHealth()
        {
            _healthController.HealAll();
        }

        public void LoseDurability(byte lostDurability)
        {
            _durabilityController.RemoveDurability(lostDurability);
        }

        public void SetDurability(byte newDurability)
        {
            _durabilityController.SetDurability(newDurability);
        }

        public void ResetDurability()
        {
            _durabilityController.SetDurability(0);
        }

        public void IncrementCombo()
        {
            _comboCountMultiplier.IncrementCombo();
        }

        public void ResetCombo()
        {
            _comboCountMultiplier.ResetCombo();
        }

        public void RefillTimer()
        {
            _comboTimerController.RefreshMeter();
        }
    }
}