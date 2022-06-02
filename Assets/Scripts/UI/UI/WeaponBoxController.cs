    using BladeBreaker.Gameplay.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.UI
{
    public class WeaponBoxController : MonoBehaviour
    {
        public Transform WeaponIcon;
        public WeaponType EquippedWeapon = WeaponType.None;

        private List<Transform> _weapons = new List<Transform>();

        private bool _swapWeapon;

        void Awake()
        {
            foreach (Transform child in WeaponIcon) _weapons.Add(child);
            _swapWeapon = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_swapWeapon)
            {
                Transform weapon = GetWeaponTransformFromEnum(EquippedWeapon);

                if (weapon == null)
                {
                    _weapons.ForEach(x => x.gameObject.SetActive(false));
                }
                else
                {
                    foreach (Transform wep in _weapons)
                    {
                        if (wep.name == weapon.name)
                        {
                            wep.gameObject.SetActive(true);
                        }
                        else
                        {
                            wep.gameObject.SetActive(false);
                        }
                    }
                }

                _swapWeapon = false;
            }
        }

        public void SwitchWeapon(WeaponType switchedWeapon)
        {
            EquippedWeapon = switchedWeapon;
            _swapWeapon = true;
        }

        public void UnequipWeapon() 
        {
            SwitchWeapon(WeaponType.None);
        }

        private Transform GetWeaponTransformFromEnum(WeaponType weaponType)
        {
            if (weaponType == WeaponType.None)
            {
                return null;
            } else
            {
                return _weapons[(int)weaponType];
            }
        }
    }
}