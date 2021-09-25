using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers.UI
{
    public class ComboCountController : MonoBehaviour
    {
        public Transform Digit1;
        public Transform Digit2;

        private List<Transform> _digit1Nums = new List<Transform>();
        private List<Transform> _digit2Nums = new List<Transform>();

        private List<Transform>[] _digits;

        private int _combo;
        private bool _updateCombo;
        void Awake()
        {
            foreach (Transform child in Digit1) _digit1Nums.Add(child);
            foreach (Transform child in Digit2) _digit2Nums.Add(child);

            _digits = new List<Transform>[] { _digit2Nums, _digit1Nums };

            _combo = 0;
            _updateCombo = true;
        }

        void Update()
        {
            if (_updateCombo)
            {
                string comboStr = _combo.ToString();
                while (comboStr.Length < 2)
                {
                    comboStr = $"0{comboStr}";
                }

                for (int i = 0; i < 2; i++)
                {
                    char digit = comboStr[i];
                    ChangeActive(i, digit);
                }

                _updateCombo = false;
            }
        }

        public void IncrementCombo()
        {
            AddCombo(1);
        }

        public void AddCombo(int addCombo)
        {
            _combo += addCombo;

            if (_combo > 9999999)
            {
                _combo = 9999999;
            }

            _updateCombo = true;
        }

        public void DecrementCombo()
        {
            RemoveCombo(1);
        }

        public void RemoveCombo(int removeCombo)
        {
            _combo -= removeCombo;

            if (_combo < 0)
            {
                _combo = 0;
            }

            _updateCombo = true;
        }

        public void ResetCombo()
        {
            SetCombo(0);
        }

        public void SetCombo(int newCombo)
        {
            _combo = newCombo;

            if (_combo > 99)
            {
                _combo = 99;
            }
            else if (_combo < 0)
            {
                _combo = 0;
            }

            _updateCombo = true;
        }

        public void ChangeActive(int digit, char number)
        {
            List<Transform> digits = _digits[digit];
            foreach (Transform num in digits)
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
