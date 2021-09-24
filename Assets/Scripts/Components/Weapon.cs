using System;
using Assets.Scripts.Types.Enums;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon")]
        public WeaponType WeaponType;

        [Header("Durability")]
        public byte Durability;
        public byte DurabilityLostOnHit;
    }
}
