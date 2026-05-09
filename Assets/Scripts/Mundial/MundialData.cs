using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;
using Fulbo.DB;
using UnityEngine.SceneManagement;

namespace Fulbo.Mundial
{
    public class MundialData : DataLoader
    {
        public DBMundial.DataFromServer rankings;
        public int stadiumID = 0;
        public int levelID = 1;

        public static MundialData mInstance;
        public static MundialData Instance
        {
            get { return mInstance; }
        }
        public bool openShortCut;
        [Serializable]
        public class LevelData
        {
            public int id;
            public int stadium_id;
            public string size;
            public string name;
            public int duration;
            public int charactersPositions;
            public int coinPrize;
            public int myTeamQty;
            public List<int> oponents;
            public ClubData clubData;
            public int referiID;
            public int pinballID;
            public int logoID;
            public int score_win;
            public string conditionText;

            public CharacterStats totalStatsGK;
            public CharacterStats totalStatsDef;
            public CharacterStats totalStatsMid;
            public CharacterStats totalStatsFor;


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
        }

        public List<LevelData> content;

        void Awake()
        {
            if (mInstance != null)
                Destroy(gameObject);
            else
            {
                mInstance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        Action OnDone;

        public void OnLoad(Action OnDone)
        {
            this.OnDone = OnDone;
            LoadData(null);
        }
        System.Action OnRankingLoaded;
        public void LoadRanking(System.Action OnRankingLoaded)
        {
            this.OnRankingLoaded = OnRankingLoaded;
            DB.DBManager.Instance.DbMundial.Load(OnRankingLoad);
            print("LoadRanking()");
        }
        void OnRankingLoad(DBMundial.DataFromServer data)
        {
            this.rankings = data;
            OnRankingLoaded();
        }

        //local
        System.Action<DBMundial.DataFromServer> OnRankingLoadedLocal;
        public void LoadRankingLocal(string country, System.Action<DBMundial.DataFromServer> OnRankingLoadedLocal)
        {
            this.OnRankingLoadedLocal = OnRankingLoadedLocal;
            DB.DBManager.Instance.DbMundial.LoadLocal(country, OnLocalRankingLoad);
            
        }
        void OnLocalRankingLoad(DBMundial.DataFromServer data)
        {
            OnRankingLoadedLocal(data);
        }


        public void SetActualStadium(int _stadiumID)
        {
            this.stadiumID = _stadiumID;
        }
        public void SetActualLevel(int _levelID)
        {
            this.levelID = _levelID;
            CheckToSwapColors();
        }
        public LevelData GetCountryData(string name_abr) // setea stadium + level:
        {
            foreach (LevelData ld in content)
            {
                if (ld.clubData.name_abr == name_abr)
                    return ld;
            }
            return null;
        }
        void CheckToSwapColors()
        {
            LevelData levelData = GetLevelActual();
            levelData.clubData.clubColor1 = Data.Instance.settings.GetSecondaryColorIfAreSimilar(Data.Instance.myTeam.clubData.clubColor1, levelData.clubData.clubColor1);

        }        
        public LevelData GetLevelActual()
        {
            return GetLevelData(stadiumID, levelID);
        }
        public List<LevelData> GetAllLevelsFromStadium(int stadiumID)
        {
            List<LevelData> arr = new List<LevelData>();
            foreach (LevelData ld in content)
                if (ld.stadium_id == stadiumID)
                    arr.Add(ld);
            return arr;
        }
        public LevelData GetLevelData(int stadiumID, int levelID)
        {
            int id = 1;
            foreach (LevelData ld in content)
            {
                if (ld.stadium_id == stadiumID)
                {
                    if (levelID == id)
                        return ld;
                    id++;
                }
            }
            return null;
        }

        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(content, d);
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
                                contentLine.stadium_id = int.Parse(value);

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
                            {
                                contentLine.size = value;
                            }
                            if (colID == 2 && value != "")
                            {
                                contentLine.myTeamQty = int.Parse(value);
                            }

                            else if (colID == 3 && value != "")
                            {
                                contentLine.name = value;
                            }
                            else if (colID == 4 && value != "")
                            {
                                contentLine.oponents = new List<int>();
                                string[] arr = value.Split(","[0]);
                                foreach (string s in arr)
                                {
                                    int id = int.Parse(s);
                                    contentLine.oponents.Add(id);
                                }
                            }
                            else if (colID == 5 && value != "")
                            {
                                contentLine.oponents.Insert(0, int.Parse(value));
                            }
                            else if (colID == 6 && value != "")
                            {
                                contentLine.referiID = int.Parse(value);
                            }
                            else if (colID == 7 && value != "")
                            {
                                contentLine.duration = int.Parse(value);
                            }
                            else if (colID == 8 && value != "")
                            {
                                contentLine.charactersPositions = int.Parse(value);
                            }
                            //else if (colID == 9 && value != "")
                            //{
                            //    contentLine.coinPrize = int.Parse(value);
                            //}
                            else if (colID == 10 && value != "")
                            {
                                contentLine.clubData.name_abr = value;
                            }
                            else if (colID == 11 && value != "")
                            {
                                contentLine.clubData.clubColor1 = Data.Instance.settings.GetColorIndexFor(value);
                            }
                            else if (colID == 12 && value != "")
                            {
                                contentLine.clubData.clubColor2 = Data.Instance.settings.GetColorIndexFor(value);
                            }
                            else if (colID == 13 && value != "")
                            {
                                contentLine.clubData.clubColor3 = Data.Instance.settings.GetColorIndexFor(value);
                            }
                            else if (colID == 14 && value != "")
                            {
                                contentLine.clubData.clubColor4 = Data.Instance.settings.GetColorIndexFor(value);
                            }
                            else if (colID == 15 && value != "")
                            {
                                contentLine.pinballID = int.Parse(value);
                            }
                            else if (colID == 16 && value != "")
                            {
                                contentLine.clubData.designID = int.Parse(value);
                            }
                            else if (colID == 17 && value != "")
                            {
                                contentLine.clubData.shieldDesignID = int.Parse(value);
                            }
                            else if (colID == 18 && value != "")
                            {
                                contentLine.clubData.logo = int.Parse(value);
                            }
                            else if (colID == 19 && value != "")
                                contentLine.totalStatsGK.accuracy = int.Parse(value);
                            else if (colID == 20 && value != "")
                                contentLine.totalStatsGK.stamina = int.Parse(value);
                            else if (colID == 21 && value != "")
                                contentLine.totalStatsGK.speed = int.Parse(value);
                            else if (colID == 22 && value != "")
                                contentLine.totalStatsGK.dexterity = int.Parse(value);
                            else if (colID == 23 && value != "")
                                contentLine.totalStatsGK.awareness = int.Parse(value);

                            else if (colID == 24 && value != "")
                                contentLine.totalStatsDef.accuracy = int.Parse(value);
                            else if (colID == 25 && value != "")
                                contentLine.totalStatsDef.stamina = int.Parse(value);
                            else if (colID == 26 && value != "")
                                contentLine.totalStatsDef.speed = int.Parse(value);
                            else if (colID == 27 && value != "")
                                contentLine.totalStatsDef.dexterity = int.Parse(value);
                            else if (colID == 28 && value != "")
                                contentLine.totalStatsDef.awareness = int.Parse(value);

                            else if (colID == 29 && value != "")
                                contentLine.totalStatsMid.accuracy = int.Parse(value);
                            else if (colID == 30 && value != "")
                                contentLine.totalStatsMid.stamina = int.Parse(value);
                            else if (colID == 31 && value != "")
                                contentLine.totalStatsMid.speed = int.Parse(value);
                            else if (colID == 32 && value != "")
                                contentLine.totalStatsMid.dexterity = int.Parse(value);
                            else if (colID == 33 && value != "")
                                contentLine.totalStatsMid.awareness = int.Parse(value);

                            else if (colID == 34 && value != "")
                                contentLine.totalStatsFor.accuracy = int.Parse(value);
                            else if (colID == 35 && value != "")
                                contentLine.totalStatsFor.stamina = int.Parse(value);
                            else if (colID == 36 && value != "")
                                contentLine.totalStatsFor.speed = int.Parse(value);
                            else if (colID == 37 && value != "")
                                contentLine.totalStatsFor.dexterity = int.Parse(value);
                            else if (colID == 38 && value != "")
                                contentLine.totalStatsFor.awareness = int.Parse(value);
                            else if (colID == 39 && value != "")
                                contentLine.score_win = int.Parse(value);
                            else if (colID == 40 && value != "")
                                contentLine.conditionText = value;
                            else if (colID == 41 && value != "")
                            {
                              
                            }
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
        }
       

        public void ResetApp() // Lockear todos los niveles al logout para q no se desblockeen en cuenta nueva
        {
        }


        public void Start()
        {
            Events.ResetApp += ResetApp;
        }

        public void OnDestroy()
        {
            Events.ResetApp -= ResetApp;
        }
    }
}