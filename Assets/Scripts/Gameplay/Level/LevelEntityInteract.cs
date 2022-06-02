using BladeBreaker.Core;
using System.Collections;
using UnityEngine;

namespace BladeBreaker.Gameplay.Level
{
    //TODO
    public class LevelEntityInteract : MonoBehaviour
    {
        private Collider2D _player;
        private GBCameraSettings _gbc;
        private PlayerCamera _pc;

        private bool _inKillPlane;
        private Collider2D _currentKillPlane;
        private bool _hasDied;
        private Door _currentDoor;

        private void Awake()
        {
            _player = GetComponent<Collider2D>();

            _hasDied = false;
            _pc = Camera.main.GetComponent<PlayerCamera>();
        }

        /*private void Update()
        {
            if (_inKillPlane)
            {
                if (_player.bounds.max.y < _currentKillPlane.bounds.max.y && !_hasDied)
                {
                    _playerStats.Die();
                    _hasDied = true;
                    _inKillPlane = false;
                    _currentKillPlane = null;
                }
            }
        }*/


        private void OnTriggerEnter2D(Collider2D collision)
        {
            string colId = collision.tag;

            if (colId == "Kill")
            {
                HitKillPlane();
                _currentKillPlane = collision;
            }
            else if (colId == "FinishLevel")
            {
                HitFinishLevel();
            }
            else if (colId == "Door")
            {
                _currentDoor = collision.GetComponent<Door>();
            }
        }

        private void HitKillPlane()
        {
            _inKillPlane = true;
            _hasDied = false;
        }

        private void HitFinishLevel()
        {
            //TODO: FINISHING THE LEVEL SHOULD NOT LIVE HERE
            //_playerStats.FinishLevel();
            //_em.levelFinish = true;
            //_em.walk = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            string colId = collision.tag;

            if (colId == "Door")
            {
                _currentDoor = null;
            }
        }

        //TODO: MOVE THIS THE FUCK OUT
        /*
        public void EnterDoor(InputAction.CallbackContext context)
        {
            if (context.started && _currentDoor)
            {
                StartCoroutine(FadeTeleportCoroutine());
            }
        }
        */

        public IEnumerator FadeTeleportCoroutine() {
            yield return StartCoroutine(CameraFader.Instance.FadeCoroutine(0, 0.5f));

            _player.transform.position = _currentDoor.Location;
            if (_currentDoor.LocationCameraBounds) _pc.CurrentArea = _currentDoor.LocationCameraBounds;

            yield return StartCoroutine(CameraFader.Instance.FadeCoroutine(1, 0.5f));

            _currentDoor = null;
        }
    }
}
