using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BladeBreaker.Core
{
    public class SceneManager : SerializedMonoBehaviour
    {
        public int level = 0;
        public int MainMenuScene = 1;
        public int CreditsScene = 2;
        public int scenesBeforeLevel = 3;

        public Dictionary<int, bool> LevelHasLoopMusicPart;

        private static SceneManager _instance;
        public static SceneManager Instance => _instance;

        private AudioController _audio;

        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            _audio = AudioController.Instance;
        }

        public void GoToMainMenu()
        {
            SwitchScene(MainMenuScene);
        }

        public void GoToCredits()
        {
            SwitchScene(CreditsScene);
        }

        public void SwitchLevel(int target)
        {
            if (level > target)
            {
                Debug.Log("Cannot backtrack levels!");
                return;
            }
            else
            {
                SwitchScene(target + scenesBeforeLevel);
            };
        }

        void SwitchScene(int i)
        {
            StartCoroutine(LoadScene(i));
        }

        IEnumerator LoadScene(int i)
        {
            if (CameraFader.Instance.FindCamera())
            {
                yield return StartCoroutine(CameraFader.Instance.FadeCoroutine(0f, 1f));
                yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i);
                if (i < scenesBeforeLevel) yield return StartCoroutine(CameraFader.Instance.FadeCoroutine(0f, 1f));
            }
            else
            {
                yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i);
            }
        }
    }

}
