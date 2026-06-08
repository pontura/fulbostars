using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;
using Fulbo.DB;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Fulbo.UI;
using Fulbo.AssetsBundle;

namespace Fulbo
{
    public class CupLevelsData : DataLoader
    {
        [SerializeField]int totalTeamsForMultiplayer;
        public List<LevelData> content;
        public List<LevelData> GetByState(string state = "on") {
            List<LevelData> all = new List<LevelData>();
            foreach(LevelData ld in content)
            {
                if (ld.state == state)
                    all.Add(ld);
            }
            return all;
        }

        public List<MultiCharactersData> multi_characters;
        //  public List<LevelData> contentMultiplayer;
        public TextAsset file_for_multiplayer;
        public TextAsset file_for_multi_characters;

        [Serializable]
        public class MultiCharactersData
        {
            public string name;
            public string rol;
            public int id;
            public string tag;
        }


        Action OnDone;

        /// ///////// Mulitplayer

        int team1;
        int team2;
        public override void LoadData(System.Action OnReady)
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                OnLoadedMultiplayer(yutokun.CSVParser.LoadFromString(file_for_multiplayer.text), OnReady);
            }
            else
                base.LoadData(OnReady);
        }
        public void OnLoadedMultiplayer(List<List<string>> d, System.Action OnReady)
        {
            if (OnReady != null)
            {
                OnReady();
                OnReady = null;
            }
        }
        public void SetOponents(LevelData levelData, int totalCharacters = 8, bool shuffle = true)
        {
            levelData.oponents = GetCharactersForTag(levelData.team_tag, totalCharacters, shuffle);
        }
        public List<int> GetCharactersForTag(string tag, int totalCharacters = 8, bool shuffle = true)
        {
            List<int>arr = new List<int>();
            int a = 0;
            foreach(MultiCharactersData characterData in multi_characters)
            {
                if(characterData.tag == tag && a <= totalCharacters && characterData.rol != "GK")
                {
                    arr.Add(characterData.id);
                    a++;
                }
            }
            if(shuffle)
                Utils.Shuffle(arr);
                
            arr.Insert(0, GetGoalkeeperForTag(tag));
            if(arr.Count>totalCharacters)
                arr.RemoveRange(totalCharacters,arr.Count - totalCharacters);
            return arr;
        }
       
        public int GetGoalkeeperForTag(string tag)
        {
            foreach (MultiCharactersData characterData in multi_characters)
            {
                if (characterData.rol == "GK" && characterData.tag == tag)
                    return characterData.id;
            }
            return 1; // gk default;
        }
        public void InitMultiplayer()
        {
            totalTeamsForMultiplayer = 0;
            foreach (LevelData ld in content)
            {
                if (ld.state == "on")
                    totalTeamsForMultiplayer++;
            }
            // default memes
            // default ninios o tercermundo

            team2 = 1;
            team1 = 2; // ninios
            int rand = Random.Range(0, 10);
            if (rand < 3)
            {
                team1 = 4; // presidentes
            }else if (rand < 5)
            {
                team1 = 9; // tercermundo
            }
            else if (rand <7)
            {
                team1 = 4; // tercermundo
            }

            SetGetMultiplayerTeam(1);
            SetGetMultiplayerTeam(2);

            Data.Instance.partyModeData.SetTeamID(1, team1);
            Data.Instance.partyModeData.SetTeamID(2, team2);

            SetClubData(1);
            SetClubData(2);
        }
        public void InitTournament()
        {
            
            team1 = 0;
            team2 = 0;

            int id = 0;
            List<LevelData> all = new List<LevelData>();
            foreach (LevelData ld in content)
            {
                if (ld.state == "torneo")
                {
                    if (team1 == 0)
                        team1 = id;
                    else team2 = id;
                }
                id++;
            }
            print("InitTournament team1" + team1 + content[team1].name);
            print("InitTournament team2" + team2 + content[team2].name);

            List<int> team1Characters = GetCharactersForTag(content[team1].team_tag, 20, false);
            List<int> team2Characters = GetCharactersForTag(content[team2].team_tag, 20, false);

            Data.Instance.matchData.team1 = team1Characters;
            Data.Instance.matchData.team2 = team2Characters;

            
            print("InitTournament team1 " + Data.Instance.matchData.team1.Count);
            print("InitTournament team2 " + Data.Instance.matchData.team2.Count);

            Data.Instance.partyModeData.SetTeamID(1, team1);
            Data.Instance.partyModeData.SetTeamID(2, team2);

            SetClubData(1);
            SetClubData(2);
        }
        public void ChangeMultiplayerTeam(int teamID, bool add)
        {
            if (teamID == 1)
            {
                if (add) team1++; else team1--;
                if (team1 < 0) team1 = totalTeamsForMultiplayer - 1; else if (team1 > totalTeamsForMultiplayer - 1) team1 = 0;
                Data.Instance.partyModeData.SetTeamID(teamID, team1);
            } 
            else
            {
                if (add) team2++; else team2--;
                if (team2 < 0) team2 = totalTeamsForMultiplayer- 1; else if (team2 > totalTeamsForMultiplayer - 1) team2 = 0;
                Data.Instance.partyModeData.SetTeamID(teamID, team2);
            }
            SetGetMultiplayerTeam(teamID);
            SetClubDataMultiplayer(teamID);
        }
        public void SetTournament(int teamID, int teamIDInArray)
        {
            if (teamID == 1)
            {
                if (team1 < 0) team1 = content.Count - 1; else if (team1 > content.Count - 1) team1 = 0;
                Data.Instance.partyModeData.SetTeamID(teamID, team1);
            }
            else
            {
                if (team2 < 0) team2 = content.Count - 1; else if (team2 > content.Count - 1) team2 = 0;
                Data.Instance.partyModeData.SetTeamID(teamID, team2);
            }
            SetGetMultiplayerTeam(teamID);
            SetClubDataMultiplayer(teamID);
        }
        void SetGetMultiplayerTeam(int teamID)
        {
            List<int> team1Characters = CupsData.Instance.levels.GetCharactersForTag(content[team1].team_tag);
            List<int> team2Characters = CupsData.Instance.levels.GetCharactersForTag(content[team2].team_tag);
            Data.Instance.matchData.team1 = team1Characters;
            Data.Instance.matchData.team2 = team2Characters;
            //Data.Instance.matchData.team1 = content[team1].oponents;
            //Data.Instance.matchData.team2 = content[team2].oponents;
            Data.Instance.matchData.ShufflePlayersInTeam(teamID);
        }
        void SetClubData(int teamID)
        {
            int t = team1;
            if (teamID == 2)
                t = team2;
            Data.Instance.partyModeData.SetClubData(content[t].clubData, teamID);
        }
        void SetClubDataMultiplayer(int teamID)
        {
            int t = team1;
            if (teamID == 2)
                t = team2;
            Data.Instance.partyModeData.SetClubData(content[t].clubData, teamID);
        }

        /// </Mulitplayer


        void Start()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            { 
                OnLoaded(yutokun.CSVParser.LoadFromString(file_in_server.text));
                OnLoadedCharacters(yutokun.CSVParser.LoadFromString(file_for_multi_characters.text));
            }
            else
                LoadData(null);
        }
        public void Init()
        {
            List<DB.DBMatches.MatchData> dbMatches = DB.DBManager.Instance.DbMatches.data.results;
            foreach (DB.DBMatches.MatchData dbMatch in dbMatches)
            {
                int cupID = dbMatch.cup;
                int id = dbMatch.id;
                LevelData lData = GetLevelData(cupID, id);
                if (lData != null)
                    lData.HasBeenPlayed();
            }
        }
        public List<LevelData> GetAllLevelsFromCup(int cupID, int tier)
        {
            List<LevelData> arr = new List<LevelData>();
            foreach (LevelData ld in content)
                if (ld.cupID == cupID && ld.tier == tier)
                    arr.Add(ld);
            return arr;
        }
        public LevelData GetLevelData(int cupID, int id)
        {
            foreach (LevelData ld in content)
                if (ld.cupID == cupID && ld.id == id) return ld;
            return null;
        }
        public override void OnLoaded(List<List<string>> d)
        {
            this.content = new List<LevelData>();
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                OnDataLoadedMultiplayer(content, d);
            }
            else
            {
                OnDataLoaded(content, d);
            }
            if (OnDone != null)
                OnDone();
        }
        public void OnLoadedCharacters(List<List<string>> d)
        {
            this.multi_characters = new List<MultiCharactersData>();
            OnDataLoadedMultiCharacters(multi_characters, d);
            if (OnDone != null)
                OnDone();
        }
        void OnDataLoaded(List<LevelData> content, List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            int _levelID = 1;
            int _stadium_id = -1;
            LevelData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                    //print("row: " + rowID + "  colID: " + colID + "  value: " + value);
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new LevelData();
                                contentLine.clubData = new ClubData();
                                contentLine.cupID = int.Parse(value);

                                if (_stadium_id != contentLine.stadium_id)
                                {
                                    _stadium_id = contentLine.stadium_id;
                                    _levelID = 1;
                                }
                                else
                                    _levelID++;

                                contentLine.totalStatsGK = new CharacterStats();
                                contentLine.totalStatsDef = new CharacterStats();
                                contentLine.totalStatsMid = new CharacterStats();
                                contentLine.totalStatsFor = new CharacterStats();

                                content.Add(contentLine);
                                contentLine.id = _levelID;
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                                contentLine.id = int.Parse(value);
                            else if (colID == 2 && value != "")
                                contentLine.tier = int.Parse(value);
                            //else if (colID == 3 && value != "")
                            //    contentLine.score_win = int.Parse(value);
                            else if (colID == 4 && value != "")
                                contentLine.stadium_id = int.Parse(value);
                            else if (colID == 5 && value != "")
                                contentLine.size = value;
                            else if (colID == 6 && value != "")
                                contentLine.myTeamQty = int.Parse(value);
                            else if (colID == 7 && value != "")
                                contentLine.name = value;
                            else if (colID == 8 && value != "") {
                                contentLine.oponents = new List<int>();
                                string v = value.Replace(" ", "");
                                string[] arr = v.Split(","[0]);
                                foreach (string s in arr) {
                                    int id = int.Parse(s);
                                    contentLine.oponents.Add(id);
                                }
                            } else if (colID == 9 && value != "")
                                contentLine.oponents.Insert(0, int.Parse(value));
                            else if (colID == 10 && value != "")
                                contentLine.referiID = int.Parse(value);
                            else if (colID == 11 && value != "")
                                contentLine.duration = int.Parse(value);
                            else if (colID == 12 && value != "")
                                contentLine.charactersPositions = int.Parse(value);
                            //else if (colID == 13 && value != "")
                            //    contentLine.coinPrize = int.Parse(value);
                            else if (colID == 14 && value != "")
                                contentLine.clubData.name_abr = value;
                            else if (colID == 15 && value != "")
                                contentLine.clubData.clubColor1 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 16 && value != "")
                                contentLine.clubData.clubColor2 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 17 && value != "")
                                contentLine.clubData.clubColor3 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 18 && value != "")
                                contentLine.clubData.clubColor4 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 19 && value != "")
                                contentLine.pinballID = int.Parse(value);
                            else if (colID == 20 && value != "")
                                contentLine.clubData.designID = int.Parse(value);
                            else if (colID == 21 && value != "")
                                contentLine.clubData.shieldDesignID = int.Parse(value);
                            else if (colID == 22 && value != "") {
                                contentLine.logoID = int.Parse(value);
                                contentLine.clubData.logo = contentLine.logoID;
                            } else if (colID == 23 && value != "")
                                contentLine.totalStatsGK.accuracy = int.Parse(value);
                            else if (colID == 24 && value != "")
                                contentLine.totalStatsGK.stamina = int.Parse(value);
                            else if (colID == 25 && value != "")
                                contentLine.totalStatsGK.speed = int.Parse(value);
                            else if (colID == 26 && value != "")
                                contentLine.totalStatsGK.dexterity = int.Parse(value);
                            else if (colID == 27 && value != "")
                                contentLine.totalStatsGK.awareness = int.Parse(value);

                            else if (colID == 28 && value != "")
                                contentLine.totalStatsDef.accuracy = int.Parse(value);
                            else if (colID == 29 && value != "")
                                contentLine.totalStatsDef.stamina = int.Parse(value);
                            else if (colID == 30 && value != "")
                                contentLine.totalStatsDef.speed = int.Parse(value);
                            else if (colID == 31 && value != "")
                                contentLine.totalStatsDef.dexterity = int.Parse(value);
                            else if (colID == 32 && value != "")
                                contentLine.totalStatsDef.awareness = int.Parse(value);

                            else if (colID == 33 && value != "")
                                contentLine.totalStatsMid.accuracy = int.Parse(value);
                            else if (colID == 34 && value != "")
                                contentLine.totalStatsMid.stamina = int.Parse(value);
                            else if (colID == 35 && value != "")
                                contentLine.totalStatsMid.speed = int.Parse(value);
                            else if (colID == 36 && value != "")
                                contentLine.totalStatsMid.dexterity = int.Parse(value);
                            else if (colID == 37 && value != "")
                                contentLine.totalStatsMid.awareness = int.Parse(value);

                            else if (colID == 38 && value != "")
                                contentLine.totalStatsFor.accuracy = int.Parse(value);
                            else if (colID == 39 && value != "")
                                contentLine.totalStatsFor.stamina = int.Parse(value);
                            else if (colID == 40 && value != "")
                                contentLine.totalStatsFor.speed = int.Parse(value);
                            else if (colID == 41 && value != "")
                                contentLine.totalStatsFor.dexterity = int.Parse(value);
                            else if (colID == 42 && value != "")
                                contentLine.totalStatsFor.awareness = int.Parse(value);
                            else if (colID == 43 && value != "")
                                contentLine.duelStatsPlayer = int.Parse(value);
                            else if (colID == 44 && value != "")
                                contentLine.duelStatsGK = int.Parse(value);
                            else if (colID == 45 && value != "")
                                contentLine.idleDelay = float.Parse(value) / 1000f;

                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
            GetComponent<CupsData>().LevelsLodaded();
        }




        void OnDataLoadedMultiplayer(List<LevelData> content, List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            int _levelID = 1;
            int _stadium_id = -1;
            LevelData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                 //   print("multiplayer row: " + rowID + "  colID: " + colID + "  value: " + value);
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new LevelData();
                                contentLine.clubData = new ClubData();

                                if (_stadium_id != contentLine.stadium_id)
                                {
                                    _stadium_id = contentLine.stadium_id;
                                    _levelID = 1;
                                }
                                else
                                    _levelID++;

                                contentLine.totalStatsGK = new CharacterStats();
                                contentLine.totalStatsDef = new CharacterStats();
                                contentLine.totalStatsMid = new CharacterStats();
                                contentLine.totalStatsFor = new CharacterStats();

                                content.Add(contentLine);
                                contentLine.name = value;
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                                contentLine.team_tag = value;
                            else if (colID == 2 && value != "")
                                contentLine.clubData.name_abr = value;
                            else if (colID == 3 && value != "")
                                contentLine.clubData.clubColor1 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 4 && value != "")
                                contentLine.clubData.clubColor2 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 5 && value != "")
                                contentLine.clubData.clubColor3 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 6 && value != "")
                                contentLine.clubData.clubColor4 = Data.Instance.settings.GetColorIndexFor(value);
                            else if (colID == 7 && value != "")
                                contentLine.pinballID = int.Parse(value);
                            else if (colID == 8 && value != "")
                                contentLine.clubData.designID = int.Parse(value);
                            else if (colID == 9 && value != "")
                                contentLine.clubData.shieldDesignID = int.Parse(value);
                            else if (colID == 10 && value != "")
                            {
                                contentLine.logoID = int.Parse(value);
                                contentLine.clubData.logo = contentLine.logoID;
                            }
                            else if (colID == 11 && value != "")
                            {
                                contentLine.state = value;
                            }
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
            GetComponent<CupsData>().LevelsLodaded();
        }
        void OnDataLoadedMultiCharacters(List<MultiCharactersData> content, List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            MultiCharactersData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                   // print("multiplayer Characters row: " + rowID + "  colID: " + colID + "  value: " + value);
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new MultiCharactersData();
                                content.Add(contentLine);
                                contentLine.name = value;
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                                contentLine.rol = value;
                            else if (colID == 2 && value != "")
                                contentLine.id= int.Parse(value);
                            else if (colID == 3 && value != "")
                                contentLine.tag = value;
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
        }


        LevelData.conditionType GetCondition(string s)
        {
            switch (s.ToLower())
            {
                case "unlocked": return LevelData.conditionType.UNLOCKED;
                case "win": return LevelData.conditionType.WIN;
                case "complete": return LevelData.conditionType.COMPLETE;
                default: return LevelData.conditionType.GOALS;
            }
        }

        public void ResetApp() // Lockear todos los niveles al logout para q no se desblockeen en cuenta nueva
        {
            //unlockedLevels.Clear();
            //for (int a = 0; a < all.Count; a++)
            //{
            //    foreach (LevelData lData in GetAllLevelsFromStadium(a))
            //    {
            //        lData.locked = true;
            //    }
            //}
        }

        public void UnlockAll()
        {
            //unlockedLevels.Clear();
            //for (int a = 0; a < all.Count; a++)
            //{
            //    foreach (LevelData lData in GetAllLevelsFromStadium(a))
            //    {
            //        lData.locked = false;
            //        unlockedLevels.Add(lData);
            //    }
            //}
        }
    }
}