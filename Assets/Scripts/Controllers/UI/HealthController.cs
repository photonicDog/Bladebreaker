using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers.UI
{
    public class HealthController : MonoBehaviour
    {
        public Transform HP1;
        public Transform HP2;
        public Transform HP3;
        public Transform HP4;
        public Transform HP5;
        public Transform HP6;

        private List<Transform> _hp1Quarters = new List<Transform>();
        private List<Transform> _hp2Quarters = new List<Transform>();
        private List<Transform> _hp3Quarters = new List<Transform>();
        private List<Transform> _hp4Quarters = new List<Transform>();
        private List<Transform> _hp5Quarters = new List<Transform>();
        private List<Transform> _hp6Quarters = new List<Transform>();

        private List<Transform>[] _hearts;

        public int MaxHearts;

        private int _maxHealth;
        private int _health;
        private int _healthChange;

        void Awake()
        {
            foreach (Transform child in HP1) _hp1Quarters.Add(child);
            foreach (Transform child in HP2) _hp2Quarters.Add(child);
            foreach (Transform child in HP3) _hp3Quarters.Add(child);
            foreach (Transform child in HP4) _hp4Quarters.Add(child);
            foreach (Transform child in HP5) _hp5Quarters.Add(child);
            foreach (Transform child in HP6) _hp6Quarters.Add(child);

            _hearts = new List<Transform>[] { _hp1Quarters, _hp2Quarters, _hp3Quarters, _hp4Quarters, _hp5Quarters, _hp6Quarters };

            _maxHealth = MaxHearts * 4;
            _health = _maxHealth;
            _healthChange = 0;
        }

        void FixedUpdate()
        {
            if (_healthChange < 0)
            {
                if (_health - _healthChange < 0)
                {
                    _healthChange = _health;
                }

                List<Transform> CurrentHeart = _hearts[(int)(Math.Ceiling((float)_health / 4)) - 1];
                CurrentHeart[_health % 4].gameObject.SetActive(false);
                _healthChange += 1;
                _health -= 1;
            }

            if (_healthChange > 0)
            {
                if (_health + _healthChange > _maxHealth)
                {
                    _healthChange = _maxHealth - _health;
                }

                int heartIndex;
                int heartStateIndex = _health % 4 == 0 ? 0 : _health % 4;

                if (heartStateIndex == 0) {
                    heartIndex = (int)(Math.Ceiling((float)_health / 4));
                } else
                {
                    heartIndex = (int)(Math.Ceiling((float)_health / 4)) - 1;
                }
                List<Transform> CurrentHeart = _hearts[heartIndex];
                CurrentHeart[heartStateIndex].gameObject.SetActive(true);
                _healthChange -= 1;
                _health += 1;
            }
        }

        public void Deplete(int healthLost)
        {
            _healthChange -= healthLost;
        }

        public void Fill(int healthGained)
        {
            _healthChange += healthGained;
        }

        public void Die()
        {
            Deplete(_maxHealth);
        }

        public void HealAll()
        {
            Fill(_maxHealth);
        }

        public void HealHearts(int heartsGained)
        {
            Fill(heartsGained * 4);
        }

        public void LoseHearts(int heartsLost)
        {
            Deplete(heartsLost * 4);
        }
    }
}