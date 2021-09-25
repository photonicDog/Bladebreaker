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
        public Ranking Ranking;

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
            Deaths = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Ranking FinishLevel()
        {
            return CalculateRanking(Score, TimeInSeconds, MaxCombo, Deaths);
        }

        private Ranking CalculateRanking(float score, double time, int maxCombo, int deaths)
        {
            // TODO: Specify ranking logic
            throw new System.NotImplementedException("Ranking calculation not yet implemented!");
        }
    }
}