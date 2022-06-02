using BladeBreaker.Gameplay.Core;
using UnityEngine;


namespace BladeBreaker.Gameplay.Combat
{
    [RequireComponent(typeof(WeaponDropper))]
    [RequireComponent(typeof(Stats))]
    public class DropWeaponOnDeath : MonoBehaviour
    {
        private WeaponDropper _dropper;
        private Stats _stats;

        // Start is called before the first frame update
        void Awake()
        {
            _dropper = GetComponent<WeaponDropper>();
            _stats = GetComponent<Stats>();

            _stats.OnDie += _dropper.DropWeapon;
        }

        // Update is called once per frame
        void OnDestroy()
        {
            _stats.OnDie -= _dropper.DropWeapon;
        }
    }
}

