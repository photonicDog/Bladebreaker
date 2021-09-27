using Assets.Scripts.Types.Enums;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class RankingController : MonoBehaviour
    {
        public float Score;
        public double TimeInSeconds;
        public int MaxCombo;
        public int CurrentCombo;
        public int Deaths;
        public int EnemiesDefeated;
        public Ranking Ranking;

        public int LevelEnemyTotal;
        public int ScoreThreshold;
        public int ComboThreshold;
        public double ParTime;
        public double BogeyTime;

        private static RankingController _instance;
        public static RankingController Instance { get { return _instance; } }

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
            Score = 0;
            TimeInSeconds = 0;
            MaxCombo = 0;
            CurrentCombo = 0;
            EnemiesDefeated = 0;
            Deaths = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Ranking FinishLevel()
        {
            return CalculateRanking(Score, TimeInSeconds, MaxCombo, Deaths, EnemiesDefeated);
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
    }
}