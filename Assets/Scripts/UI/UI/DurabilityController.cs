using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.UI
{
    public class DurabilityController : MonoBehaviour
    {
        public Transform durabilityMaskAnchor;
        public float MaxDurability = 20f;

        private float _maxSize;
        public float _durability;
        public bool _updateDurability;

        void Awake()
        {
            _maxSize = durabilityMaskAnchor.localScale.x;
            _durability = MaxDurability;
            _updateDurability = true;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_updateDurability)
            {
                float newSize = _maxSize * (_durability / MaxDurability);

                if (MaxDurability > 24)
                {
                    newSize = Mathf.Round(newSize * 4) / 4;
                }
                durabilityMaskAnchor.localScale = new Vector3(newSize, 1, 1);
                _updateDurability = false;
            }
        }

        public void SetDurability(int newDurability)
        {
            if (newDurability < 0)
            {
                _durability = 0;
            }
            else if (newDurability > MaxDurability)
            {
                _durability = MaxDurability;
            }
            else
            {
                _durability = newDurability;
            }

            _updateDurability = true;
        }

        private void AddDurability(int addDurability)
        {
            _durability = _durability + addDurability > MaxDurability ? MaxDurability : _durability + addDurability;
            _updateDurability = true;
        }

        public void RemoveDurability(int removeDurability)
        {
            _durability = _durability - removeDurability > 0 ? 0 : _durability + removeDurability;
            _updateDurability = true;
        }
    }
}