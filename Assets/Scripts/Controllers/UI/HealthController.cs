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
        private int _uiHealth;

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
            ResetHealth();
            _uiHealth = _health;
        }

        void FixedUpdate()
        {
            if (_uiHealth != _health)
            {
                // If displayed health is greater than actual health, e.g. needs to be decreased
                if (_uiHealth > _health)
                {
                    List<Transform> currentHeart;
                    Transform healthSegment;

                    if (_uiHealth == _maxHealth)
                    {
                        currentHeart = _hearts[5];
                        healthSegment = currentHeart[0];
                    } else
                    {
                        currentHeart = _hearts[(int)(Math.Floor((float)(_uiHealth - 1) / 4))];
                        healthSegment = currentHeart[4 - (_uiHealth % 4) == 4 ? 0 : 4 - (_uiHealth % 4)];
                    }

                    healthSegment.gameObject.SetActive(false);
                    _uiHealth -= 1;
                }

                // If displayed health is lower than actual health, e.g. needs to be increased
                if (_uiHealth < _health)
                {
                    _uiHealth += 1;
                    List<Transform> currentHeart;
                    Transform healthSegment;

                    if (_uiHealth == 0)
                    {
                        currentHeart = _hearts[0];
                        healthSegment = currentHeart[3];
                    }
                    else
                    {
                        currentHeart = _hearts[(int)(Math.Floor((float)(_uiHealth - 1) / 4))];
                        healthSegment = currentHeart[4 - (_uiHealth % 4) == 4 ? 0 : 4 - (_uiHealth % 4)];
                    }

                    healthSegment.gameObject.SetActive(true);
                }
            }
        }

        public void Deplete(int healthLost)
        {
            // Don't decrease health below zero
            if (_health - healthLost >= 0)
            {
                _health -= healthLost;
            } else
            {
                _health = 0;
            }
        }

        public void Fill(int healthGained)
        {
            // Don't increase health above the max
            if (_health + healthGained <= _maxHealth)
            {
                _health += healthGained;
            }
            else
            {
                _health = _maxHealth;
            }
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

        public void ResetHealth()
        {
            _health = _maxHealth;
        }
    }
}