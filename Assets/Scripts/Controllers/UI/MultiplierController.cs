using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers.UI
{
    public class MultiplierController : MonoBehaviour
    {
        public Transform MultiplierNumber;
        public byte MaxMultiplier = 5;
        public byte MinMultiplier = 1;

        private List<Transform> _multiplierNums = new List<Transform>();

        private byte _multiplier;
        private bool _updateMultiplier;

        void Awake()
        {
            foreach (Transform child in MultiplierNumber) _multiplierNums.Add(child);
            _multiplier = 1;
            _updateMultiplier = true;
        }

        void Update()
        {
            if (_updateMultiplier)
            {
                ChangeActive(_multiplier);
                _updateMultiplier = false;
            }
        }

        public void SetMultiplier(byte newMultiplier)
        {
            _multiplier = newMultiplier;

            if (_multiplier > MaxMultiplier)
            {
                _multiplier = MaxMultiplier;
            }
            else if (_multiplier < MinMultiplier)
            {
                _multiplier = MinMultiplier;
            }

            _updateMultiplier = true;
        }

        public void IncrementMultiplier()
        {
            IncreaseMultiplier(1);
        }

        public void IncreaseMultiplier(byte addMultiplier)
        {
            _multiplier += addMultiplier;

            if (_multiplier > MaxMultiplier)
            {
                _multiplier = MaxMultiplier;
            }

            _updateMultiplier = true;
        }
        public void DecrementMultiplier()
        {
            DecreaseMultiplier(1);
        }

        public void DecreaseMultiplier(byte subMultiplier)
        {
            _multiplier -= subMultiplier;

            if (_multiplier < MinMultiplier)
            {
                _multiplier = MinMultiplier;
            }

            _updateMultiplier = true;
        }

        public void ResetMultiplier()
        {
            _multiplier = MinMultiplier;

            _updateMultiplier = true;
        }
        public void MaxOutMultiplier()
        {
            _multiplier = MaxMultiplier;

            _updateMultiplier = true;
        }

        public void ChangeActive(byte number)
        {
            foreach (Transform num in _multiplierNums)
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
