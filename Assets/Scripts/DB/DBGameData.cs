using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using static Fulbo.Game.CharactersPositions;
using Fulbo.Game;
using System.Linq;

namespace Fulbo.DB
{
    public class DBGameData
    {
        [Serializable]
        class GData
        {
            public string hash;
            public string game_data;
        }
        [Serializable]
        public class Content
        {
            public int reviewRequestState;
            public string review_day;

            public string notifications; //separated by comas:
            public int tutorialStep; //on boarding steps:

            public DBFormationSave formation5;
            public DBFormationSave formation6;
            public DBFormationSave formation7;
            public DBFormationSave formation8;
            public DBFormationSave formation9;
            public DBFormationSave formation10;
            public DBFormationSave formation11;

            public DBEnergyData energyData;
            public DBExpertieseData expertieseData;
            public DBCupsData cups;
            public bool firstGalacticCupSign;
            public bool levelsOnFirstMatchPlayedSign;
            public bool hasVisitedMyTeam;
            public bool knowStatHints;
            public bool knowTraining;
            public bool knowCupsReplay;
            public bool hasVisitedUpgradableStat;
            public bool hasVisitedShop;
            public bool hasVisitedDailyExtras;

            public void Init()
            {
                if (cups == null)
                {
                    cups = new DBCupsData();
                    cups.Init();
                }
                if (energyData == null)
                    energyData = new DBEnergyData();
                if (expertieseData == null)
                    expertieseData = new DBExpertieseData();
            }

