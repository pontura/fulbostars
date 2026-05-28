using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Fulbo.AssetsBundle;
using Fulbo.UI;
using Fulbo.DB;

namespace Fulbo
{
    public class CharactersData : MonoBehaviour
    {
        public bool reload;
        public string myPath;
        static CharactersData mInstance = null;
        public bool loaded;

        [SerializeField] int referiId = 1;
        public static CharactersData Instance
        {
            get { return mInstance; }
        }
        [Serializable]
        public class CharacterData
        {
            public string avatarName;
            public int uniqueID; // "id" of player saved on DB.DBUserData
            public int id;
            public int positionID;
            public bool isReferi;
            public bool isGoalkeeper;
            public GameObject asset;
            public Sprite thumb;
            public List<AudioClip> audio_names;
            public List<AudioClip> audio_goal;
            public List<AudioClip> comments_goal;
            public CharacterStats stats;
            public string nationality;
            public string text;
            public List<int> upgrades;
            public FigusData.rarities rarity; // for partymode!
            public string status = "";

            public void SetDataFromDB(DBUserData.DBCharacterData dbData)
            {
                id = dbData.player_id;
                uniqueID = dbData.id;
                stats.ForceStats(dbData);
            }

            public bool IsAvailable()
            {
                if (status == "blocked") return false;
                return true;
            }

            public void SetUpgrades(int[] arr)
            {
                foreach (int a in arr)
                    upgrades.Add(a);
            }
            public void Upgrade(int id)
            {
                InitUpgrades();
                upgrades[id]++;
            }
            void InitUpgrades()
            {
                if (upgrades.Count == 0)
                    upgrades = new List<int> { 0, 0, 0, 0, 0 };
            }
            public int GetTotalStats(bool considerPosition)
            {
                int total = stats.GetTotal(considerPosition);
                foreach (int upgradeValue in upgrades)
                    total += upgradeValue;
                return total;
            }
            public CharacterStats GetStats()
            {
                CharacterStats totalStats = new CharacterStats();
                totalStats = stats;
                if (upgrades.Count > 0)
                {
                    totalStats.accuracy += upgrades[0];
                    totalStats.stamina += upgrades[1];
                    totalStats.speed += upgrades[2];
                    totalStats.dexterity += upgrades[3];
                    totalStats.awareness += upgrades[4];
                }
                return totalStats;
            }
            public CharacterStats GetStatsFromStoryMode(CharacterStats cs)
            {
                CharacterStats totalStats = new CharacterStats();
                totalStats.accuracy = cs.accuracy;
                totalStats.stamina = cs.stamina;
                totalStats.speed = cs.speed;
                totalStats.dexterity = cs.dexterity;
                totalStats.awareness = cs.awareness;
                return totalStats;
            }
        }
        public List<CharacterData> all;
        public List<CharacterData> all_goalkeepers;
        public List<CharacterData> all_referis;

        [HideInInspector] public List<int> availablesTeam1;
        [HideInInspector] public List<int> availablesTeam2;
        [HideInInspector] public List<int> availablesTeam1_goalkeepers;
        [HideInInspector] public List<int> availablesTeam2_goalkeepers;

        [SerializeField] int totalCharacters;
        [SerializeField] int totalGoalKeepers;

        AssetsBundleManager assetsBundleManager;

