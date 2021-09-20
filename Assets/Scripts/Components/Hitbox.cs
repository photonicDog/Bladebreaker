using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Hitbox : Component
    {
        [Header("Damage")]
        public byte Damage;

        [Header("Knockback")]
        public float HorizontalKnockback;
        public float VerticalKnockback;

        [Header("Hit Stun")]
        public byte HitStunDuration;
    }
}
