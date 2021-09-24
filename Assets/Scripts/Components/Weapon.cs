using System;
using Assets.Scripts.Types.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public class Weapon
    {
        public Weapon(WeaponType weaponType, byte durability, byte durabilityLostOnHit) {
            WeaponType = weaponType;
            Durability = durability;
            DurabilityLostOnHit = durabilityLostOnHit;
        }

        public Weapon(WeaponObject obj) {
            WeaponType = obj.WeaponType;
            Durability = obj.Durability;
            DurabilityLostOnHit = obj.DurabilityLostOnHit;
        }

        [Header("Weapon")]
        public WeaponType WeaponType;

        [Header("Durability")]
        public byte Durability;
        public byte DurabilityLostOnHit;
    }
}