            public DBEnergyData GetEnergyData(int energyInitial)
            {
                if (energyData == null) // if init:
                {
                    energyData = new DBEnergyData();
                    energyData.ResetEnergyTo(energyInitial);
                } else if (energyData != null && (energyData.IsANewHour() || energyData.IsANewDay())) // one day has passed at least 
                {
                    energyData.RefillEnergyByHour();
                }
                return energyData;
            }
            public void VideoSeen()
            {
                energyData.videosSeen++;
            }
            public void OnEnergyChanged(int value, System.Action<bool,string> OnSuccess)
            {
                energyData.Add(value);                
                DB.DBManager.Instance.DbGameData.Put(OnSuccess);
            }
            public void SaveReview(int value, System.Action<bool, string> OnSuccess)
            {
                if(value == 1)
                    reviewRequestState = value;
                if (value == 3)
                    review_day = Utils.Today(DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD);

                DB.DBManager.Instance.DbGameData.Put(OnSuccess);
            }
            public DBFormationSave GetFormation(int qtyPlayers)
            {
                Debug.Log("GetFormation: " + qtyPlayers);
                if (qtyPlayers == 5 && formation5 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation5;
                else if (qtyPlayers == 6 && formation6 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation6;
                else if (qtyPlayers == 7 && formation7 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation7;
                else if (qtyPlayers == 8 && formation8 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation8;
                else if (qtyPlayers == 9 && formation9 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation9;
                else if (qtyPlayers == 10 && formation10 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation10;
                else if (qtyPlayers == 11 && formation11 != null) return DB.DBManager.Instance.DbUserData.data.gameData.formation11;
                else return null;
            }
            public void Swap(int id_from, int id_to, int formationQty)
            {
                //Debug.Log("SWAP id_from: " + id_from + " to: " + id_to  + " in formationQty: " + formationQty);

                DBFormationSave d = GetFormation(formationQty);

                if (d == null) { Debug.LogError("Swap, but no formation for " + formationQty); return; }

                //Debug.Log("Swap " + id_from + " to " + id_to + " formation: " + formationQty);
                //Debug.Log("d.formation.Length " + d.formation.Length);

                int arrPos = -1;
                int id = 0;
                foreach (DBFormationSave.DBFormationChar dc in d.formation)
                {
                    if (dc.uniqueID == id_to)
                        arrPos = id;
                    id++;
                }

                foreach (DBFormationSave.DBFormationChar dc in d.formation)
                {
                    if (dc.uniqueID == id_from)
                    {
                        if(arrPos>-1)
                        {
                            int newID = dc.uniqueID;
                            d.formation[arrPos].uniqueID = newID;
                        }
                        dc.uniqueID = id_to;
                       // Debug.Log("YES " + id_from + " to " + id_to + " formation: " + formationQty);
                        return;
                    }
                }
                DB.DBManager.Instance.DbGameData.PutFormation(d.formation.Length, d.ToPosition(), null);
            }
            public void OnCharacterSold(int uniqueId) // Delete every formation with character:
            {
              //  Debug.Log("OnCharacterSold " + uniqueId);
                int totalPlayers = 5;
                while (totalPlayers < 12)
                {
                    ChangePlayerBySold(uniqueId, totalPlayers);
                    totalPlayers++;
                }
            }
            void ChangePlayerBySold(int uniqueId, int formationID)
            {
               // Debug.Log("OnCharacterSold " + uniqueId + " formationID: " + formationID);
                bool isOnTeam = FormationHasCharacter(uniqueId, formationID);
              //  Debug.Log(uniqueId + " isOnTeam " + isOnTeam);
                if (!isOnTeam) return;
                if(IsGoalkeeperForFormation(uniqueId, formationID))
                {
                    int newGoalkeeper = DB.DBManager.Instance.DbUserData.data.players_goalkeepers[0].id;
                    Swap(uniqueId, newGoalkeeper, formationID);
                    return;
                }

                int totalPlayers = DB.DBManager.Instance.DbUserData.data.players_characters.Count;

               // Debug.Log("totalPlayers " + totalPlayers);
                if (totalPlayers < formationID-1)
                    DeleteFormation(formationID);
                else
                {
                    int newUniqueID = GetReplacement(uniqueId, formationID);
                    Swap(uniqueId, newUniqueID, formationID);
                }
            }
            int GetReplacement(int uniqueId, int formationID)
            {
                List<DBUserData.DBCharacterData> availablePlayers = DB.DBManager.Instance.DbUserData.data.players_characters;

                foreach (DBUserData.DBCharacterData cData in availablePlayers)
                {
                    if (!FormationHasCharacter(cData.id, formationID))
                        return cData.id;
                }
                Debug.LogError("No more replacements for " + uniqueId + " in formation : " + formationID);
                return 0;
            }
            void DeleteFormation(int formationID)
            {
               // Debug.Log("DeleteFormation " + formationID);
                DBFormationSave dSave = DB.DBManager.Instance.DbUserData.data.gameData.GetFormation(formationID);
                ResetFormation(dSave);
            }
            bool IsGoalkeeperForFormation(int uniqueId, int formationID)
            {
                int id = 0;
                DBFormationSave dSave = DB.DBManager.Instance.DbUserData.data.gameData.GetFormation(formationID);
                foreach (DBFormationSave.DBFormationChar cChar in dSave.formation)
                {
                    if (cChar.uniqueID == uniqueId && id == 0)
                        return true;
                    id++;
                }
                return false;
            }
            bool FormationHasCharacter(int uniqueId, int formationID)
            {
                //  Debug.Log("OnCharacterSold " + uniqueId + " formationID: " + formationID);
                DBFormationSave dSave = DB.DBManager.Instance.DbUserData.data.gameData.GetFormation(formationID);
                foreach (DBFormationSave.DBFormationChar cChar in dSave.formation)
                {
                    if (cChar.uniqueID == uniqueId)
                        return true;
                }
                return false;
            }
            public void ResetFormation(DBFormationSave dSave)
            {
              //  Debug.Log("Reset Formation:" + dSave.formation.Length + " players");
                dSave.formation = new DBFormationSave.DBFormationChar[0];
            }
            public void AddNotificationRead(int _id, System.Action<bool, string> OnSuccess)
            {
                string id = _id.ToString();
                if (notifications == null || notifications.Length <= 1)
                {
                    notifications = id + ",";
                }
                else
                {
                    string[] arr = notifications.Split(","[0]);
                    foreach (string s in arr)
                        if (s == id)
                            return;
                    notifications += id + ",";
                }
                DBManager.Instance.DbGameData.Put("notifications", notifications, OnSuccess);
            }
        }

        [Serializable]
        public class DBFormationSave
        {
            public DBFormationChar[] formation;
            [Serializable]
            public class DBFormationChar
            {
                public int uniqueID;
                public float[] pos;
            }

            public PlayerPositionData ToPosition()
            {
                PlayerPositionData save = new PlayerPositionData();
                if (formation == null || formation.Length == 0) return null;
                foreach(DBFormationChar character in formation)
                {
                    CharacterPositionData charPosData = new CharacterPositionData();
                    charPosData.pos = character.pos;
                    charPosData.uniqueId = character.uniqueID;
                    save.posData.Add(charPosData);
                }

                return save;
            }
        }


        public PlayerPositionData GetFormation(int qtyPlayers)
        {
            if (qtyPlayers < 5) qtyPlayers = 5;
            Debug.Log("get formation " + qtyPlayers + " players");
            DBFormationSave dSave = DB.DBManager.Instance.DbUserData.data.gameData.GetFormation(qtyPlayers);
            if (dSave != null && ErrorInFormation(dSave))
            {
                DB.DBManager.Instance.DbUserData.data.gameData.ResetFormation(dSave);
                return null;
            }
            else if (dSave == null) return null;
            return dSave.ToPosition();
        }
        //##ErrorInFormation
        bool ErrorInFormation(DBFormationSave dSave)
        {
            bool formationChanged = false;
            //Debug.Log("# Check if ErrorInFormation " + dSave.formation.Length);
            if (dSave.formation == null || dSave.formation.Length == 0) return true;
            List<int> players = new List<int>();
            int count = 0;
            foreach (DBFormationSave.DBFormationChar cpd in dSave.formation)
            {
                foreach (int id in players) // que no esté 2 veces:
                    if (id == cpd.uniqueID)
                        return true;

                if(count == 0) {
                    List<DBUserData.DBCharacterData> gks = DBManager.Instance.DbUserData.data.GetCharacters(count == 0);
                    DBUserData.DBCharacterData chD = gks.Find(x => x.id == cpd.uniqueID);
                    if (chD == null) {
                        if (gks.Count > 0) {
                            cpd.uniqueID = gks[0].id;
                            players.Add(gks[0].id);
                            formationChanged = true;
                        } else {
                            Debug.LogError("ErrorInFormation no hay arquero con Id: " + cpd.uniqueID);
                            return true;
                        }
                    }
                }

                if (DB.DBManager.Instance.DbUserData.data.GetPlayerByID(cpd.uniqueID) == null) // que todavía exista:
                {
                    List<DBFormationSave.DBFormationChar> l = dSave.formation.ToList<DBFormationSave.DBFormationChar>();
                    List<DBUserData.DBCharacterData> all = DBManager.Instance.DbUserData.data.GetCharacters(count == 0);
                    List<DBUserData.DBCharacterData> selecteds = all.FindAll(x => l.Find(y=>y.uniqueID==x.id)==null);
                    if (selecteds != null) {
                        cpd.uniqueID = selecteds[0].id;
                        players.Add(selecteds[0].id);
                        formationChanged = true;
                    } else {
                        Debug.LogError("ErrorInFormation No hay: " + cpd.uniqueID);
                        return true;
                    }
                } else
                    players.Add(cpd.uniqueID);
                count++;
            }
            if(formationChanged)
                DB.DBManager.Instance.DbGameData.PutFormation(dSave.formation.Length, dSave.ToPosition(), null);
            return false;
        }
        public void ParseContent(string game_data)
        {
            if (game_data == null || game_data.Length < 2) return;
           // Debug.Log("DBGameData PARSE: " + game_data);
            try
            {
                Content c = JsonUtility.FromJson<Content>(game_data);
                DBManager.Instance.DbUserData.data.gameData = JsonUtility.FromJson<Content>(game_data);
                DBManager.Instance.DbUserData.data.gameData.cups.Init();
            }
            catch
            {
                Debug.Log("Not json");
            }
        }
        System.Action OnSuccess;
        Content content;
        public void Put(string key, string value, System.Action<bool, string> OnSuccess)
        {
            switch (key)
            {
                case "tutorialStep":
                    DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep = int.Parse(value); break;
                case "tierUnlocked":
                    DB.DBManager.Instance.DbUserData.data.gameData.cups.hasUnlockedTier = bool.Parse(value); break;
                case "replayCups":
                    DB.DBManager.Instance.DbUserData.data.gameData.cups.hasReplayCups = bool.Parse(value); break;
                case "unlockedCup":
                    DB.DBManager.Instance.DbUserData.data.gameData.cups.unlockedCup = bool.Parse(value); break;
                case "notifications":
                    DB.DBManager.Instance.DbUserData.data.gameData.notifications = value; break;
                case "myTeam":
                    DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedMyTeam = bool.Parse(value); break;
                case "upgradableStat":
                    DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedUpgradableStat = bool.Parse(value); break;
                case "shop":
                    DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedShop = bool.Parse(value); break;
                case "statHints":
                    DB.DBManager.Instance.DbUserData.data.gameData.knowStatHints = bool.Parse(value); break;
                case "training":
                    DB.DBManager.Instance.DbUserData.data.gameData.knowTraining = bool.Parse(value); break;
                case "cup_replay":
                    DB.DBManager.Instance.DbUserData.data.gameData.knowCupsReplay = bool.Parse(value); break;
                case "galacticCupSign":
                    DB.DBManager.Instance.DbUserData.data.gameData.firstGalacticCupSign = bool.Parse(value); break;
                case "levelsFirstMatchPlayedSign":
                    DB.DBManager.Instance.DbUserData.data.gameData.levelsOnFirstMatchPlayedSign = bool.Parse(value); break;
                case "hasVisitedDailyExtras":
                    DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedDailyExtras = bool.Parse(value); break;
            }
            Put(OnSuccess);
        }
        public void PutFormation(int qtyPlayers, PlayerPositionData value, System.Action<bool, string> OnSuccess)
        {
            if (qtyPlayers == 5) DB.DBManager.Instance.DbUserData.data.gameData.formation5 = value.ToDBSave();
            else if (qtyPlayers == 6) DB.DBManager.Instance.DbUserData.data.gameData.formation6 = value.ToDBSave();
            else if (qtyPlayers == 7) DB.DBManager.Instance.DbUserData.data.gameData.formation7 = value.ToDBSave();
            else if (qtyPlayers == 8) DB.DBManager.Instance.DbUserData.data.gameData.formation8 = value.ToDBSave();
            else if (qtyPlayers == 9) DB.DBManager.Instance.DbUserData.data.gameData.formation9 = value.ToDBSave();
            else if (qtyPlayers == 10) DB.DBManager.Instance.DbUserData.data.gameData.formation10 = value.ToDBSave();
            else if (qtyPlayers == 11) DB.DBManager.Instance.DbUserData.data.gameData.formation11 = value.ToDBSave();
            else
            {
                Debug.LogError("No qty available " + qtyPlayers);
                if (OnSuccess != null)
                    PutFormation(qtyPlayers, value, OnSuccess);
            }
            Put(OnSuccess);
        }
        public void Put(System.Action<bool, string> OnSuccess)
        {
            string url = DBManager.Instance.UrlRegister + DBManager.Instance.Email + "/gameData";
            Debug.Log("[DBGameData] Put : " + url);
            WWWForm form = new WWWForm();

            string hashText =
                DBManager.Instance.Email +
                DBManager.HASH_SALT2 +
                DBManager.Instance.DbUserData.data.id;

            GData tData = new GData();
            tData.game_data = JsonUtility.ToJson(DB.DBManager.Instance.DbUserData.data.gameData);            
            tData.hash = Utils.SHA(hashText);
            string json = JsonUtility.ToJson(tData);
            //  DBManager.Instance.Request(url, json, OnSuccessDone, "PUT", Data.Instance.texts.Get("http_updating_user"));
            DBManager.Instance.Request(url, json, OnSuccess, "PUT", Data.Instance.texts.Get("http_updating_user"));
        }
        //void OnSuccessDone(bool isOk, string response)
        //{
        //    if (isOk)
        //    {
        //        if (OnSuccess != null)
        //        {
        //            OnSuccess();
        //            OnSuccess = null;
        //        }
        //    }
        //    else
        //    {
        //        Events.OnPopup(response, null);
        //       // Put(OnSuccess); // re-try saving cups Data!
        //    }
        //    // DBManager.Instance.DbUserData.LoadUserData(OnLoaded);
        //}
        //void OnLoaded()
        //{
        //    if (OnSuccess != null)
        //    {
        //        OnSuccess();
        //        OnSuccess = null;
        //    }
        //}          
    }
}