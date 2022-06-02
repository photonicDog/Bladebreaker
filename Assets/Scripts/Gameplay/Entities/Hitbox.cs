using System;
using UnityEngine;

namespace BladeBreaker.Gameplay.Entities
{
    public class Hitbox : MonoBehaviour
    {
        [Header("Damage")]
        public byte Damage;

        [Header("Knockback")]
        public float HorizontalKnockback;
        public float VerticalKnockback;

        [Header("Hit Stun")]
        public float HitStunDuration;
        public bool Player;
        public bool Thrown;

        private void OnTriggerEnter2D(Collider2D other) {
            if (Player && other.gameObject.layer == LayerMask.NameToLayer("EnemyHitbox")) {
                other.gameObject.SetActive(false);
                //TODO: Implement damage.
            }
        }
    }
}
