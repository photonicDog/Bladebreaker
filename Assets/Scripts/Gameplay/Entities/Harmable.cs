using BladeBreaker.Core;
using BladeBreaker.Gameplay.Core;
using System;
using UnityEngine;

namespace BladeBreaker.Gameplay.Entities
{
    public abstract class Harmable : MonoBehaviour, IHarmable
    {
        private IStats _stats;
        public bool iFrame = false;

        public LayerMask attackingLayer;

        public delegate void HitEvent(bool hit);
        public HitEvent OnHit;

        private void Awake()
        {
            TryGetComponent(out _stats);
        }

        private void Start()
        {
        }

        public void Damage(Hitbox hitbox)
        {
            Transform tf = hitbox.transform;
            Transform parent = tf.parent;
            OnHit?.Invoke(true);
            Damage(hitbox.Damage,
                hitbox.HitStunDuration,
                hitbox.HorizontalKnockback,
                hitbox.VerticalKnockback,
                parent != null ? parent.transform : tf);
        }

        protected void Damage(byte Damage, float HitStunDuration, float HorizontalKnockback, float VerticalKnockback, Transform source)
        {
            if (iFrame) return;
            _stats.ModifyHealth(-Damage);
        }
    }
}
