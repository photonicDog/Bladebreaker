using System;
using System.Collections;
using UnityEngine;

namespace BladeBreaker.Gameplay.Combat
{
    public class DecayIntoWeapon : MonoBehaviour
    {
        private WeaponPickup pck;
        private EntityMovement em;
        private Coroutine crt;

        public GameObject explodeEffect;
        public GameObject asplosion;
        private void Start()
        {
            pck = GetComponent<WeaponPickup>();
            em = GetComponent<EntityMovement>();
        }

        void Update()
        {
            if (em.velocity.magnitude < 0.001f && crt == null)
            {
                Decay(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Terrain")) Decay(false);
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) Decay(true);
        }

        public void Decay(bool hit)
        {
            if (crt == null) crt = StartCoroutine(DecayRoutine(hit));
            else
            {
                StopCoroutine(crt);
                crt = StartCoroutine(DecayRoutine(hit));
            }
        }

        IEnumerator DecayRoutine(bool hit)
        {
            int faceVel = Math.Sign(em.velocity.x) * 1;
            em.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.1f);
            if (hit)
            {
                pck.WeaponObject.Weapon.Durability -= pck.WeaponObject.Weapon.DurabilityLostOnHit * 3;
                if (pck.WeaponObject.Weapon.Durability <= 0)
                {
                    Instantiate(explodeEffect, transform.position, Quaternion.identity);
                    Instantiate(asplosion, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    yield break;
                }
            }

            GameObject drp = Instantiate(pck.WeaponObject.Drop, transform.position,
                Quaternion.identity);
            drp.GetComponent<WeaponPickup>().WeaponObject = pck.WeaponObject;
            EntityMovement dem = drp.GetComponent<EntityMovement>();
            dem.PushEntity(new Vector2(-0.3f * faceVel, 1f).normalized * 0.75f);
            Destroy(gameObject);
        }
    }

}

