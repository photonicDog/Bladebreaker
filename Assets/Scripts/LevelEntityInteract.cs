using Assets.Scripts.Components;
using Assets.Scripts.Controllers;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

namespace Assets.Scripts
{
    public class LevelEntityInteract : MonoBehaviour
    {
        public PostProcessVolume volume;

        private Collider2D _player;
        private PlayerStatsController _playerStats;
        private EntityMovement _em;
        private GBCameraSettings _gbc;
        private PlayerCamera _pc;

        private bool _inKillPlane;
        private Collider2D _currentKillPlane;
        private bool _hasDied;
        public Door _currentDoor;

        private void Awake()
        {
            _player = GetComponent<Collider2D>();
            _playerStats = GetComponent<PlayerStatsController>();
            _em = GetComponent<EntityMovement>();

            _hasDied = false;
            Camera camera = Camera.main;
            volume.profile.TryGetSettings(out _gbc);
            _pc = camera.GetComponent<PlayerCamera>();
        }

        private void Update()
        {
            if (_inKillPlane)
            {
                if (_player.bounds.max.y < _currentKillPlane.bounds.max.y && !_hasDied)
                {
                    _playerStats.Die();
                    _hasDied = true;
                }
            }
        }

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
        }

        private void HitFinishLevel()
        {
            //_playerStats.FinishLevel();
            _em.levelFinish = true;
            _em.walk = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            string colId = collision.tag;

            if (colId == "Door")
            {
                _currentDoor = null;
            }
        }

        public void EnterDoor(InputAction.CallbackContext context)
        {
            if (context.started && _currentDoor)
            {
                StartCoroutine(FadeTeleportCoroutine());
            }
        }

        public IEnumerator FadeTeleportCoroutine()
        {
            float time = 0.5f;

            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                _gbc._Fade.value = Mathf.Lerp(1, 0, elapsedTime / time);
                yield return null;
            }

            _player.transform.position = _currentDoor.Location;
            _pc.CurrentArea = _currentDoor.LocationCameraBounds;

            elapsedTime = 0;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                _gbc._Fade.value = Mathf.Lerp(0, 1, elapsedTime / time);
                yield return null;
            }

            _currentDoor = null;
        }
    }
}
