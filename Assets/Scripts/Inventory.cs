using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.Types.Enums;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class Inventory : SerializedMonoBehaviour {

    public Animator animator;
    
    public Dictionary<WeaponType, GameObject> weaponObjects;
    public WeaponType currentWeaponType;
    public Weapon weapon;

    public WeaponPickup standingOn;

    private LayerMask pickupLayer;

    private void Awake() {
        pickupLayer = LayerMask.NameToLayer("Pickup");
    }

    private void Update() {
        DurabilityCheck();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (pickupLayer == other.gameObject.layer) {
            WeaponPickup pickup;
            if (other.gameObject.TryGetComponent(out pickup)) {
                standingOn = pickup;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (pickupLayer == other.gameObject.layer) {
            WeaponPickup pickup;
            if (other.gameObject.TryGetComponent(out pickup)) {
                if (standingOn == pickup) {
                    standingOn = null;
                }
            }
        }
    }

    public void SelectLogic(Vector2 fling) {
        if (standingOn) {
            PickUpWeapon();
        }
        else {
            EjectWeapon(fling);
        }
    }

    public void PickUpWeapon() {
        if (standingOn) {
            EjectWeapon(Vector2.zero);
            SetWeapon(standingOn.weapon);
            Destroy(standingOn.gameObject);
        }
    }

    public void EjectWeapon(Vector2 fling) {
        Vector2 throwVector;
        
        if (fling != Vector2.zero) {
            throwVector = fling;
        }
        else {
            throwVector = new Vector2(0.25f, 1);
        }

        if (weapon != null) {
            GameObject droppedWeapon = Instantiate(GameManager.Instance.WeaponDrops[weapon.WeaponType], transform.position, quaternion.identity);
            droppedWeapon.GetComponent<WeaponPickup>().weapon = weapon;
            droppedWeapon.GetComponent<EntityMovement>().PushEntity(throwVector);
        }
        
        ClearWeapons();
    }

    public void WeaponDamage() {
        weapon.Durability -= weapon.DurabilityLostOnHit;
    }

    public void DurabilityCheck() {
        if (weapon != null && weapon.Durability <= 0) {
            ClearWeapons();
        }
    }

    public void ClearWeapons() {
        foreach (KeyValuePair<WeaponType, GameObject> entry in weaponObjects) {
            entry.Value.SetActive(false);
        }
        weapon = null;
    }

    public void SetWeapon(Weapon weaponSet) {
        ClearWeapons();
        weaponObjects[weaponSet.WeaponType].SetActive(true);
        currentWeaponType = weaponSet.WeaponType;
        weapon = weaponSet;
        animator.SetInteger("WeaponID", (int)weaponSet.WeaponType);
    }
}
