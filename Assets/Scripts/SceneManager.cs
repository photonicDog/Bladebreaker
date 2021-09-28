using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.Controllers;
using Sirenix.OdinInspector;
using UnityEngine;

public class SceneManager : SerializedMonoBehaviour
{
    public int level = 0;
    public int MainMenuScene = 1;
    public int CreditsScene = 2;
    public int scenesBeforeLevel = 3;

    public Weapon currentWeapon;

    public Dictionary<int, bool> LevelHasLoopMusicPart;

    private static SceneManager _instance;
    public static SceneManager Instance => _instance;
    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }

    public void GoToMainMenu() {
        SwitchScene(MainMenuScene);
    }

    public void GoToCredits() {
        SwitchScene(CreditsScene);
    }

    public void SwitchLevel(int target) {
        if (level > target) {
            Debug.Log("Cannot backtrack levels!");
            return;
        }
        else {
            SwitchScene(target+scenesBeforeLevel);
        };
    }

    void SwitchScene(int i) {
        StartCoroutine(LoadScene(i));
    }

    IEnumerator LoadScene(int i) {
        if (CameraFader.Instance.FindCamera()) {
            yield return StartCoroutine(CameraFader.Instance.FadeCoroutine(0f, 1f));
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i);
            yield return new WaitForSeconds(2f);
            if (i >= scenesBeforeLevel) GameManager.Instance.player.GetComponent<PlayerStatsController>().StartLevel();
            yield return StartCoroutine(CameraFader.Instance.FadeCoroutine(1f, 1f));
        }
        else {
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i);
        }
    }
}