        void Awake()
        {
            if (!mInstance)
                mInstance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this);
        }
        System.Action OnDone;
        public void Init(System.Action OnDone)
        {
            this.OnDone = OnDone;
            assetsBundleManager = AssetsBundleManager.Instance;
            LoadCharacters();
            //LoadAudios();
            totalCharacters = all.Count;
            totalGoalKeepers = all_goalkeepers.Count;

            availablesTeam1.Clear();
            availablesTeam2.Clear();

            availablesTeam1_goalkeepers.Clear();
            availablesTeam2_goalkeepers.Clear();

            InitAll();
            OnDone();
            //GetComponent<CharactersDefaultData>().Init(OnDone);
            //  loaded = true;
        }
        public void LoadCharactersDefaultData(System.Action OnDone)
        {
            this.OnDone = OnDone;
            GetComponent<CharactersDefaultData>().Init(OnDone);
            loaded = true;
        }
        //public void ForceAddAllCharacters()
        //{
        //    print("________ForceAddAllCharacters______________");
        //    if (Data.Instance.forceAddAllCharacters)
        //    {
        //        foreach (CharacterData characterData in all)
        //        {
        //            if (CanBeAdded(characterData))
        //                Data.Instance.myTeam.SetCharacter(characterData.id, Game.Character.types.FOR, true);
        //        }
        //        foreach (CharacterData characterData in all_goalkeepers)
        //        {
        //            if (CanBeAdded(characterData))
        //                Data.Instance.myTeam.SetCharacter(characterData.id, Game.Character.types.GOALKEEPER, true);
        //        }
        //    }
        //}
        //bool CanBeAdded(CharacterData characterData)
        //{
        //    // if (characterData.rarity == FigusData.rarities.NORMALITO && !Data.Instance.myTeam.HasThisCharacter(Data.Instance.myTeam.goalkeepers, characterData.id, characterData.isGoalkeeper))
        //    if (!Data.Instance.myTeam.HasThisCharacter(Data.Instance.myTeam.goalkeepers, characterData.id, characterData.isGoalkeeper))
        //        return true;
        //    return false;
        //}
        private void InitAll()
        {
            for (int a = 1; a < totalCharacters + 1; a++)
            {
                if (a <= (totalCharacters + 1) / 2)
                    availablesTeam1.Add(a);
                else
                    availablesTeam2.Add(a);
            }
            for (int a = 1; a < totalGoalKeepers + 1; a++)
            {
                if (a <= (totalGoalKeepers + 1) / 2)
                    availablesTeam1_goalkeepers.Add(a);
                else
                    availablesTeam2_goalkeepers.Add(a);
            }

            Utils.Shuffle(availablesTeam1);
            Utils.Shuffle(availablesTeam2);

            Utils.Shuffle(availablesTeam1_goalkeepers);
            Utils.Shuffle(availablesTeam2_goalkeepers);

        }

        public GameObject GetCharacterByTeam(int teamID, int id, bool isGoalKeeper)
        {
            int teamCharacterId = 0;

            if (teamID == 1)
                teamCharacterId = Data.Instance.matchData.GetTeam(1)[id];
            else
                teamCharacterId = Data.Instance.matchData.GetTeam(2)[id];

            CharacterData d = GetCharacterData(teamCharacterId, isGoalKeeper);
            return d.asset;
        }
        public CharacterData GetCharacterDataByTeam(int teamID, int id, bool isGoalKeeper)
        {
            int teamCharacterId = 0;
            List<int> team = Data.Instance.matchData.GetTeam(teamID);
            if (id > -1 && id < team.Count)
                teamCharacterId = team[id];            

            CharacterData d = GetCharacterData(teamCharacterId, isGoalKeeper);
            //print(teamID + " id: " + id);
            return d;
        }
        public List<CharacterData> GetReferies()
        {
            List<CharacterData> allavailable = new List<CharacterData>();
            foreach (CharacterData cd in all_referis)
            {
                if(cd.IsAvailable())
                    allavailable.Add( cd );
            }
            return allavailable;
        }
        public List<int> GetAvailablePlayersID(bool isGoalKeeper)
        {
            List<CharacterData> arr = GetAvailablePlayers(isGoalKeeper);
            List<int> allavailable = new List<int>();
            foreach(CharacterData cd in arr)
                allavailable.Add(cd.id);
            return allavailable;
        }
        public List<CharacterData> GetAvailablePlayers( bool isGoalKeeper)
        {
            List<CharacterData> allavailable = new List<CharacterData>();
            List<CharacterData> players = new List<CharacterData>();

            bool showAll = false;
            //if (Data.Instance.mode == Data.modes.PARTYMODE && Data.Instance.settings.mainSettings.isArcade)
            //    showAll = true;

            if (isGoalKeeper) players = all_goalkeepers;
            else players = all;

            foreach (CharacterData characterData in players)
            {
                //if (showAll)
                //    allavailable.Add(characterData);
                //else 
                if(characterData.IsAvailable())
                    allavailable.Add(characterData);
            }        

            return allavailable;
        }
        public CharacterData GetCharacterData(int characterID, bool isGoalkeeper, bool check_null=false)
        {
            List<CharacterData> arr;
            if (isGoalkeeper)
                arr = all_goalkeepers;
            else
                arr = all;
            foreach (CharacterData cd in arr)
            {
                if (cd.id == characterID)
                    return cd;
            }           
            Debug.LogError("No hay character: " + characterID + " isGoalKeeper " + isGoalkeeper);

            if (check_null)
                return null;
            else
                return arr[0];
            //return null;
        }

