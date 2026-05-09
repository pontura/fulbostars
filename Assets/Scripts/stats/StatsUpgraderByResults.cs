using UnityEngine;
using UnityEditor;
using Fulbo.Game;

namespace Fulbo
{
    public class StatsUpgraderByResults
    {
        GamePlay gameplayStats;
        int teamID;
        Character.types type;

        int score;
        int scoreOpponent;

        float speedAmount = 0.06f;
        float kickAmount = 9f;

        int diff = 0;
        int lastDiff = 0;
        public void Init(GamePlay gameplayStats, int teamID, Character.types type)
        {
            this.type = type;
            this.teamID = teamID;
            this.gameplayStats = gameplayStats;
        }
        public void UpdateStats()
        {
            float _speedAmount = speedAmount;
            float _kickAmount = kickAmount;

            if (type == Character.types.GOALKEEPER) _speedAmount *= 4;

            int sign = 1;
            if (diff < lastDiff) sign = -1;
            lastDiff = diff;

            gameplayStats.ChangeSpeedByResults(_speedAmount * sign);
            gameplayStats.ChangeKickForceByResults(_kickAmount * sign);

            gameplayStats.SetStatsByResults(diff);
        }

        public void OnGoal()
        {
            int team1Results = (int)Data.Instance.matchData.score.x;
            int team2Results = (int)Data.Instance.matchData.score.y;
            if (teamID == 1)
            {
                if (team1Results > team2Results) diff++; else if (team1Results < team2Results) diff--; else diff = 0;
            }
            else if (teamID == 2)
            {
                if (team1Results < team2Results) diff++; else if (team1Results > team2Results) diff--;  else diff = 0;
            }
            UpdateStats();
        }
    }
}