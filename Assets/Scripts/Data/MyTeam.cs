using Fulbo.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo
{
    public class MyTeam : DataLoader
    {
        public int forceStats = 0;// te fuerza los stats para DEBUG
        public MyTeamData myTeamData;

        [SerializeField] int tutorialStep;

        public string teamName;

        public ClubData clubData;

        Action OnReady;

        public override void Reset()
        {
            base.Reset();
            //goalkeepers.Clear();
            //characters.Clear();
            //selectedTeam.Clear();
            //positions.Clear();
            //consumables.Clear();
            //consumables_gk.Clear();
            //teamName = "";
            //totalWonScore = 0;
            //tutorialStep = 0;
        }
        public override void LoadData(Action OnReady)
        {
            myTeamData = new MyTeamData();
            myTeamData.Init(this);
            this.OnReady = OnReady;
            LoadSavedData();

            string u;
            if (Data.Instance.mode == Data.modes.PARTYMODE)
                u = "demo";
            else
                u = Social.localUser.userName.ToUpper();
#if UNITY_EDITOR
            u = DBManager.Instance.user.ToString();
#endif

            if (clubData.name_abr == null || clubData.name_abr.Length < 1)
                Data.Instance.myTeam.SetTeamName(u);
        }
        public bool IsEmpty()
        {
          //  if (PlayerPrefs.GetString("saved_characters", "") == "")
          if(DBManager.Instance.DbUserData.data.players_characters.Count <=0 )
                return true;
            return false;
        }
        void LoadSavedData()
        {
            DB.DBUserData.UserData userData = DB.DBManager.Instance.DbUserData.data;

            if (userData.shortName != "")
            {
                teamName = userData.user;
                clubData.name_abr = userData.shortName;
                clubData.SetDataFromString(userData.style);
            } else if (userData.user != "")
            { 
                clubData.name_abr = Data.Instance.myTeam.SetTeamName(userData.user);
            }

            if (DBManager.Instance.DbUserData.data.gamesPlayed > 0)
            { 
                tutorialStep = 1000;
            }

            DB.DBUserData.UserData d = DBManager.Instance.DbUserData.data;

            OnReady();

        }
        
        public string SetTeamName(string _teamName)
        {
            teamName = _teamName.ToUpper();
            if (teamName.Length>2)
            {
                clubData.name_abr = "";
                clubData.name_abr += teamName[0];
                clubData.name_abr += teamName[UnityEngine.Random.Range(1,_teamName.Length - 2)];
                clubData.name_abr += teamName[_teamName.Length-1];
            }
            else
            {
                clubData.name_abr = _teamName.ToUpper();
            }
            return clubData.name_abr;
        }
        public List<int> GetBestTeamPlayersID(int maxPlayers)
        {
            List<int> arr = new List<int>();
            arr.Add(DBManager.Instance.DbUserData.data.players_goalkeepers[0].player_id);
            int id = 0;
            foreach(DBUserData.DBCharacterData cData in DBManager.Instance.DbUserData.data.players_characters)
            {
                if(id< maxPlayers-1) arr.Add(cData.player_id);
                id++;
            }
            print("Best Team has " + arr.Count + " players");
            return arr;
        }
        public List<DBUserData.DBCharacterData> GetSavedTeamFor(int totalPlayers)
        {
            Debug.Log("_____GetSavedTeamFor totalPlayers: " + totalPlayers);
            List<DBUserData.DBCharacterData> arr = new List<DBUserData.DBCharacterData>();
            DBGameData.DBFormationSave formationArr = DB.DBManager.Instance.DbUserData.data.gameData.GetFormation(totalPlayers);
            if (formationArr == null || formationArr.formation == null || formationArr.formation.Length == 0)
                return GetBestTeamDataPlayersID(totalPlayers);
               // return null;

            List<int> myNewTeam = new List<int>();
            foreach (DBGameData.DBFormationSave.DBFormationChar d in formationArr.formation)
            {
                DBUserData.DBCharacterData dbUserData = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(d.uniqueID);
               // print("juega: " + dbUserData.id);
                if (dbUserData == null)
                    Debug.LogError("No character id: " + d.uniqueID + " in your team");
                else
                {
                    myNewTeam.Add(dbUserData.player_id);
                    arr.Add(dbUserData);
                }
            }
            Data.Instance.matchData.SetTeam(2, myNewTeam);
            return arr;
        }
        public List<DBUserData.DBCharacterData> GetBestTeamDataPlayersID(int maxPlayers)
        {
            List<DBUserData.DBCharacterData> arr = new List<DBUserData.DBCharacterData>();
            arr.Add(DBManager.Instance.DbUserData.data.players_goalkeepers[0]);
            int id = 0;
            foreach (DBUserData.DBCharacterData cData in DBManager.Instance.DbUserData.data.players_characters)
            {
                if (id < maxPlayers-1) arr.Add(cData);
                id++;
            }
            return arr;
        }
        public List<int> GetCharacterIds(bool isGoalkeeper)
        {
            return DBManager.Instance.DbUserData.data.GetCharacterIds(isGoalkeeper);
        }
        public List<DBUserData.DBCharacterData> GetCharacters(bool isGoalkeeper)
        {
            return DBManager.Instance.DbUserData.data.GetCharacters(isGoalkeeper);
        }
        public void Save()
        {
            DBEvents.LoadUserData(null);
        }
        string SetList(List<int> arr)
        {
            int a = 0;
            string varName = "";
            foreach (int id in arr)
            {
                a++;
                varName += id.ToString();
                if (a < arr.Count)
                    varName += ",";
            }
            return varName;
        }
        public void CheckUpgradesFor(string text)
        {
            string[] arr = text.Split("_"[0]);
            string type = arr[0].Substring(0, 1);
            int characterID = int.Parse(arr[0].Substring(1, arr[0].Length - 1));
            CharactersData.CharacterData data;
            bool isgoalKeeper = false;
            if (type == "g") isgoalKeeper = true;
            data = CharactersData.Instance.GetCharacterData(characterID, isgoalKeeper);
            if (data == null) return;
            string[] upgrades = arr[1].Split("."[0]);
            int[] all = new int[5];
            int id = 0;
            foreach (string s in upgrades)
            {
                all[id] = int.Parse(s);
                id++;
            }
            data.SetUpgrades(all);
        }       

        public int GetTutorial() { return tutorialStep; }
        public void OnTutorialStepDone(int _tutorialStep)
        {
            if (_tutorialStep > tutorialStep)
            {
                tutorialStep = _tutorialStep;
            }
        }
        public int GetGamesPlayed() { return DBManager.Instance.DbUserData.data.gamesPlayed; }       
       
        public int GetCharacterType(int characterID)
        {
            return DBManager.Instance.DbUserData.data.GetCharacterType(characterID);
        }
        
        System.Action<LevelData> OnShortcutToLevelClicked;
        bool shortcutToLevelActive;
        
        public void ForceSecondLevelOpened()// for the onboarding
        {
            shortcutToLevelActive = true;
        }
        public bool CheckShortcutToSpecificLevel()
        {
            if (shortcutToLevelActive)
            {
                shortcutToLevelActive = false;
                return true;
            }
            return false;
        }
        public string GetCharacterLevelAsString(DBUserData.DBCharacterData uData)
        {
            int level = uData.level;
            return Data.Instance.texts.Get("level") + " " + level;
        }

        public string GetCharacterTierXp(DBUserData.DBCharacterData uData) {
            return ""+uData.current_level_xp;
        }

        public string GetCharacterLevelProgress(DBUserData.DBCharacterData uData)
        {
            return uData.current_level_xp + "/" + (uData.current_level_xp + uData.xp_to_next_level);
        }
        public float GetCharacterLevelProgressValue(DBUserData.DBCharacterData uData)
        {
            return (float)uData.current_level_xp / (float)((float)uData.current_level_xp + uData.xp_to_next_level);
        }
    }

}