        string[] names_players;
        string[] names_goalkeepers;
        string[] names_referis;
        string[] goals;
        string[] comments;

        void LoadCharacters()
        {
            string lang = Data.Instance.langsManager.GetLang();
            names_players = assetsBundleManager.assetsBundleLoader.bundles[lang + "/players.1_100"].GetAllAssetNames();
            names_goalkeepers = assetsBundleManager.assetsBundleLoader.bundles[lang + "/goalkeepers.1_100"].GetAllAssetNames();
            names_referis = assetsBundleManager.assetsBundleLoader.bundles[lang + "/referis.1_100"].GetAllAssetNames();

            goals = assetsBundleManager.assetsBundleLoader.bundles[lang + "/goals.1_100"].GetAllAssetNames();
          //  comments = assetsBundleManager.assetsBundleLoader.bundles[lang + "/comments.1_100"].GetAllAssetNames();

            string[] referis = assetsBundleManager.assetsBundleLoader.bundles["referis.1_100"].GetAllAssetNames();
            string[] players = assetsBundleManager.assetsBundleLoader.bundles["players.1_100"].GetAllAssetNames();
            string[] goalkeepers = assetsBundleManager.assetsBundleLoader.bundles["goalkeepers.1_100"].GetAllAssetNames();

           // Debug.Log("___________players count in bundle: " +             players.Length);

            all.Clear();
            all_goalkeepers.Clear();
            all_referis.Clear();

            AddCharacter(referis, "referis", false);
            AddCharacter(players, "players", false);
            AddCharacter(goalkeepers, "goalkeepers", true);


            names_players = null;
            names_goalkeepers = null;
            names_referis = null;
            goals = null;
            comments = null;

        }
        void AddCharacter(string[] arr, string folder, bool _isGoalkeeper)
        {
            foreach (string s in arr)
            {
                string[] cuttedDash = s.Split("/"[0]);
                string[] cuttedPoint = cuttedDash[cuttedDash.Length - 1].Split("."[0]);

                int id = int.Parse(cuttedPoint[0]); // toma el id del nombre del asset:
                CharacterData characterData = new CharacterData();
                characterData.stats = new CharacterStats();
                characterData.audio_names = new List<AudioClip>();
                characterData.audio_goal = new List<AudioClip>();
                characterData.comments_goal = new List<AudioClip>();
                characterData.upgrades = new List<int>();

                characterData.id = id;
                characterData.isGoalkeeper = _isGoalkeeper;

                string xtra = "";
                string folderComplete = folder + ".1_100";
                string spritePath = "";
                string thumbs = "thumbs.1_100";

                if (folder == "referis")
                {
                    characterData.isReferi = true;
                    //Assets/Characters/referies/thumbs/thumb_referi_2.png
                    spritePath = "assets/characters/referies/thumbs/thumb_referi_" + id + ".png";
                }
                else if (folder == "goalkeepers")
                    spritePath = "assets/characters/1_100/players_thumbnails/thumb_goalkeeper_" + id + ".png";
                else
                    spritePath = "assets/characters/1_100/players_thumbnails/thumb_" + id + ".png";


                Texture2D tex = assetsBundleManager.assetsBundleLoader.GetAssetAsTexture2D(thumbs, spritePath);
                if (tex != null)
                {
                    Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    characterData.thumb = mySprite;
                }
                characterData.asset = assetsBundleManager.assetsBundleLoader.GetAsset(folderComplete, s);
                if (folder == "players")
                    all.Add(characterData);
                else if (folder == "goalkeepers")
                    all_goalkeepers.Add(characterData);
                else if (folder == "referis")
                    all_referis.Add(characterData);

                LoadAudios(characterData);
            }
        }

