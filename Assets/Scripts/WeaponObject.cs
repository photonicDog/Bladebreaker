using Assets.Scripts.Components;
using Assets.Scripts.Types.Enums;
using UnityEngine;


    [CreateAssetMenu(fileName = "Weapon Object", menuName = "Weapon", order = 0)]
    public class WeaponObject : ScriptableObject {
        [Header("Weapon")]
        public WeaponType WeaponType;

        [Header("Durability")]
        public byte Durability;
        public byte DurabilityLostOnHit;
    }
