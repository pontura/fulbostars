using UnityEngine;
using System.Collections;
using static Fulbo.MatchStats;
using System.Collections.Generic;
using Fulbo.Onboarding;

namespace Fulbo
{
    public class MatchRewards
    {
        public int score;
        public int totalRewards;
        public int goalDiff;
        public int goalDiffValue;
        public int coinsReward = 0;
        public float ballPosession;

        MatchStats matchStats;
        public TeamStats team1;
        public TeamStats team2;
        LevelBonusData levelBonusData;

        public List<float> scoresToAdd;
        public int bonus;
        public int bonusTotal;
        bool dataSavedToDB;
        bool userdataSavedToDB;

        public void Reset()
        {
            dataSavedToDB = false;
            userdataSavedToDB = false;
            scoresToAdd = new List<float>();
            score = 0;
            totalRewards = 0;
            goalDiff = 0;
            goalDiffValue = 0;
            ballPosession = 0;
            bonus = 0;
            bonusTotal = 0;
        }
        public void Calculate()
        {
            Reset();
            levelBonusData = Data.Instance.levelBonusData;
            matchStats = Data.Instance.GetComponent<MatchStats>();

            MatchData matchData = Data.Instance.matchData;
            goalDiff = (int)(matchData.score.y - matchData.score.x);

            LevelData levelData = CupsData.Instance.GetActualLevel();
            bonus = 0;
            bonusTotal = 0;

            if (goalDiff < 0) // si perdes resetea todo el score:
            {
                float percent = Data.Instance.settings.GetSetting("percent_lose_score");
                score = (int)(levelData.GetScoreWin() * percent);
            }
            else if (goalDiff == 0)
            {
                float percent = Data.Instance.settings.GetSetting("percent_draw_score");
                score = (int)(levelData.GetScoreWin() * percent);
            }
            else
            {
                score = levelData.GetScoreWin();
                bonusTotal = levelData.GetBonus();
            }

            if (Fulbo.UI.Cheats.Cheats.moneyCheat)
            {
                score += 100000;
                Fulbo.UI.Cheats.Cheats.moneyCheat = false;
            }

            ballPosession = matchStats.teams[1].ball_possesion / (matchStats.teams[1].ball_possesion + matchStats.teams[0].ball_possesion) * 100;
            if (ballPosession < 0) ballPosession = 50;


            team1 = matchStats.teams[0];
            team2 = matchStats.teams[1];

            AddScore(LevelBonusData.parameters.Ball_Possession, ballPosession, score);
            AddScore(LevelBonusData.parameters.Effective_Passes, team1.kicks_passes, score);
            AddScore(LevelBonusData.parameters.Shoots, team1.kicks_to_goal, score);
            AddScore(LevelBonusData.parameters.Referee_Hits, team1.balls_to_referi, score);
            AddScore(LevelBonusData.parameters.Center_Kicks, team1.centros, score);
            AddScore(LevelBonusData.parameters.Effective_Tackles, team1.tackles, score);
            AddScore(LevelBonusData.parameters.Saves, team1.saves, score);

            scoresToAdd.Add(score);

            totalRewards = 0;
            foreach (int a in scoresToAdd)
                totalRewards += a;

            goalDiffValue = (int)(Mathf.Round(Data.Instance.levelBonusData.GetScoreByGoalDiff(matchData.score.y, matchData.score.x, score)));
            coinsReward = GetRewardByCoinsGrabbed();
            

            totalRewards += goalDiffValue;
            totalRewards += coinsReward;

            //Debug.Log("___________score:" + score + 
            //    "  totalRewards: " + totalRewards 
            //    + "  goalDiffValue: " + goalDiffValue
            //    + "  coinsReward: " + coinsReward
            //     + "  bonusTotal: " + bonusTotal
            //    + "  bonus: " + bonus);

            if (totalRewards < 0) totalRewards = 0;

            Data.Instance.matchData.SaveCupDataOnGameOver(DataSaved);

            int rewards_extra = totalRewards - score; // le mando solo los extra:
            Data.Instance.myTeam.myTeamData.GameOver(rewards_extra, UserDataSaved);
        }
        public bool AllReady()
        {
            if (userdataSavedToDB && dataSavedToDB) return true;
            return false;
        }
        void UserDataSaved()
        {
            userdataSavedToDB = true;
        }
        void DataSaved(bool isOk, string error)
        {
            if(isOk)
                dataSavedToDB = true;
            else
                Data.Instance.matchData.SaveCupDataOnGameOver(DataSaved);
        }
        bool IsFirstGame()
        {
            if (Data.Instance.onBoardingManager.IsBoardingStep(OnBoardingManager.BoardingStepStates.FIRST_MATCH_PLAYED))
                return true;
            return false;
        }
        void AddScore(LevelBonusData.parameters param, float value, float total)
        {
            int newScore = levelBonusData.GetScore(param, value, (int)total);
            scoresToAdd.Add(newScore);
        }
        int GetRewardByCoinsGrabbed()
        {
            float coins_grabbed = Data.Instance.GetComponent<MatchStats>().GetStats(2).coins_grabbed;
            float coinsReward = coins_grabbed;// CupsData.Instance.GetActualLevel().coinPrize;
            return (int)coinsReward;
        }
       
    }
}
