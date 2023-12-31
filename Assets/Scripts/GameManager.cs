using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Types.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public class GameManager : SerializedMonoBehaviour {

    public Dictionary<WeaponType, WeaponObject> WeaponData;
    public Dictionary<WeaponType, GameObject> WeaponDrops;
    public Dictionary<WeaponType, GameObject> WeaponThrows;

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
    public GameObject player;

    [Header("Pause")]
    public bool IsPaused;
    public bool CanPause;

    [Header("Tilemaps")]
    public Tilemap BackgroundTileMap;
    public Tilemap TerrainTileMap;
    public Tilemap PlatformTileMap;

    private static GameManager _instance;
    public static GameManager Instance {
        get { return _instance; }
    }

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }

        CanPause = true;
        player = GameObject.Find("Player");
        profile = postprocess.profile;
        profile.TryGetSettings(out gbc);
        _fightRooms = new List<FightRoom>(FindObjectsOfType<FightRoom>().ToList());
        gbc._Fade.value = 0;
    }

    IEnumerator FadeCoroutine(float target, float time) {
        float startValue = gbc._Fade.value;

        float elapsedTime = 0;

        while (elapsedTime < time) {
            elapsedTime += Time.deltaTime;
            gbc._Fade.value = Mathf.Lerp(startValue, target, elapsedTime/time);
            yield return null;
        }
    }

    public void DieAndReset(PlayerStatsController psc) {
        StartCoroutine(DeathSequence(psc));
    }

    IEnumerator DeathSequence(PlayerStatsController psc) {
        yield return FadeCoroutine(0f, 1f);
        _fightRooms.ForEach(a=>a.Reset());
        psc.ModifyHealth(24);
        /* TODO: Optimize */ FindObjectsOfType<EntityAI>().ForEach(a => a.ResetEverything());
        checkpoints[checkpointMarker].TeleportToCheckpoint();
        yield return FadeCoroutine(1f, 1f);
        AudioController.Instance.PlayMusic();
    }

    public void GameOver(PlayerStatsController psc) {
        StartCoroutine(GameOverSequence(psc));
    }

    IEnumerator GameOverSequence(PlayerStatsController psc) {
        yield return FadeCoroutine(0f, 1f);
        _fightRooms.ForEach(a=>a.Reset());
        psc.ResetAll();
        checkpointMarker = 0;
        player.GetComponent<Inventory>().ClearWeapons();
        FindObjectsOfType<EntityAI>().ForEach(a => a.ResetEverything());
        FindObjectsOfType<Secret>(true).ForEach(a => a.gameObject.SetActive(true));
        gameOver.SetActive(true);
        yield return FadeCoroutine(1f, 1f);
    }

    public void LoadLevel() {
        gameOver.SetActive(false);
        CanPause = true;
        StartCoroutine(StartLevelSequence());
    }

    IEnumerator StartLevelSequence() {
        yield return FadeCoroutine(0f, 1f);
        checkpoints[checkpointMarker].TeleportToCheckpoint();
        player.GetComponent<PlayerStatsController>().StartLevel();
        AudioController.Instance.PlayMusic();
        yield return FadeCoroutine(1f, 1f);
    }

    public void Pause(bool cameraFade = true)
    {
        if (!CanPause) return;
        Time.timeScale = 0;
        IsPaused = true;
        player.GetComponent<EntityInput>().IsPaused = true;
        player.GetComponent<EntityMovement>().IsPaused = true;
        UIController.Instance.DisplayPause();
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
        UIController.Instance.HidePause();
        AudioController.Instance.OnResume();
        Time.timeScale = 1;
        IsPaused = false;
        player.GetComponent<EntityInput>().IsPaused = false;
        player.GetComponent<EntityMovement>().IsPaused = false;
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

            if(shaking)
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
