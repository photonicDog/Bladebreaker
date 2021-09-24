using System;
using Assets.Scripts.Components;
using UnityEngine;

public class Harmable : MonoBehaviour, IHarmable {
    private EntityMovement _em;
    private EntityAnimation _ea;
    private EntityAI _eai;
    private IStats _stats;
    private bool _hasAI;

    public LayerMask attackingLayer;

    private void Awake() {
        _em = GetComponent<EntityMovement>();
        _ea = GetComponent<EntityAnimation>();
        _hasAI = TryGetComponent(out _eai);
        TryGetComponent(out _stats);
    }

    public void Damage(Hitbox hitbox) {
        if (TryGetComponent(out Inventory playerInventory)) {
            playerInventory.WeaponDamage();
        }
        
        _stats.ModifyHealth(-hitbox.Damage);
        _em.Hitstun(hitbox.HitStunDuration);
        if (_hasAI) _eai.Hitstun(hitbox.HitStunDuration);
        _em.PushEntity(new Vector2(
            hitbox.HorizontalKnockback * Mathf.Sign(hitbox.transform.parent.transform.localScale.x), 
            hitbox.VerticalKnockback));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (attackingLayer == (attackingLayer | (1 << other.gameObject.layer))) {
            Hitbox hitbox;
            if (other.gameObject.TryGetComponent(out hitbox)) {
                Damage(hitbox);
            }
        }
    }
}
