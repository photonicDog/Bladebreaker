using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace BladeBreaker.Gameplay.Core
{
    public abstract class Stats : SerializedMonoBehaviour, IStats
    {
        public float Health;

        public Action OnTakeDamage;
        public Action OnHeal;
        public Action OnDie;

        public virtual void Die()
        {
            OnDie?.Invoke();
        }

        public virtual void ModifyHealth(int modify)
        {
            Health += modify;

            if (modify > 0)
            {
                OnHeal?.Invoke();
            } else
            {
                OnTakeDamage?.Invoke();
            }

            if (Health <= 0)
            {
                Die();
            }
        }
    }
}
