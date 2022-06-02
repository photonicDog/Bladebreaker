using BladeBreaker.Gameplay.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.Gameplay.Combat
{
    public class HitboxNullifyLink : MonoBehaviour
    {

        public Harmable _harmable;
        private SpriteRenderer _sprRender;
        private bool working;

        private Coroutine _flash;

        // Start is called before the first frame update
        void Start()
        {
            _sprRender = GetComponent<SpriteRenderer>();
        }

        public void Invincible(float time)
        {
            if (!working)
            {
                StartCoroutine(InvincibleCoroutine(time));
                working = true;
            }
        }

        public void FlashSet()
        {
            if (_flash == null) _flash = StartCoroutine(Flash());
        }

        IEnumerator InvincibleCoroutine(float time)
        {
            _harmable.iFrame = true;
            yield return new WaitForSeconds(time);
            _harmable.iFrame = false;
            if (_flash != null) StopCoroutine(_flash);
            _flash = null;
            _sprRender.enabled = true;
            working = false;
        }

        IEnumerator Flash()
        {
            while (working)
            {
                //TODO: Un-hardcode flash timer.
                _sprRender.enabled = false;
                yield return new WaitForSeconds(0.06f);
                _sprRender.enabled = true;
                yield return new WaitForSeconds(0.06f);
            }
        }
    }
}