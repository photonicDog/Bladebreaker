using Assets.Scripts.Controllers;
using System;
using UnityEngine;

namespace Assets.Scripts.Components
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

        [Header("SFX")]
        public AudioClip slashSfx;
        public AudioClip bigSlashSfx;

        private void OnEnable()
        {
            AudioController.Instance.PlayPlayerSFX(Damage >= 5 ? bigSlashSfx : slashSfx);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (Player && other.gameObject.layer == LayerMask.NameToLayer("EnemyHitbox")) {
                other.gameObject.SetActive(false);
            }
        }
    }
}
