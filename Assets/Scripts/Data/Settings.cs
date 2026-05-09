using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Game;
using Fulbo.AssetsBundle;

namespace Fulbo
{
    public class Settings : DataLoader
    {
        public float ___Y = 1000;
        public float ___Y2 = 520;
        public bool loaded;
        [SerializeField] public TextAsset url_settings_file;
        [SerializeField] string url_settings = "https://docs.google.com/spreadsheets/d/e/2PACX-1vR_IX7b2VUn8q2_yvC3lD2JpNxZZiObI31lFeyX2OV-eqc3OYlrQ9U_ffo1kiKeHgvtpVi0v5hvQ54s/pub?gid=65279539&single=true&output=tsv";
        [SerializeField] public TextAsset url_gameplay_file;
        [SerializeField] public string url_gameplay;

        public int totalPlayersAvailable = 4;

        public List<SettingsData> content;
        Dictionary<string, float> settings;

        public bool SettingExist(string key) { return settings.ContainsKey(key); }

        public float GetSetting(string key)
        {
            return settings[key];
        }
        public TextAsset settingsDataFile;
        public ColorStyles[] colorStyles;

        public MainSettings mainSettings;
        [Serializable]
        public class MainSettings
        {
            public bool music_on = true;
            public bool announcer_on = true;
            public bool speech_bubbles_on = true;
            public bool isArcade; // multiplayer
            public bool rewired;
            public bool debug;
            public bool turn_off_team2;
            public bool turn_on_only_other_goalkeeper;
            public bool goals_blocked;
            public int stadium_id;
        }
        [Serializable]
        public class SettingsData
        {
            public string id;
            public float value;
        }
        [Serializable]
        public class ColorStyles
        {
            public string stringID;
            public Color color;
            public List<string> oppositeIds;
        }
        [Serializable]
        public class Stats
        {
            public stat stat;
            public Sprite icon;
            public Color color;
            public Sprite wearable;
        }
        public enum stat { accuracy, stamina, speed, dexterity, trickery }

        public List<Stats> statsSettings;



        public List<RaritySetting> raritySetting;

        [Serializable]
        public class RaritySetting
        {
            public FigusData.rarities rarity;
            public Color color;
            public Sprite icon;
        }
        public RaritySetting GetRaritySettingFor(int rarityID)
        {
            return raritySetting[rarityID];
        }
        public RaritySetting GetRaritySettingFor(FigusData.rarities rarity)
        {
            foreach (RaritySetting s in raritySetting)
                if (rarity == s.rarity)
                    return s;
            return null;
        }




        public Vector2 selectedTeams;
        public GamePlay gameplay;

        void Awake()
        {
            settings = new Dictionary<string, float>();
        }
        Action OnReady;
        public override void LoadData(Action OnReady)
        {
            this.OnReady = OnReady;
            mainSettings = JsonUtility.FromJson<MainSettings>(settingsDataFile.text);

            if (Data.Instance.loadType == Data.loadTypes.DATABASE) 
                Data.Instance.sheetLoader.LoadFromTo(url_settings, OnLoaded);
            else if (Data.Instance.loadType == Data.loadTypes.LOCAL)
                OnLoaded(yutokun.CSVParser.LoadFromString(url_settings_file.text));
            else {
                AssetsBundleLoader abl = AssetsBundleManager.Instance.assetsBundleLoader;
                OnLoaded(yutokun.CSVParser.LoadFromString(abl.GetSheetText(file_in_server.name + ".txt")));
                //Data.Instance.sheetLoader.LoadFromTo(Data.Instance.GetURL() + "AssetBundles/" + file_in_server.name + ".txt" + "?rand=" + UnityEngine.Random.Range(1000, 10000), OnLoaded);
            }

        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnSettingsContentLoaded(content, d);
            LoadGamePlaySettings();
        }
        void OnSettingsContentLoaded(List<SettingsData> content, List<List<string>> d)
        {
            print("OnSettingsContentLoaded " + d.Count);
            content.Clear();
            int colID = 0;
            int rowID = 0;
            SettingsData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                contentLine = new SettingsData();
                                contentLine.id = value;
                                content.Add(contentLine);
                            }
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                            {
                                settings.Add(contentLine.id, float.Parse(value) / 100);
                                contentLine.value = float.Parse(value) / 100;
                            }
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
        }



