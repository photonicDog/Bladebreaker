using System.Collections.Generic;
using BladeBreaker.Gameplay.Combat;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace BladeBreaker.Gameplay.Player
{
    public class Inventory : SerializedMonoBehaviour
    {
        public Animator animator;

        public WeaponType currentWeaponType;
        public WeaponObject currentWeaponObject;
        public WeaponObject fistsObject;

        public WeaponPickup standingOn;

        private LayerMask pickupLayer;
        private PlayerStats _psc;
        private EntityMovement _em;

        private void Awake()
        {
            pickupLayer = LayerMask.NameToLayer("Pickup");
            _psc = GetComponent<PlayerStats>();
            _em = GetComponent<EntityMovement>();
        }

        private void Update()
        {
            DurabilityCheck();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (pickupLayer == other.gameObject.layer)
            {
                WeaponPickup pickup;
                if (other.gameObject.TryGetComponent(out pickup))
                {
                    standingOn = pickup;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (pickupLayer == other.gameObject.layer)
            {
                WeaponPickup pickup;
                if (other.gameObject.TryGetComponent(out pickup))
                {
                    if (standingOn == pickup)
                    {
                        standingOn = null;
                    }
                }
            }
        }

        public void SelectLogic(Vector2 fling)
        {
            if (standingOn)
            {
                PickUpWeapon();
            }
            else
            {
                if (currentWeaponObject != null && currentWeaponObject.Weapon.WeaponType != WeaponType.None) EjectWeapon(fling);
            }
        }

        public void PickUpWeapon()
        {
            if (standingOn)
            {
                EjectWeapon(Vector2.zero);
                SetWeapon(standingOn.WeaponObject, true);
                _psc.ChangeWeapon(currentWeaponObject);
                Destroy(standingOn.gameObject);
            }
        }

        public void EjectWeapon(Vector2 dir)
        {
            if (currentWeaponObject == null || currentWeaponObject.Weapon.WeaponType == WeaponType.None) return;
            Vector2 throwVector;
            Debug.Log(dir);

            if (dir == Vector2.down && !_em.midair)
            {
                EatWeapon();
                return;
            }

            if (Mathf.Abs(dir.x) > 0.01f)
            {
                //THROW FORWARD
                throwVector = new Vector2(1f * _em._facing, 0f).normalized * 1.2f;
                GameObject thrownWeapon = Instantiate(currentWeaponObject.Throw, transform.position, quaternion.identity);
                thrownWeapon.GetComponent<WeaponPickup>().WeaponObject = currentWeaponObject;
                thrownWeapon.GetComponent<EntityMovement>().PushEntity(throwVector);
                thrownWeapon.GetComponent<EntityMovement>().antigravity = true;
            }
            else
            {
                throwVector = new Vector2(-0.25f * _em._facing, 0.75f).normalized * 0.40f;
                GameObject droppedWeapon = Instantiate(currentWeaponObject.Drop, transform.position, quaternion.identity);
                droppedWeapon.GetComponent<WeaponPickup>().WeaponObject = currentWeaponObject;
                droppedWeapon.GetComponent<EntityMovement>().PushEntity(throwVector);
            }

            ClearWeapons();
        }

        public void EatWeapon()
        {
            _psc.ModifyHealth(Mathf.CeilToInt((currentWeaponObject.Weapon.Durability / 255f) * 4));
            ClearWeapons();
        }

        public void WeaponDamage()
        {
            currentWeaponObject.Weapon.Durability -= currentWeaponObject.Weapon.DurabilityLostOnHit;
            _psc.SetDurability(currentWeaponObject.Weapon.Durability);
        }

        public void DurabilityCheck()
        {
            if (currentWeaponObject != null && currentWeaponObject.Weapon.Durability <= 0 && currentWeaponObject.Weapon.WeaponType != WeaponType.None)
            {
                _psc.WeaponBreak();
                ClearWeapons();
            }
        }

        public void ClearWeapons()
        {
            SetWeapon(fistsObject, false);
            _psc.ChangeWeapon(fistsObject);
        }

        public void SetWeapon(WeaponObject weaponSet, bool clear)
        {
            if (clear) ClearWeapons();
            currentWeaponType = weaponSet.Weapon.WeaponType;
            currentWeaponObject = weaponSet;
            animator.SetInteger("WeaponID", (weaponSet.Weapon.WeaponType == WeaponType.None) ? -1 : (int)weaponSet.Weapon.WeaponType);
        }
    }
}

