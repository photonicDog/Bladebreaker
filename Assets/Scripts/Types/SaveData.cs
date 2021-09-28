using Assets.Scripts.Types.Enums;
using System.Collections.Generic;

namespace Assets.Scripts.Types
{
    [System.Serializable]
    public class SaveData
    {
        public List<Ranking> Rankings;
        public List<int> Scores;
        public List<float> Times;
        public List<int> Deaths;
        public List<int> MaxCombos;
        public List<int[]> Secrets;
        public List<int> EnemiesDefeated;
        public int LastClearedLevel;

        private const int totalLevelCount = 3;
        public SaveData()
        {
            Rankings = new List<Ranking>();
            Scores = new List<int>();
            Times = new List<float>();
            Deaths = new List<int>();
            MaxCombos = new List<int>();
            Secrets = new List<int[]>();
            EnemiesDefeated = new List<int>();
            LastClearedLevel = 0;

            for(int i = 0; i < totalLevelCount; i++)
            {
                Rankings.Add(Ranking.None);
                Scores.Add(0);
                Times.Add(0f);
                Deaths.Add(0);
                MaxCombos.Add(0);
                EnemiesDefeated.Add(0);
                Secrets.Add(new int[] { 0, 0, 0 });
            }
        }
    }
}