        void LoadAudios(CharacterData cData)
        {
            string lang = Data.Instance.langsManager.GetLang();
            if (cData.isReferi)
            {
                LoadAudiosIn(cData, cData.audio_names, names_referis, lang + "/referis.1_100");
            }
            else if (cData.isGoalkeeper)
            {
                LoadAudiosIn(cData, cData.audio_names, names_goalkeepers, lang + "/goalkeepers.1_100");
            }
            else
            {
                LoadAudiosIn(cData, cData.audio_names, names_players, lang + "/players.1_100");
                LoadAudiosIn(cData, cData.audio_goal, goals, lang + "/goals.1_100");
               // LoadAudiosIn(cData, cData.comments_goal, comments, lang + "/comments.1_100");
            }
            //CheckAudioNamesByType(cData);
        }
        //void CheckAudioNamesByType(CharacterData cData)
        //{
        //    List<AudioClip> arr = new List<AudioClip>();
        //    foreach (AudioClip ac in cData.audio_names)
        //    {
        //        if (ac.name.Contains("_low"))
        //            arr.Add(ac);
        //    }
        //    if (arr.Count > 0)
        //    {
        //        foreach (AudioClip ac in arr)
        //        {
        //            cData.audio_names.Remove(ac);
        //            cData.audio_names_low.Add(ac);
        //        }
        //    }
        //}
        void LoadAudiosIn(CharacterData data, List<AudioClip> audioclips, string[] files, string folder)
        {
            string characterID = data.id.ToString();
            foreach (string s in files)
            {
                string[] arr = s.Split("/"[0]);
                string id_name = arr[arr.Length - 1];
                string[] n = id_name.Split("_"[0]);
                if (n.Length > 1)
                    id_name = n[0];
                else
                    id_name = id_name.Split("."[0])[0];

                if (id_name == characterID)
                {
                    // ac = new WWW(fi.FullName).GetAudioClip(false, true, AudioType.WAV);
                    AudioClip ac = Fulbo.AssetsBundle.AssetsBundleManager.Instance.assetsBundleLoader.GetAssetAsAudioClip(folder, s);
                    audioclips.Add(ac);
                }
            }
        }
        public List<int> GetCharactersIDByRarity(FigusData.rarities rarity, bool isGoalKeeper)
        {
            List<int> arr_rarity = new List<int>();
            List<CharacterData> arr;
            if (isGoalKeeper)
                arr = all_goalkeepers;
            else
                arr = all;
            foreach (CharacterData cd in arr)
            {
                if (cd.rarity == rarity)
                    arr_rarity.Add(cd.id);
            }
            return arr_rarity;
        }
        public CharacterData GetReferi()
        {
            return GetReferi(referiId);
        }
        public CharacterData GetReferi(int id)
        {
            foreach (CharacterData cd in all_referis)
                if (id == cd.id)
                    return cd;
            Debug.LogError("No hay referi id: " + id);
            return null;
        }
        public void SetReferi(int id)
        {
            this.referiId = id;
        }
         public void SetRandomReferi()
         {
            int randomIndex = UnityEngine.Random.Range(0, all_referis.Count);
            print("SetRandomReferi "+ randomIndex + " total: " + all_referis.Count);
            this.referiId = all_referis[randomIndex].id;
        }
    }
}