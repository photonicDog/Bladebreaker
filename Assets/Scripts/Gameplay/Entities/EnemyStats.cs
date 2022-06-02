using System.Collections.Generic;
using BladeBreaker.Gameplay.Core;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BladeBreaker.Gameplay.Entities
{
    public class EnemyStats : Stats
    {
        public float health;
        public AudioClip explodeSound;
        private EntityAnimation _ea;
        private EntityMovement _em;

        // Start is called before the first frame update
        void Start()
        {
            _ea = GetComponent<EntityAnimation>();
            _em = GetComponent<EntityMovement>();
        }

        // Update is called once per frame
        void Update()
        {
            if (health <= 0)
            {
                Die();
            }
        }

        public override void Die()
        {
            base.Die();
            //TODO: Decouple enemy death logic from the GameManager.
            //GameManager.Instance.player.GetComponent<PlayerStatsController>().DefeatEnemy();
            //GameManager.Instance.player.GetComponent<PlayerStatsController>().IncrementCombo();
            _ea.Die();
            Destroy(gameObject);
        }
    }
}

