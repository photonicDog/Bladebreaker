using System;
using UnityEngine;

namespace BladeBreaker.Gameplay.Combat
{
    [Serializable]
    public class Weapon
    {
        public Weapon(WeaponType weaponType, int durability, byte durabilityLostOnHit) {
            WeaponType = weaponType;
            Durability = durability;
            DurabilityLostOnHit = durabilityLostOnHit;
        }

        [Header("Weapon")]
        public WeaponType WeaponType;

        [Header("Durability")]
        public int Durability;
        public int DurabilityLostOnHit;
    }
}
