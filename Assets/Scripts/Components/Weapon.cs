using Assets.Scripts.Types.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Weapon : Component
    {
        [Header("Weapon")]
        public WeaponType WeaponType;

        [Header("Durability")]
        public byte Durability;
        public byte DurabilityLostOnHit;
    }
}
