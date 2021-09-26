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
        Damage(hitbox.Damage, 
            hitbox.HitStunDuration, 
            hitbox.HorizontalKnockback, 
            hitbox.VerticalKnockback, 
            hitbox.transform.parent.transform);
    }

    public void Damage(byte Damage, float HitStunDuration, float HorizontalKnockback, float VerticalKnockback, Transform source) {
        if (TryGetComponent(out Inventory playerInventory)) {
            playerInventory.WeaponDamage();
        }
        
        _stats.ModifyHealth(-Damage);
        _ea.Hurt();
        _em.Hitstun(HitStunDuration);
        if (_hasAI) _eai.Hitstun(HitStunDuration);
        _em.PushEntity(new Vector2(
            HorizontalKnockback * Mathf.Sign(source.localScale.x), 
            VerticalKnockback));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (attackingLayer == (attackingLayer | (1 << other.gameObject.layer))) {
            Hitbox hitbox;
            if (other.gameObject.TryGetComponent(out hitbox)) {
                if (_hasAI) _eai.TryDamage(this, hitbox);
            }
        }
    }
}
