using System.Collections;
using System.Collections.Generic;
using BladeBreaker.Gameplay.Core;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BladeBreaker.Gameplay.Entities
{
    public class EnemyModel : SerializedMonoBehaviour
    {

        public bool logicEnabled;
        public bool playerDetected;
        public EntityMovement em;
        public EntityAnimation ea;
        public IHarmable harm;
        public GameObject puff;

        private Vector3 startPosition;

        [ShowInInspector] public List<Vector3> wanderNodes;

        [HideInInspector] public Vector3 leashRadiusCenter;
        public Vector3 leashRadiusCenterOffset;
        public float leashRadiusBounds;

        public float detectionRadius = 5;

        private Vector2 wanderDirection = Vector2.left;
        private bool leashBounded;

        private GameObject player;
        private IHarmable playerHarm;
        [HideInInspector] public bool guarding;

        public int _behaviorQueueIndex = 0;

        private float entityRandomSpeed;

        // Start is called before the first frame update
        void Start()
        {
            leashRadiusCenter = leashRadiusCenterOffset + transform.position;

            entityRandomSpeed = Random.Range(0.90f, 1.1f);
        }

        public void AssignPlayer(Stats playerStats)
        {
            player = playerStats.gameObject;
            playerHarm = player.GetComponent<Harmable>();
        }

        bool LeashCheck()
        {
            if (leashBounded)
            {
                Vector3 position = transform.position;
                float currentDistance = (position - leashRadiusCenter).magnitude;
                float prospectiveDistance = ((position + (Vector3)wanderDirection) - leashRadiusCenter).magnitude;
                if (prospectiveDistance > currentDistance)
                {
                    wanderDirection = Vector2.zero;
                    return true;
                }

                return false;
            }

            if ((transform.position - leashRadiusCenter).magnitude > leashRadiusBounds / 2)
            {
                leashBounded = true;
                return true;
            }

            if ((transform.position - leashRadiusCenter).magnitude < leashRadiusBounds / 2 - 0.5f)
            {
                leashBounded = false;
                return false;
            }

            return false;
        }

        public float DistanceTo(Vector2 target)
        {
            return Vector2.Distance(target, transform.position);
        }

        public void Walk(Vector2 target)
        {
            wanderDirection = new Vector2(target.x - transform.position.x, 0).normalized;
            //if (!LeashCheck()) 
            em.Walk(wanderDirection.x * entityRandomSpeed);
        }

        public void Stop()
        {
            em.Stop();
            wanderDirection = Vector2.zero;
            em.velocity = Vector2.zero;
        }

        public void Attack()
        {
            ea.Attack();
        }

        public bool WalkTo(Vector2 target)
        {
            if (DistanceTo(target) <= 1f) return true;
            Walk(target);
            return false;
        }

        public void TryDamage(Harmable input, Hitbox hitbox)
        {
            if (guarding)
            {
                //TODO: Guard effect
                Hitbox h = new Hitbox()
                {
                    Damage = 0,
                    HorizontalKnockback = -1f,
                    VerticalKnockback = 0.5f,
                    HitStunDuration = 1f
                };
                playerHarm.Damage(h);
            }
            else
            {
                ea.Hurt();
                input.Damage(hitbox);
            }
        }

        public void EmitPuff()
        {
            GameObject.Instantiate(puff, transform.position, quaternion.identity);
        }

        public void ResetEverything()
        {
        }

        public void Hitstun(float time)
        {
            wanderDirection = Vector2.zero;
            logicEnabled = false;
            StartCoroutine(HitstunTimer(time));
        }

        IEnumerator HitstunTimer(float time)
        {
            yield return new WaitForSeconds(time);
            logicEnabled = true;
        }
    }

}
