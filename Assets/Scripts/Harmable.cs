using System;
using Assets.Scripts;
using Assets.Scripts.Components;
using Assets.Scripts.Controllers;
using UnityEngine;

public class Harmable : MonoBehaviour, IHarmable {
    private EntityMovement _em;
    private EntityAnimation _ea;
    private EntityAI _eai;
    private IStats _stats;
    private bool _hasAI;
    public bool iFrame = false;

    public LayerMask attackingLayer;
    public PlayerStatsController _pStats;

    private void Awake() {
        _em = GetComponent<EntityMovement>();
        _ea = GetComponent<EntityAnimation>();
        _hasAI = TryGetComponent(out _eai);
        TryGetComponent(out _stats);
    }

    private void Start()
    {
    }

    public void Damage(Hitbox hitbox) {
        if ((hitbox.Player) && hitbox.transform.parent.parent.parent.TryGetComponent(out Inventory playerInventory)) {
//            Debug.Log("A");
            playerInventory.WeaponDamage();
            if (_em.midair) {
                _em.Antigravity();
                playerInventory.GetComponent<EntityMovement>().Antigravity();
            }
        }

        Transform tf = hitbox.transform;
        Transform parent = tf.parent;
        Damage(hitbox.Damage,
            hitbox.HitStunDuration,
            hitbox.HorizontalKnockback,
            hitbox.VerticalKnockback,
            parent != null ? parent.transform : tf);
    }

    public void Damage(byte Damage, float HitStunDuration, float HorizontalKnockback, float VerticalKnockback, Transform source) {
        if (iFrame) return;

        
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
                else _em.GetComponent<IHarmable>().Damage(hitbox);
            }
        }
    }
}
