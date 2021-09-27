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
using Object = UnityEngine.Object;

public class GameManager : SerializedMonoBehaviour {

    public Dictionary<WeaponType, WeaponObject> WeaponData;
    public Dictionary<WeaponType, GameObject> WeaponDrops;

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
        
        player = GameObject.Find("Player");
        profile = postprocess.profile;
        profile.TryGetSettings(out gbc);
        _fightRooms = new List<FightRoom>(FindObjectsOfType<FightRoom>().ToList());
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
        gameOver.SetActive(true);
        yield return FadeCoroutine(1f, 1f);
    }

    public void LoadLevel() {
        gameOver.SetActive(false);
        StartCoroutine(StartLevelSequence());
    }

    IEnumerator StartLevelSequence() {
        yield return FadeCoroutine(0f, 1f);
        checkpoints[checkpointMarker].TeleportToCheckpoint();
        yield return FadeCoroutine(1f, 1f);
    }
}
