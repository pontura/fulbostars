using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.DB;
using Fulbo.Game;

namespace Fulbo
{
    [Serializable]
    public class LevelData
    {
        public bool controlledInFixtures;
        public int cupID;
        public string team_tag; // for multiplayer_
        public int id;
        public int tier;
        public int stadium_id;
        public string size;
        public string name;
        public int duration;
        public int charactersPositions;
       // public int coinPrize;
        public int myTeamQty;
        public float idleDelay;
        public List<int> oponents;
        public ClubData clubData;
        public int referiID;
        public int pinballID;
        public int logoID;
        //public int score_win;
        public string conditionText;
        public List<Condition> conditions;

        public CharacterStats totalStatsGK;
        public CharacterStats totalStatsDef;
        public CharacterStats totalStatsMid;
        public CharacterStats totalStatsFor;

        public int duelStatsPlayer;
        public int duelStatsGK;

        public string state = "on"; // "on" = todos: sino "torneo"
        public bool locked = true;

        [Serializable]
        public class Condition
        {
            public conditionType type;
            public int value;
        }
        public void HasBeenPlayed()
        {
            locked = false;
        }
        public bool CheckForUnlockCondition()
        {
            return false;
            //print("CheckForUnlockCondition locked: " + locked + " stadium_id " + stadium_id);
            if (!locked) return false;
            bool isConditionDone = false;
            int levelID = 0;
            foreach (Condition c in conditions)
            {
                if (c.type == conditionType.WIN)
                {
                    levelID = c.value;
                    DBMatches.MatchData mData = DB.DBManager.Instance.DbMatches.GetBestMatchResults(stadium_id, levelID);
                    if (mData != null && mData.score_team2 > mData.score_team1)
                        isConditionDone = true;
                }
                if (isConditionDone && c.type == conditionType.GOALS)
                {
                    int goalsDiff = c.value;
                    DBMatches.MatchData mData = DB.DBManager.Instance.DbMatches.GetBestMatchResults(stadium_id, levelID, true);
                    //Debug.Log("Mejor partida segun esto: " + mData.score_team2 + " - " + mData.score_team1 + ". Score de " + mData.score + ".");
                    if (mData != null && mData.score_team2 >= mData.score_team1 + goalsDiff)
                        return true;
                    else
                        return false;
                }
                if (c.type == conditionType.COMPLETE)
                {
                    int stadiumID = c.value;
                    //List<LevelData> allLevelsInStadium = StoryModeData.Instance.GetAllLevelsFromStadium(stadiumID);
                    //foreach (LevelData lData in allLevelsInStadium)
                    //{
                    //    DBMatches.MatchData mData = DB.DBManager.Instance.DbMatches.GetBestMatchResults(stadiumID, lData.id);
                    //    if (mData != null && mData.score_team2 > mData.score_team1)
                    //        isConditionDone = true;
                    //    else
                    //        return false;
                    //}
                }
            }
            return isConditionDone;
        }

        public CharacterStats GetTotalStats(Character.types type)
        {
            if (type == Character.types.GOALKEEPER) return totalStatsGK;
            else if (type == Character.types.DEF) return totalStatsDef;
            else if (type == Character.types.MID) return totalStatsMid;
            else return totalStatsFor;
        }
        public int GetTotalStats()
        {
            int total = 0;
            CharactersPositions.PositionsData positionsData = Data.Instance.charactersPositions.GetPositionData(oponents.Count, charactersPositions);
            foreach (CharactersPositions.CharacterPositionData positionData in positionsData.posData)
            {
                switch (positionData.type)
                {
                    case Character.types.GOALKEEPER: total += totalStatsGK.GetTotal(false); break;
                    case Character.types.DEF: total += totalStatsDef.GetTotal(false); break;
                    case Character.types.MID: total += totalStatsMid.GetTotal(false); break;
                    default: total += totalStatsFor.GetTotal(false); break;
                }
            }
            return total;
        }
        public int GetPercentStats()
        {
            float total = 0;

            CharactersPositions.PositionsData positionsData = Data.Instance.charactersPositions.GetPositionData(oponents.Count, charactersPositions);
            foreach (CharactersPositions.CharacterPositionData positionData in positionsData.posData)
            {
                switch (positionData.type)
                {
                    case Character.types.GOALKEEPER: total += totalStatsGK.GetTotal(false); break;
                    case Character.types.DEF: total += totalStatsDef.GetTotal(false); break;
                    case Character.types.MID: total += totalStatsMid.GetTotal(false); break;
                    default: total += totalStatsFor.GetTotal(false); break;
                }
            }

            float totalCharacters = oponents.Count;
            float percent = total / totalCharacters;
            return (int)percent;
        }
        public enum conditionType
        {
            UNLOCKED,
            WIN,
            GOALS,
            COMPLETE
        }
        public int GetScoreWin()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return 100;
            LevelData ld = Data.Instance.matchData.levelData;
            int cupID = ld.cupID;
            int tier = ld.tier;
            PricesData.TierData tierData = Data.Instance.pricesData.GetScore(ld.cupID, ld.tier);
            DBCupsData.DBCupData cupData = DBManager.Instance.DbUserData.data.gameData.cups.GetCup(cupID, tier);
            if (cupData != null && cupData.timesWon>0)
            {
                int result = (int)(tierData.win + (cupData.timesWon * tierData.bonus));
                Debug.Log("tierData.bonus:" + tierData.bonus + "  timesCupWon:" + cupData.timesWon + "  tierData.win:" + tierData.win + "  result:" + result);
                return result;
            }
            return (int)tierData.win;
        }

        public int GetBonus()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return 0;
            LevelData ld = Data.Instance.matchData.levelData;
            PricesData.TierData tierData = Data.Instance.pricesData.GetScore(ld.cupID, ld.tier);

            DBCupsData.DBCupData cupData = DBManager.Instance.DbUserData.data.gameData.cups.GetCup(cupID, tier);
            if (cupData != null && cupData.timesWon > 0)
                return (int)tierData.bonus * cupData.timesWon;
            return 0;
        }
    }
}
