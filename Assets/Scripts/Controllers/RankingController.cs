using Assets.Scripts.Types.Enums;
using System;
using Unity.Mathematics;
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

        public float MaxMultiplier = 5;
        public int MaxComboForMaxMultiplier = 15;

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

        public void FinishLevel()
        {
            Ranking = CalculateRanking(Score, TimeInSeconds, MaxCombo, Deaths);
        }

        private Ranking CalculateRanking(float score, double time, int maxCombo, int deaths)
        {
            // TODO: Specify ranking logic
            throw new System.NotImplementedException("Ranking calculation not yet implemented!");
        }

        public void IncrementCombo()
        {
            CurrentCombo++;
            if (CurrentCombo < MaxCombo)
            {
                MaxCombo = CurrentCombo;
            }
        }
        public void EndCombo()
        {
            CurrentCombo = 0;
        }

        public void AddScore(float addedScore)
        {
            Score += addedScore;
        }

        public void AddScoreEnemyDefeated()
        {
            AddScore(100 * GetCurrentMultiplier());
        }

        public void AddScoreWeaponBreak()
        {
            AddScore(20 * GetCurrentMultiplier());
        }

        public void AddScoreFoundSecret()
        {
            AddScore(500);
        }

        public void AddScoreFinishedCombo()
        {
            AddScore(10 * CurrentCombo * GetCurrentMultiplier());
            CurrentCombo = 0;
        }

        public int GetCurrentMultiplier()
        {
            // This maths scales the multiplier increment with the highest combo that counts for the multiplier
            // and the maximum multiplier the player should have itself
            return (int)Math.Min(math.floor(1 + CurrentCombo * (MaxMultiplier - 1 / MaxComboForMaxMultiplier)), MaxMultiplier);
        }
    }
}