using Assets.Scripts.Controllers.UI;
using Assets.Scripts.Types.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class RankingController : MonoBehaviour
    {
        [Header("UI")]
        public MultiplierController LevelDisplay;
        public ScoreController ScoreDisplay;
        public TimeController TimeDisplay;
        public Transform GaugeAnchor;
        public Transform RankingGrade;
        public Transform Secrets;

        [Header("Player Stats")]
        public int LevelNumber;
        public int Score;
        public double TimeInSeconds;
        public int MaxCombo;
        public int CurrentCombo;
        public int Deaths;
        public int EnemiesDefeated;
        public Ranking Ranking;

        [Header("Ranking Parameters")]
        public int LevelEnemyTotal;
        public int ScoreThreshold;
        public int ComboThreshold;
        public double ParTime;
        public double BogeyTime;

        private static RankingController _instance;
        public static RankingController Instance { get { return _instance; } }
        private Camera _camera;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _camera = Camera.current;
            Score = 0;
            TimeInSeconds = 0;
            MaxCombo = 0;
            CurrentCombo = 0;
            EnemiesDefeated = 0;
            Deaths = 0;
            LevelDisplay.SetMultiplier((byte)LevelNumber);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Ranking FinishLevel(int levelNumber, int[] secrets, PlayerStatsController psc) {
            Score = psc.Score;
            TimeInSeconds = psc.StageTimeInSeconds;
            MaxCombo = psc.MaxCombo;
            Deaths = psc.Deaths;
            EnemiesDefeated = psc.EnemiesDefeated;
            LevelNumber = levelNumber;
            Ranking = CalculateRanking(Score, TimeInSeconds, MaxCombo, Deaths, EnemiesDefeated);
            StartCoroutine(DisplayRankScreen(secrets));
            return Ranking;
        }

        private Ranking CalculateRanking(float score, double time, int maxCombo, int deaths, int enemies)
        {
            bool parCheck = time < ParTime;
            bool bogeyCheck = time < BogeyTime;
            float enemyCompletion = enemies / LevelEnemyTotal;

            float scoreCompletion = score / ScoreThreshold;
            float comboCompletion = maxCombo / ComboThreshold;
            float styleRating = (scoreCompletion + comboCompletion) / 2;

            if (deaths == 0 && enemyCompletion >= 1f && styleRating >= 1f && parCheck)
            {
                return Ranking.Overdrive;
            } else if (deaths < 2 && enemyCompletion >= 1f && styleRating >= 0.8f && parCheck)
            {
                return Ranking.Insane;
            } else if (deaths < 5 && enemyCompletion >= 0.75f && styleRating >= 0.6f && bogeyCheck)
            {
                return Ranking.Gold;
            } else if (enemyCompletion >= 0.5f && styleRating >= 0.4f && bogeyCheck)
            {
                return Ranking.Silver;
            } else
            {
                return Ranking.Bronze;
            }
        }

        public void DisplayRanking()
        {
            string letterGrade = Ranking switch
            {
                Ranking.Overdrive => "X",
                Ranking.Insane => "A",
                Ranking.Gold => "B",
                Ranking.Silver => "C",
                _ => "D"
            };

            RankingGrade.Find(letterGrade).gameObject.SetActive(true);
        }

        private IEnumerator DisplayRankScreen(int[] secrets)
        {
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(ScrollDown(1f));
            yield return StartCoroutine(ScoreDisplay.TickUpScore(3f, Score));
            yield return StartCoroutine(TimeDisplay.TickUpTime(3f, (float)TimeInSeconds));
            yield return StartCoroutine(FillGauge(Ranking, 1f));
            yield return new WaitForSeconds(0.5f);
            DisplayRanking();
            yield return new WaitForSeconds(0.75f);
            if (Array.IndexOf(secrets, 1) != -1)
            {
                yield return StartCoroutine(DisplaySecrets(secrets));
            }

            yield return new WaitForSeconds(4f);
            SceneManager.Instance.level++;
            SceneManager.Instance.SwitchLevel(SceneManager.Instance.level);
        }

        private IEnumerator ScrollDown(float time)
        {
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = startPos - new Vector3(0, 18f, 0);
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / time);
                yield return null;
            }
        }

        private IEnumerator FillGauge(Ranking ranking, float progress)
        {
            int totalIterations = (int)ranking + 1;
            int iterations = 0;
            float localProgress = 0f;
            float localTime = 0f;
            float timePerQuickBar = 0.2f;
            float timePerLastBar = 1f;

            while (iterations < totalIterations)
            {
                if (iterations + 1 < totalIterations || ranking == Ranking.Overdrive)
                {
                    localProgress = 1f;
                    localTime = timePerQuickBar;
                } else
                {
                    localProgress = progress;
                    localTime = timePerLastBar;
                }

                float ellapsedTime = 0f;

                while (ellapsedTime < localTime) {
                    ellapsedTime += Time.deltaTime;
                    GaugeAnchor.localScale = new Vector3(Mathf.Lerp(0, localProgress, ellapsedTime / localTime), 1, 1);
                    yield return new WaitForEndOfFrame();
                }

                iterations += 1;
            }
        }

        private IEnumerator DisplaySecrets(int[] secrets)
        {
            yield return new WaitForSeconds(1f);
            Secrets.gameObject.SetActive(true);
            Transform[] secretIcons = new Transform[3];
            for (int i=0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.5f);
                if(secrets[i] == 1) Secrets.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}