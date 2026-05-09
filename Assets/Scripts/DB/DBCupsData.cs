using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using static Fulbo.CupsData;
using Fulbo.Onboarding;

namespace Fulbo.DB
{
    [Serializable]
    public class DBCupsData
    {
        public int activeCup = 0;
        public List<DBCupData> cupsPlayed;
        public float lifesBurned;
        public float totalLifes = 3;
        public bool hasLoseLife;
        public bool hasUnlockedTier;
        public bool hasReplayCups;
        public bool unlockedCup;
        public bool hasLoseCup;

        public void Init()
        {
            if(cupsPlayed == null)
                cupsPlayed = new List<DBCupData>();

            if (activeCup > 0) {
                DBCupData dcd = GetActiveCupData();
                if (dcd != null) {
                    CupsData.Instance.ForceOpen(dcd.cupID, dcd.tier);
                }
            }
        }

        [Serializable]
        public class DBCupData
        {
            public int cupID;
            public int tier = 1;
            public int timesWon;
            public List<DBCupLevelData> levels;
            public void Won() { timesWon++; } 
        }
        [Serializable]
        public class DBCupLevelData
        {
            public int score;
            public int opp_score;
            public int levelID;
        }
        public int GetLevelActiveID()
        {
            if(Data.Instance.matchData.levelData.id>0)
                return Data.Instance.matchData.levelData.id;
            else {
                return GetCurrentCupLevel();
            }
        }

        public List<DBCupData> GetPlayedCupsID()
        {
            List<DBCupData> all = new List<DBCupData>();
            if (cupsPlayed == null) return all;
            foreach (DBCupData d in cupsPlayed)
            {
                //Debug.Log("GetPlayedCupsID cupID: " + d.cupID);
                all.Add(d);
            }
            return all;
        }
        public int GetLevelDataActive()
        {
            DBCupData cupData = GetCupActive();
            if (cupData.levels == null || cupData.levels.Count == 0) return 0;
           // Debug.Log("_____________LAST GetLevelDataActive: " + cupData.levels[cupData.levels.Count - 1].levelID);
            return cupData.levels[cupData.levels.Count - 1].levelID;
        }
        public DBCupLevelData GetLastLevelForCup(int cupID, int tier)
        {
            DBCupData cup = GetCup(cupID, tier);
            if (cup != null && cup.levels.Count>0 && tier == cup.tier)
                return cup.levels[cup.levels.Count - 1];
            return null;
        }
        public DBCupLevelData GetLevelDataForCup(int cupID, int tier, int levelID)
        {
            DBCupData cupData = GetCup(cupID, tier);
            if (cupData == null || cupData.levels == null || cupData.levels.Count == 0) return null;
            foreach (DBCupLevelData l in cupData.levels)
            {
                if (l.levelID == levelID)
                    return l;
            }
            return null;
        }
        public DBCupData GetCup(int cupID, int tier)
        {
            if (cupsPlayed == null || cupsPlayed.Count == 0)
                return null;
            foreach (DBCupData d in cupsPlayed)
            {
              //  Debug.Log("cupID:" + d.cupID + "  tier:" + d.tier);
                if (d.cupID == cupID && tier == d.tier)
                {
                    return d;
                }
            }
            return null;
        }
        DBCupData GetCupActive()
        {
            if(cupsPlayed == null || cupsPlayed.Count == 0)
                cupsPlayed = new List<DBCupData>();
            else {
                activeCup = Data.Instance.matchData.levelData.cupID;
                int tier = Data.Instance.matchData.levelData.tier;
                DBCupData cup = GetCup(activeCup, tier);
                if (cup != null)  return cup;
            }
            DBCupData d = AddNewCup(Data.Instance.matchData.levelData.cupID, Data.Instance.matchData.levelData.tier);
            return d;
        }
        DBCupData AddNewCup(int cupID, int tier)
        {
           // Debug.Log("AddNewCup " + cupID + " tier " + tier);
            DBCupData newCup = new DBCupData();
            newCup.cupID = cupID;
            newCup.tier = tier;
            activeCup = newCup.cupID;
            cupsPlayed.Add(newCup);
            newCup.levels = new List<DBCupLevelData>();
            return newCup;
        }
        
