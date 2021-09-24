using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers.UI
{
    public class ScoreController : MonoBehaviour
    {
        public Transform Digit1;
        public Transform Digit2;
        public Transform Digit3;
        public Transform Digit4;
        public Transform Digit5;
        public Transform Digit6;
        public Transform Digit7;

        private List<Transform> _digit1Nums = new List<Transform>();
        private List<Transform> _digit2Nums = new List<Transform>();
        private List<Transform> _digit3Nums = new List<Transform>();
        private List<Transform> _digit4Nums = new List<Transform>();
        private List<Transform> _digit5Nums = new List<Transform>();
        private List<Transform> _digit6Nums = new List<Transform>();
        private List<Transform> _digit7Nums = new List<Transform>();

        private List<Transform>[] _digits;

        private int _score;
        private bool _updateScore;

        private void Awake()
        {
            foreach (Transform child in Digit1) _digit1Nums.Add(child);
            foreach (Transform child in Digit2) _digit2Nums.Add(child);
            foreach (Transform child in Digit3) _digit3Nums.Add(child);
            foreach (Transform child in Digit4) _digit4Nums.Add(child);
            foreach (Transform child in Digit5) _digit5Nums.Add(child);
            foreach (Transform child in Digit6) _digit6Nums.Add(child);
            foreach (Transform child in Digit7) _digit7Nums.Add(child);

            _digits = new List<Transform>[] { _digit7Nums, _digit6Nums, _digit5Nums, _digit4Nums, _digit3Nums, _digit2Nums, _digit1Nums };

            _score = 0;
            _updateScore = true;
        }

        private void Update()
        {
            if (_updateScore)
            {
                string scoreStr = _score.ToString();
                while (scoreStr.Length < 7)
                {
                    scoreStr = $"0{scoreStr}";
                }

                for(int i = 0; i < 7; i++)
                {
                    char digit = scoreStr[i];
                    ChangeActive(i, digit);
                }

                _updateScore = false;
            }
        }

        public void AddScore(int addScore)
        {
            _score += addScore;

            if (_score > 9999999)
            {
                _score = 9999999;
            }

            _updateScore = true;
        }
        public void RemoveScore(int removeScore)
        {
            _score -= removeScore;

            if (_score < 0)
            {
                _score = 0;
            }

            _updateScore = true;
        }

        public void SetScore(int newScore)
        {
            _score = newScore;

            if (_score > 9999999)
            {
                _score = 9999999;
            } else if (_score < 0)
            {
                _score = 0;
            }

            _updateScore = true;
        }

        public void ChangeActive(int digit, char number)
        {
            List<Transform> digits = _digits[digit];
            foreach (Transform num in digits)
            {
                if (num.name == $"NUM{number}")
                {
                    num.gameObject.SetActive(true);
                } else
                {
                    num.gameObject.SetActive(false);
                }
            }
        }
    }
}
