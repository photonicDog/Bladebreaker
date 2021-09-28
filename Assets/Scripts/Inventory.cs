using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.Controllers;
using Assets.Scripts.Types.Enums;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class Inventory : SerializedMonoBehaviour {

    public Animator animator;
    
    public Dictionary<WeaponType, GameObject> weaponObjects;
    public WeaponType currentWeaponType;
    public Weapon weapon;
    public Weapon fists;

    public WeaponPickup standingOn;

    private LayerMask pickupLayer;
    private PlayerStatsController _psc;
    private EntityMovement _em;

    private void Awake() {
        pickupLayer = LayerMask.NameToLayer("Pickup");
        _psc = GetComponent<PlayerStatsController>();
        _em = GetComponent<EntityMovement>();
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
            if (weapon != null && weapon.WeaponType != WeaponType.None) EjectWeapon(fling);
        }
    }

    public void PickUpWeapon() {
        if (standingOn) {
            EjectWeapon(Vector2.zero);
            SetWeapon(standingOn.weapon, true);
            _psc.ChangeWeapon(weapon);
            Destroy(standingOn.gameObject);
        }
    }

    public void EjectWeapon(Vector2 dir) {
        if (weapon == null || weapon.WeaponType == WeaponType.None) return;
        Vector2 throwVector;
        Debug.Log(dir);
        
        if (dir == Vector2.down && !_em.midair) {
            EatWeapon();
            return;
        }
        
        if (Mathf.Abs(dir.x) > 0.01f) {
            //THROW FORWARD
            throwVector = new Vector2(1f * _em._facing, 0f).normalized * 1.2f;
            GameObject thrownWeapon = Instantiate(GameManager.Instance.WeaponThrows[weapon.WeaponType], transform.position, quaternion.identity);
            thrownWeapon.GetComponent<WeaponPickup>().weapon = weapon;
            thrownWeapon.GetComponent<EntityMovement>().PushEntity(throwVector);
            thrownWeapon.GetComponent<EntityMovement>().antigravity = true;
        }
        else {
            throwVector = new Vector2(-0.3f * _em._facing, 0.8f).normalized * 0.55f;
            GameObject droppedWeapon = Instantiate(GameManager.Instance.WeaponDrops[weapon.WeaponType], transform.position, quaternion.identity);
            droppedWeapon.GetComponent<WeaponPickup>().weapon = weapon;
            droppedWeapon.GetComponent<EntityMovement>().PushEntity(throwVector);
        }
        


        
        ClearWeapons();
    }

    public void EatWeapon() {
        if (weapon.Durability == 255) {
            _psc.GainLives(1);
        }
        else {
            _psc.ModifyHealth(Mathf.CeilToInt((weapon.Durability / 255f) * 4));
            ClearWeapons(); 
        }
    }

    public void WeaponDamage() {
        weapon.Durability -= weapon.DurabilityLostOnHit;
        _psc.SetDurability(weapon.Durability);
    }

    public void DurabilityCheck() {
        if (weapon != null && weapon.Durability <= 0) {
            GameManager.Instance.player.GetComponent<PlayerStatsController>().WeaponBreak();
            ClearWeapons();
        }
    }

    public void ClearWeapons() {
        foreach (KeyValuePair<WeaponType, GameObject> entry in weaponObjects) {
            entry.Value.SetActive(false);
        }
        SetWeapon(fists, false);
        _psc.ChangeWeapon(fists);
    }

    public void SetWeapon(Weapon weaponSet, bool clear) {
        if (clear) ClearWeapons();
        if (weaponSet.WeaponType != WeaponType.None) weaponObjects[weaponSet.WeaponType].SetActive(true);
        currentWeaponType = weaponSet.WeaponType;
        weapon = weaponSet;
        animator.SetInteger("WeaponID", (weaponSet.WeaponType==WeaponType.None)?-1:(int)weaponSet.WeaponType);
    }
}
