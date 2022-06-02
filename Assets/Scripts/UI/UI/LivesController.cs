using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.UI
{
    public class LivesController : MonoBehaviour
    {
        public Transform LifeNumber;
        public int MaxLives = 9;

        private List<Transform> _lifeDigits = new List<Transform>();

        private int _lives;
        private bool _updateLives;

        void Awake()
        {
            foreach (Transform child in LifeNumber) _lifeDigits.Add(child);

            _lives = 3;
            _updateLives = true;
        }

        void Update()
        {
            if (_updateLives)
            {
                ChangeActive(_lives);
                _updateLives = false;
            }
        }

        public void SetLives(int newLives)
        {
            _lives = newLives;

            if (_lives > MaxLives)
            {
                _lives = MaxLives;
            } else if (_lives < 0)
            {
                _lives = 0;
            }

            _updateLives = true;
        }

        public void AddLife()
        {
            AddLives(1);
        }

        public void AddLives(int addLives)
        {
            _lives += addLives;

            if (_lives > MaxLives)
            {
                _lives = MaxLives;
            }

            _updateLives = true;
        }
        public void LoseLife()
        {
            RemoveLives(1);
        }

        public void RemoveLives(int removeLives)
        {
            _lives -= removeLives;

            if (_lives < 0)
            {
                _lives = 0;
            }

            _updateLives = true;
        }

        public void ChangeActive(int number)
        {
            foreach (Transform num in _lifeDigits)
            {
                if (num.name == $"NUM{number}")
                {
                    num.gameObject.SetActive(true);
                }
                else
                {
                    num.gameObject.SetActive(false);
                }
            }
        }
    }
}