        public void SaveResults(int otherScore, int myScore)
        {
            Debug.Log("SaveResults otherScore " + otherScore + " myScore: " + myScore);
            activeCup = Data.Instance.matchData.levelData.cupID;
            int tier = Data.Instance.matchData.levelData.tier;

            DBCupData cupData;
          //  DBCupData cupData = GetCup(activeCup, tier);
          // if (cupData == null)
            cupData = GetCupActive();

            int levelID = GetLevelActiveID();
            int cupID = Data.Instance.matchData.levelData.cupID;
            DBCupLevelData ld = GetLevelDataForCup(cupID, tier, levelID);
            if (ld == null) //if no level yet created creates one:
            {
                ld = new DBCupLevelData();
                Debug.Log("SaveResults levelID " + levelID + " ld: " + ld + " tier: " + cupData.tier);
                cupData.levels.Add(ld);                
            }
            ld.levelID = levelID;
            ld.score = myScore;
            ld.opp_score = otherScore;

            // if tutorial dont lose life
            if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.SECOND_MATCH_PLAYED)) return;
            if (otherScore > myScore) {
                lifesBurned++;
                if (!hasLoseLife) {
                    Debug.Log("##hasLoseLife first time");
                    hasLoseLife = true;
                    Data.Instance.onBoardingManager.SetFirsTimeLose();
                }
            } else if (otherScore == myScore)
                lifesBurned++;
            if (lifesBurned >= totalLifes) {
                if (!hasLoseCup)
                    hasLoseCup = true;
                ResetActiveCup();
            }
        }
                
        public void InitCup(CupData c)
        {
            activeCup = c.id;
            totalLifes = c.life;
            lifesBurned = 0;
        }
        public void UnlockNewCup(int cupID, int tier)
        {
           // Debug.Log("UnlockNewCup " + cupID + " tier " + tier);
            activeCup = cupID;
            DBCupData d = GetCup(cupID, tier);
            if (d == null)
                AddNewCup(cupID, tier);
        }
        void ResetActiveCup()
        {
            activeCup = 0;
        }
        public bool IsLastMatch()
        {
            DBCupData cupData = GetCupActive();
            int levelID = GetLevelActiveID();
            Debug.Log("levels: " + cupData.levels.Count + " levelID: " + levelID);
            if (cupData.levels[cupData.levels.Count - 1].levelID == levelID)
                return true;
            else
                return false;
        }

        public bool NoMoreLifes() {
            bool noMoreLifes = lifesBurned >= totalLifes;
            Debug.Log("lifesBurned: " + lifesBurned + " >= totalLifes: " + totalLifes);
            return noMoreLifes;
        }

        public bool IsPlayingLastLife()
        {
            bool isPlayingLastMatch = lifesBurned + 1 >= totalLifes;
           // Debug.Log("isPlayingLAstMAtch: " + isPlayingLastMatch);
            return isPlayingLastMatch;
        }
        public bool IsCupCompleted(int cupID, int tier)
        {
           // Debug.Log("IsCupCompleted cupID: " + cupID + " tier: " + tier);
            DBCupData d = GetCup(cupID, tier);
           // Debug.Log("d.levels.Count: " + d.levels.Count);
            if (d == null) return false;
           // Debug.Log("GetAllLevelsFromCup: " + CupsData.Instance.GetAllLevelsFromCup(cupID, tier).Count);
            if (d.levels.Count == CupsData.Instance.GetAllLevelsFromCup(cupID, tier).Count)
                return true;
            return false;
        }
        public int GetTotalMatchesPlayedForCup(int cupID, int tier)
        {
            DBCupData d = GetCup(cupID, tier);
            if (d == null || d.levels == null) return 0;
            return d.levels.Count;
        }
        public void AddNewLives(int qty)
        {
            lifesBurned -= qty;
            if (lifesBurned < 0) lifesBurned = 0;
            Events.ResetLifesTo(totalLifes - lifesBurned);
        }
        public void ResetProgress()
        {
          //  Debug.Log("Progress deleted!");
            foreach(DBCupData d in cupsPlayed)
            {
                d.levels.Clear();
            }
            lifesBurned = 0;
        }
        public bool IsStillPlayingFirstCup()
        {
            if (cupsPlayed == null || cupsPlayed.Count == 0)
                return true;
            return cupsPlayed.Count < 2;
        }

        public int GetTimesWon(int cupID, int tier)
        {
            DBCupData d = GetCup(cupID, tier);
            if (d == null)
                return 0;
            return d.timesWon;
        }
        public void CupWon(int cupID, int tier)
        {
            Debug.Log("CupWon " + cupID + " tier " + tier);
            DBCupData d = GetCup(cupID, tier);
            d.Won();
        }

        public DBCupData GetActiveCupData() {
            if (activeCup == 0) return null;
            return cupsPlayed.Find(x => x.cupID == activeCup && x.levels.Count > 0);
        }        

        public int GetCurrentCupLevel() {
            DBCupData dcd = GetActiveCupData();
            int levelID = 0;
            if (dcd != null) {
                List<DBCupLevelData> levels = dcd.levels.FindAll(x => (x.score > x.opp_score)).OrderBy(x => x.levelID).ToList();
                if (levels.Count>0)
                    return levels[levels.Count - 1].levelID;
            }
            return levelID;
        }

        public bool HasUnlockedCupTier(int cupID) {
            if (hasUnlockedTier)
                return false;
            return cupsPlayed.Find(x => x.cupID == cupID && x.tier==2 && x.levels.Count == 0)!=null;
        }

        public void UnlockAll() // Cheat for unlocking all cups:
        {
            cupsPlayed.Clear();
            List<CupData> cupData = CupsData.Instance.all;
            foreach(CupData c in cupData)
            {
                DBCupData dbCup = new DBCupData();
                dbCup.cupID = c.id;
                dbCup.tier = c.tier;
                dbCup.timesWon = 1;
                dbCup.levels = new List<DBCupLevelData>();
                List<LevelData> levels = CupsData.Instance.GetAllLevelsFromCup(c.id, c.life);
                foreach (LevelData l in levels)
                {
                    int levelID = 0;
                    DBCupLevelData levelData = new DBCupLevelData();
                    levelData.levelID = levelID;
                    levelData.opp_score = 0;
                    levelData.score = 5;
                    dbCup.levels.Add(levelData);
                }
                cupsPlayed.Add(dbCup);
            }
        }
    }   
}