        void LoadGamePlaySettings()
        {
            Events.OnLoading("GamePlay settings");
            if (Data.Instance.loadType == Data.loadTypes.DATABASE)
                Data.Instance.sheetLoader.LoadFromTo(url_gameplay, OnDataLoaded);
            else if (Data.Instance.loadType == Data.loadTypes.LOCAL)
                OnDataLoaded(yutokun.CSVParser.LoadFromString(url_gameplay_file.text));
            else {
                AssetsBundleLoader abl = AssetsBundleManager.Instance.assetsBundleLoader;
                OnDataLoaded(yutokun.CSVParser.LoadFromString(abl.GetSheetText(url_gameplay_file.name + ".txt")));
                //Data.Instance.sheetLoader.LoadFromTo(Data.Instance.GetURL() + "AssetBundles/" + url_gameplay_file.name + ".txt" + "?rand=" + UnityEngine.Random.Range(1000, 10000), OnDataLoaded);
            }
        }
        public List<GamePlay> stats_by_character;
        public GamePlay GetStats(Character.types characterType, int teamID)
        {
            if (!Data.Instance.isMobile)
                return gameplay;
            else
                foreach (GamePlay gp in stats_by_character)
                    if (gp.characterType == characterType)
                        return gp;
            return null;
        }
        void OnDataLoaded(List<List<string>> all)
        {
            int colID = 0;
            int rowID = 0;
            GamePlay gameplayStats;
            foreach (List<string> line in all)
            {
                foreach (string data in line)
                {

                    if (rowID == 0) // init all:
                    {
                        //  print(data);
                        // print(rowID + " _____________ " + colID);
                        if (colID > 1)
                        {
                            gameplayStats = new GamePlay();
                            switch (data)
                            {
                                case "DEFENSOR": gameplayStats.characterType = Character.types.DEF; break;
                                case "CENTRAL": gameplayStats.characterType = Character.types.MID; break;
                                case "DELANTERO": gameplayStats.characterType = Character.types.FOR; break;
                                default: gameplayStats.characterType = Character.types.GOALKEEPER; break;
                            }
                            stats_by_character.Add(gameplayStats);
                        }
                    }
                    else if (colID == 0)
                    {
                        for (int a = 0; a < stats_by_character.Count + 1; a++)
                        {
                            if (a > 0) // va para cada jugador
                                gameplayStats = stats_by_character[a - 1];
                            else // va para los settings generales
                                gameplayStats = gameplay;

                            float value = 0;
                            if (line.Count > a + 1)
                            {
                                float.TryParse(line[a + 1], out value);
                                // value = float.Parse(line.data[a + 1]);

                                if (a > 0) // le suma el default:
                                {
                                    float defaultValue;
                                    float.TryParse(line[1], out defaultValue);
                                    if (defaultValue > 0)
                                        value += defaultValue;
                                }

                                if (value > 0)
                                    value /= 10;
                            }

                            AddValue(gameplayStats, data, value);
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
            OnDone();
        }
        void OnDone()
        {
            loaded = true;
            OnReady();
        }
        void AddValue(GamePlay gameplayStats, string field, float value)
        {
            switch (field)
            {
                case "speed": gameplayStats.speed = value; break;
                case "speedRun": gameplayStats.speedRun = value; break;
                case "speedRunWithBall": gameplayStats.speedRunWithBall = value; break;
                case "freeze_by_kick": gameplayStats.freeze_by_kick = value; break;
                case "freeze_by_loseBall": gameplayStats.freeze_by_loseBall = value; break;
                case "freeze_by_dashBall": gameplayStats.freeze_by_dashBall = value; break;
                case "freeze_by_hit": gameplayStats.freeze_by_hit = value; break;
                case "freeze_dash": gameplayStats.freeze_dash = value; break;
                case "distance_to_dash_ai": gameplayStats.distance_to_dash_ai = value; break;
                case "dash_percent": gameplayStats.dash_percent = value; break;
                case "random_jump_a_dash": gameplayStats.random_jump_a_dash = value; break;
                case "height_to_dominate_ball": gameplayStats.height_to_dominate_ball = value; break;
                case "speedWithBall": gameplayStats.speedWithBall = value; break;
                case "speedDash": gameplayStats.speedDash = value; break;
                case "speedRunFade": gameplayStats.speedRunFade = value; break;
                case "defenseDelay": gameplayStats.defenseDelay = value; break;
                case "attackDelay": gameplayStats.attackDelay = value; break;
                case "kickHard": gameplayStats.kickHard = value; break;
                case "kickHardAngle": gameplayStats.kickHardAngle = value; break;
                case "kickSoft": gameplayStats.kickSoft = value; break;
                case "kickSoftAngle": gameplayStats.kickSoftAngle = value; break;
                case "kickBaloon": gameplayStats.kickBaloon = value; break;
                case "kickBaloonAngle": gameplayStats.kickBaloonAngle = value; break;
                case "kickHead": gameplayStats.kickHead = value; break;
                case "kickHeadAngle": gameplayStats.kickHeadAngle = value; break;
                case "kickChilena": gameplayStats.kickChilena = value; break;
                case "kickChilenaAngle": gameplayStats.kickChilenaAngle = value; break;
                case "kickCentro": gameplayStats.kickCentro = value; break;
                case "kickCentroAngle": gameplayStats.kickCentroAngle = value; break;
                case "duration_dash": gameplayStats.duration_dash = value; break;
                case "lujito_multiplier": gameplayStats.lujito_multiplier = value; break;
                case "idleDelay": gameplayStats.idleDelay = value; break;
                case "collider_radius": gameplayStats.collider_radius = value; break;
                case "collider_radius_air": gameplayStats.collider_radius_air = value; break;
                case "collider_height": gameplayStats.collider_height = value; break;
                case "collider_radius_dash_multiplier": gameplayStats.collider_radius_dash_multiplier = value; break;
                case "cooldown_lujito": gameplayStats.cooldown_lujito = value; break;
                case "cooldown_dash": gameplayStats.cooldown_dash = value; break;
            }
        }
        public void SetRewiredControlls(bool isOn)
        {
            //#if UNITY_STANDALONE
            //        Rewired.UI.ControlMapper.ControlMapper controlMapper = Data.Instance.rewiredInputManager.GetComponent<Rewired.UI.ControlMapper.ControlMapper>();

            //        if (isOn)
            //        {
            //            foreach (Rewired.Player player in Rewired.ReInput.players.AllPlayers)
            //            {
            //                player.controllers.maps.SetMapsEnabled(true, "Default");
            //            }
            //        }
            //        else
            //        {
            //            foreach (Rewired.Player player in Rewired.ReInput.players.AllPlayers)
            //            {
            //                player.controllers.maps.SetMapsEnabled(false, "Default");
            //            }
            //        }
            //#endif
        }
        public Color GetColorFor(string stringID)
        {
            foreach (ColorStyles cs in colorStyles)
                if (stringID == cs.stringID)
                    return cs.color;
            Debug.LogError("No hay color para " + stringID);
            return colorStyles[0].color;
        }
        public int GetColorIndexFor(string stringID)
        {
            int index = 0;
            foreach (ColorStyles cs in colorStyles)
            {
                if (stringID == cs.stringID)
                    return index;
                index++;
            }
            return 0;
        }
        public int GetSecondaryColorIfAreSimilar(int color1, int color2)
        {
            Settings settings = Data.Instance.settings;
            if (color1 == color2)
            {
                if (color1 != settings.GetColorIndexFor("negro"))
                    color2 = settings.GetColorIndexFor("negro");
                else
                    color2 = settings.GetColorIndexFor("blanco");
            }
            else if (SimialColors(color1, color2, new string[] { "amarillo", "naranja", "mandarina" })) color2 = settings.GetColorIndexFor("azul");
            else if (SimialColors(color1, color2, new string[] { "azul", "celeste" })) color2 = settings.GetColorIndexFor("amarillo");
            else if (SimialColors(color1, color2, new string[] { "azul", "violeta" })) color2 = settings.GetColorIndexFor("blanco");
            return color2;
        }
        bool SimialColors(int color1, int color2, string[] arr)
        {
            Settings settings = Data.Instance.settings;
            bool color_1_Matches = false;
            bool color_2_Matches = false;
            foreach (string color in arr)
            {
                if (color1 == settings.GetColorIndexFor(color)) color_1_Matches = true;
                if (color2 == settings.GetColorIndexFor(color)) color_2_Matches = true;
            }
            if (color_1_Matches && color_2_Matches) return true;
            return false;
        }
        public Color GetColorByIndex(int index)
        {
            if (index > colorStyles.Length - 1)
                index = 0;
            return colorStyles[index].color;
        }
        public List<int> GetOppositeColorIndexesByIndex(int index) {
            if (index > colorStyles.Length - 1)
                index = 0;
            List<int> results = new List<int>();            
            foreach (string id in colorStyles[index].oppositeIds) 
                results.Add(GetColorIndexFor(id));
            return results;
        }
        public Stats GetStat(stat stat)
        {
            foreach (Stats s in statsSettings)
                if (s.stat == stat)
                    return s;
            return null;
        }
    }

}