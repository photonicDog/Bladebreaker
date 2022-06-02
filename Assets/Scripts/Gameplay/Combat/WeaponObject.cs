using UnityEngine;

namespace BladeBreaker.Gameplay.Combat
{
    [CreateAssetMenu(fileName = "Weapon Object", menuName = "Weapon", order = 0)]
    public class WeaponObject : ScriptableObject
    {
        [Header("Weapon")]
        public Weapon Weapon;

        [Header("Ingame Representations")]
        public GameObject Drop;
        public GameObject Throw;
    }
}

