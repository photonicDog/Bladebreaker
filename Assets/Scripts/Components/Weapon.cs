using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Weapon : Component
    {
        [Header("Weapon")]
        public Weapon WeaponType;

        [Header("Durability")]
        public byte Durability;
        public byte DurabilityLostOnHit;
    }
}
