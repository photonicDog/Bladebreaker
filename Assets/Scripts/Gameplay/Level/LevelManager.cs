using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BladeBreaker.Core;
using BladeBreaker.Gameplay.Core;
using BladeBreaker.Gameplay.Entities;
using BladeBreaker.Gameplay.Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;

namespace BladeBreaker.Gameplay.Level
{
    public class LevelManager : SerializedMonoBehaviour
    {
        [Header("Checkpoints and Death")]
        public List<Checkpoint> checkpoints;
        public int checkpointMarker;
        private List<FightRoom> _fightRooms;

        [Header("Visuals")]
        public PostProcessVolume postprocess;
        private PostProcessProfile profile;
        private GBCameraSettings gbc;

        [Header("UI Screens")]
        public GameObject gameOver;

        [Header("Player reference")]
        public PlayerStats player;

        [Header("Pause")]
        public bool IsPaused;
        public bool CanPause;

        [Header("Tilemaps")]
        public Tilemap BackgroundTileMap;
        public Tilemap TerrainTileMap;
        public Tilemap PlatformTileMap;

        public static LevelManager Instance => _instance;
        private static LevelManager _instance;

        public Action OnDeath;
        public Action OnGameOver;
        public Action OnLevelLoad;
        public Action OnPause;
        public Action OnUnpause;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;

            CanPause = true;
            profile = postprocess.profile;
            profile.TryGetSettings(out gbc);
            _fightRooms = new List<FightRoom>(FindObjectsOfType<FightRoom>().ToList());
            gbc._Fade.value = 0;


        }

        private void Start()
        {
            player = PlayerStats.Instance;

            EventMessageBus.Instance.OnGameOver += GameOver;
            player.OnDie += DieAndReset;
        }

        IEnumerator FadeCoroutine(float target, float time)
        {
            float startValue = gbc._Fade.value;

            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                gbc._Fade.value = Mathf.Lerp(startValue, target, elapsedTime / time);
                yield return null;
            }
        }

        public void StartLevel()
        {
            //TODO: Move inventory messaging into inventory.
            /*if (SceneManager.Instance.currentWeapon != null)
            {
                _inventory.currentWeaponObject = SceneManager.Instance.currentWeapon;
                _inventory.SetWeapon(_inventory.currentWeaponObject, true);
            }*/
        }

        public void FinishLevel()
        {
            //TODO: Move all of these where they're supposed to be.
            /*
            _gm.CanPause = false;
            StageTimeInSeconds = Time.realtimeSinceStartup - _levelStartTime;
            Ranking ranking = _rankingController.FinishLevel(CurrentLevelIndex + 1, Secrets, this);
            _saveDataManager.SaveData(CurrentLevelIndex, ranking, Score, StageTimeInSeconds, Deaths, MaxCombo, EnemiesDefeated, Secrets);
            SceneManager.Instance.currentWeapon = _weaponObject;
            */
        }

        public void DieAndReset()
        {
            StartCoroutine(DeathSequence());
        }

        IEnumerator DeathSequence()
        {
            yield return FadeCoroutine(0f, 1f);
            _fightRooms.ForEach(a => a.Reset());
            // TODO: Put health reset into player death via Action.
            /* TODO: Optimize resetting enemies. */
            OnDeath?.Invoke();

            FindObjectsOfType<EnemyModel>().ForEach(a => a.ResetEverything());
            checkpoints[checkpointMarker].TeleportToCheckpoint();
            yield return FadeCoroutine(1f, 1f);
            AudioController.Instance.PlayMusic();
        }

        public void GameOver()
        {
            StartCoroutine(GameOverSequence());
        }

        IEnumerator GameOverSequence()
        {
            yield return FadeCoroutine(0f, 1f);
            _fightRooms.ForEach(a => a.Reset());
            // TODO: Put game over reset into player.
            // TODO: Reset the player inventory too.
            OnGameOver?.Invoke();

            checkpointMarker = 0;
            FindObjectsOfType<EnemyModel>().ForEach(a => a.ResetEverything());
            FindObjectsOfType<Secret>(true).ForEach(a => a.gameObject.SetActive(true));
            gameOver.SetActive(true);
            yield return FadeCoroutine(1f, 1f);
        }

        public void LoadLevel()
        {
            gameOver.SetActive(false);
            CanPause = true;
            StartCoroutine(StartLevelSequence());
        }

        IEnumerator StartLevelSequence()
        {
            yield return FadeCoroutine(0f, 1f);
            checkpoints[checkpointMarker].TeleportToCheckpoint();
            // TODO: Link into player start.
            OnLevelLoad?.Invoke();
            AudioController.Instance.PlayMusic();
            yield return FadeCoroutine(1f, 1f);
        }

        public void Pause(bool cameraFade = true)
        {
            if (!CanPause) return;
            Time.timeScale = 0;
            IsPaused = true;
            //TODO: Link into everything that needs a pause.
            OnPause?.Invoke();
            //player.GetComponent<EntityInput>().IsPaused = true;
            //player.GetComponent<EntityMovement>().IsPaused = true;
            //UIController.Instance.DisplayPause();
            AudioController.Instance.OnPause();
            if (cameraFade)
            {
                gbc._Fade.value = 0.75f;
            }
        }

        public void Unpause()
        {
            if (IsPaused)
            {
                gbc._Fade.value = 1;
            }
            // TODO: Link into action.
            OnUnpause?.Invoke();
            /*UIController.Instance.HidePause();
            AudioController.Instance.OnResume();*/
            Time.timeScale = 1;
            IsPaused = false;
            /*player.GetComponent<EntityInput>().IsPaused = false;
            player.GetComponent<EntityMovement>().IsPaused = false;*/
        }

        public void HitStun(float duration)
        {
            StartCoroutine(HitStunPause(duration));
        }

        IEnumerator HitStunPause(float duration)
        {
            Pause(false);
            yield return new WaitForSeconds(duration);
            Unpause();
        }

        public void ShakeScreen(float duration)
        {
            StartCoroutine(ShakeScreenCoroutine(duration, 0.1f));
        }

        IEnumerator ShakeScreenCoroutine(float duration, float shakeTick)
        {
            float ellapsedTime = 0;
            int rotate = 0;
            float pixelOffset = 0.125f;
            bool shaking = false;
            Vector3 shake = new Vector2(0, 0);

            while (ellapsedTime < duration)
            {
                yield return new WaitWhile(() => IsPaused);

                if (shaking)
                {
                    BackgroundTileMap.tileAnchor -= shake;
                    TerrainTileMap.tileAnchor -= shake;
                    PlatformTileMap.tileAnchor -= shake;
                }

                shaking = true;

                shake = rotate switch
                {
                    0 => new Vector2(pixelOffset, pixelOffset),
                    1 => new Vector2(-pixelOffset, pixelOffset),
                    2 => new Vector2(-pixelOffset, -pixelOffset),
                    3 => new Vector2(pixelOffset, -pixelOffset)
                };

                BackgroundTileMap.tileAnchor += shake;
                TerrainTileMap.tileAnchor += shake;
                PlatformTileMap.tileAnchor += shake;

                rotate = rotate == 3 ? 0 : rotate + 1;
                ellapsedTime += shakeTick;
                yield return new WaitForSeconds(shakeTick);
            }

            BackgroundTileMap.tileAnchor -= shake;
            TerrainTileMap.tileAnchor -= shake;
            PlatformTileMap.tileAnchor -= shake;
        }
    }

}
