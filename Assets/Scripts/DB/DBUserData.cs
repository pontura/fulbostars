using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using static Fulbo.Settings;
using UnityEngine.UIElements;
using Fulbo.Game;

namespace Fulbo.DB
{
    public class DBUserData : MonoBehaviour
    {
        List<int> wonCharacters;

        public userStates state;
        public enum userStates
        {
            LOGGED_IN,
            FIRST_TIME            
        }
        public types type;
        public enum types
        {
            REGISTERED,
            GUEST
        }
        public UserData pvpData;
        public UserData data;
        [Serializable]
        public class DataFromServer
        {
            public UserData user;
            public string hash;
            public List<DBCharacterData> players;
            public List<DBCharacterData> goalkeepers;
        }
        [Serializable]
        public class DBCharacterData
        {
            string avatarName;
            public string AvatarName()
            {
                if (avatarName == "")
                {
                    avatarName = CharactersData.Instance.GetCharacterData(player_id, IsGoalkeeper()).avatarName;
                    return avatarName;
                }
                Debug.Log("No name for " + player_id);
                return "";
            }
            public int id;
            public string role;
            public int player_id;
            public int user_id;
            public int accuracy;
            public int stamina;
            public int speed;
            public int dexterity;
            public int trickery;
            public int position;
            public int rarity;
            public int tier;
            public int xp;                  // experiencia
            public int currentTierMaxXP;
            public int maxLevelFromTier;
            public int upgraded_stats;      // cantidad de puntos de stats upgradeados en total
            public int total_stats;         // cantidad total de puntos de stats
            public int sell_price;          // precio para la venta del personaje
            public int level;               // nivel del personaje
            public int current_level_xp;    // experiencia en el nivel actual
            public int xp_to_next_level;    // cantidad de experiencia necesaria para llegar al próximo nivel
            public int stats_from_level;    // cantidad de puntos de stats liberados en el nivel actual
            public int available_stats;     // cantidad de puntos de stats disponibles para upgradear
            public int price_per_stat;      // precio actual para cada punto de stat

            public bool canUpdateTier;
            public int tierPriceHard;
            public int tierPriceShards;

            public int GetTotalStats()
            {
                return accuracy + stamina + speed + dexterity + trickery;
            }
            public string GetPositionText()
            {
                return Data.Instance.textsData.GetPositionName(role == "GOALKEEPER", id, true);
            }
            public bool IsGoalkeeper()
            {
                return role == "GOALKEEPER";
            }
            public FigusData.rarities GetRarity() // TO-DO : Esto debería venir del server en rarity:
            {
                // return rarity;
                CharactersData.CharacterData characterData = CharactersData.Instance.GetCharacterData(player_id, role == "GOALKEEPER");
                if (characterData == null) return 0;
                return characterData.rarity;
            }

            public static List<DBCharacterData> SortCharacters(List<DBCharacterData> list, SortOrder order)
            {
                DBCharacterData holder;

                switch (order)
                {
                    case SortOrder.StatsGK:
                        for (int i = 0; i < list.Count; i++)
                        {
                            int j = i;
                            while (j > 0)
                            {
                                if (!list[j].IsGoalkeeper() && list[j - 1].IsGoalkeeper())
                                    break;
                                else if (list[j].total_stats <= list[j - 1].total_stats)
                                    break;

                                //Swap
                                holder = list[j - 1];
                                list[j - 1] = list[j];
                                list[j] = holder;
                                j--;
                            }
                        }
                        break;
                    case SortOrder.Stats:
                        for (int i = 0; i < list.Count; i++)
                        {
                            int j = i;
                            while (j > 0)
                            {
                                if (list[j].total_stats <= list[j - 1].total_stats)
                                    break;

                                //Swap
                                holder = list[j - 1];
                                list[j - 1] = list[j];
                                list[j] = holder;
                                j--;
                            }
                        }
                        break;
                    case SortOrder.Upgradeable:

                        //Esto se re puede optimizar

                        list = SortCharacters(list, SortOrder.StatsGK);

                        for (int i = 0; i < list.Count; i++)
                        {
                            int j = i;
                            while (j > 0)
                            {
                                if (list[j].available_stats <= list[j - 1].available_stats)
                                    break;

                                //Swap
                                holder = list[j - 1];
                                list[j - 1] = list[j];
                                list[j] = holder;
                                j--;
                            }
                        }
                        break;
                }

                return list;
            }

            public enum SortOrder
            {
                Stats,
                StatsGK,
                Upgradeable,
            }
        }
        [Serializable]
        public class UserData
        {
            public int id;
            public string discord;
            public string twitter;
            public string user;
            public string shortName;
            public string style;
            public int gamesPlayed;
            public int score;
            public int old_score;
            public int shards;
            public int hard_currency;
            public string country;
            public int daily_reward_chest_count;
            //[HideInInspector] 
            public string game_data;
            public DBGameData.Content gameData;

            public List<DBCharacterData> players_characters;
            public List<DBCharacterData> players_goalkeepers;

            [HideInInspector] public string hash;

            public string Name()
            {
                if(user.Length>13)
                {
                    return user.Substring(0, 13);
                }
                return user;
            }
            public int Score()
            {
                return score;
            }
            public void AddScore(int add) // TO-DO Replace with server!
            {
                score += add;
                Events.RefreshScore(score);
            }
            public int GetCharacterType(int id) // 0 def 1 mid 2 for
            {
                foreach (DBCharacterData cdb in players_characters)
                    if(cdb.id == id)
                    return cdb.position;
                return 0;
            }
            public List<DBCharacterData> GetCharacters(bool isGoalkeeper)
            {
                List<DBCharacterData> arr = new List<DBCharacterData>();
                List<DBCharacterData> players;
                if (isGoalkeeper) players = players_goalkeepers;
                else players = players_characters;
                foreach (DBCharacterData cdb in players)
                    arr.Add(cdb);

                return arr;
            }
            public List<int> GetCharacterIds(bool isGoalkeeper)
            {
                List<int> arr = new List<int>();
                List<DBCharacterData> players;
                if (isGoalkeeper) players = players_goalkeepers;
                else players = players_characters;
                foreach (DBCharacterData cdb in players)
                    arr.Add(cdb.player_id);

                return arr;
            }
            public void ReplaceOnSelectedTeam(int id_from, int id_to, int formationQty)
            {
               // print("REPLACE " + id_from + " to: " + id_to + "    formationQty: " + formationQty);
                gameData.Swap(id_from, id_to, formationQty);
            }
            public DBCharacterData GetPlayerByID(int id)
            {
                foreach (DBCharacterData cdb in players_characters)
                    if (cdb.id == id)
                        return cdb;
                foreach (DBCharacterData cdb in players_goalkeepers)
                    if (cdb.id == id)
                        return cdb;
                return null;
            }
            public void SetCountry(string shortName)
            {
                this.country = shortName;
            }
        }
        private void Start()
        {
            DBEvents.LoadUserData += LoadUserData;
            DBEvents.SaveUserData += SaveUserData;
            //DBEvents.UpdateCharacters += UpdateCharacters;
            Events.ResetApp += ResetApp;
        }
        private void OnDestroy()
        {
            DBEvents.LoadUserData -= LoadUserData;
            DBEvents.SaveUserData -= SaveUserData;
           // DBEvents.UpdateCharacters -= UpdateCharacters;
            Events.ResetApp -= ResetApp;
        }
        void ResetApp()
        {
            data = new UserData();
            Events.RefreshScore(data.score);
            Events.RefreshHardCurrency(data.hard_currency);
        }
        public void LoadUserData(System.Action OnSuccess)
        {
            if (type == types.GUEST)
            {
                LoadUserDataAsGuest();
                OnSuccess();
            }
            else
                StartCoroutine(LoadUserDataC(OnSuccess, DBManager.Instance.UrlLoadUserData));
        }
        
        IEnumerator LoadUserDataC(System.Action OnSuccess, string url)
        {
            WWWForm form = new WWWForm();
            UnityWebRequest www = UnityWebRequest.Get(url);

            print("[GET] " + url);

            yield return www.SendWebRequest();

            if (www.isNetworkError)
                Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
            else
            {
                string s = www.downloadHandler.text;
                Debug.Log(s);
                DataFromServer d = JsonUtility.FromJson<DataFromServer>(s);
                data = d.user;

                DBManager.Instance.DbGameData.ParseContent(d.user.game_data);

                if (d.goalkeepers.Count>0)   data.players_goalkeepers = d.goalkeepers;
                if (d.players.Count > 0)    data.players_characters = d.players;

                Events.RefreshScore(d.user.score);
                Events.RefreshHardCurrency(d.user.hard_currency);

                if (data != null && OnSuccess != null)
                {
                    //data.SetDefaultSelectedTeam();
                    OnSuccess();
                }

                Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
            }
        }


        [Serializable]
        class SaveUserDB
        {
            public string user;
            public string shortName;
            public string style;
            public string twitter;
            public string discord;
            public string hash;
            public string country;
        }
        public void SaveUserData(UserData data, System.Action<bool, string> OnSuccess)
        {
            if(type == types.GUEST)
            {
                SaveUserDataAsGuest(data);
                if(OnSuccess != null)  OnSuccess(true, "");
                return;
            }

            if (data.twitter == null) data.twitter = "";
            if (data.discord == null) data.discord = "";
            if (data.country == null) data.country = this.data.country;

            //this.data.players_characters = data.players_characters;
            //this.data.players_goalkeepers = data.players_goalkeepers;

            this.data.user = data.user;
            this.data.shortName = data.shortName;
            this.data.style = data.style;
            this.data.twitter = data.twitter;
            this.data.country = data.country;

            string hashString =
                DBManager.Instance.Email + DBManager.HASH_SALT1 +
                data.user +
                data.twitter +
                data.discord +
                data.shortName +
                data.style +
                data.country;

            SaveUserDB sData = new SaveUserDB();
            sData.user = data.user;
            sData.shortName = data.shortName;
            sData.style = data.style;
            sData.twitter = data.twitter;
            sData.discord = data.discord;
            if (data.country != "")
                sData.country = data.country;

            sData.hash =  Utils.SHA(hashString);

            string json = JsonUtility.ToJson(sData, true);
            DBManager.Instance.Request(DBManager.Instance.UrlSaveUserData, json, OnSuccess, "PUT", "Updating User Data");
        }
        public string GetFormatedStyle()
        {
            string result = "";
            result += Data.Instance.myTeam.clubData.shieldDesignID + ".";
            result += Data.Instance.myTeam.clubData.clubColor1 + "."; 
            result += Data.Instance.myTeam.clubData.clubColor2 + ".";
            result += Data.Instance.myTeam.clubData.clubColor3 + ".";
            result += Data.Instance.myTeam.clubData.clubColor4 + ".";
            result += Data.Instance.myTeam.clubData.designID + ".";
            result += Data.Instance.myTeam.clubData.logo;
            return result;
        }
        public void SetType(types _type)
        {
            this.type = _type;
        }




        class BuyPlayersData
        {
            public int id;
            public string role;
            public int position;
            public string device;
            public string version;
            public string hash;
        }
        BuyPlayersData lastCharacterBought;
        public void BuyPlayer(int player_id, bool isGoalkeeper, int position, System.Action<bool, string> OnSuccess)
        {
            lastCharacterBought = new BuyPlayersData();
            lastCharacterBought.id = player_id;

            if (isGoalkeeper)
                lastCharacterBought.role = "GOALKEEPER";
            else
                lastCharacterBought.role = "PLAYER";

            lastCharacterBought.position = position;

            lastCharacterBought.device = Application.platform.ToString();
            lastCharacterBought.version = Application.version;

            string hastString =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 +
                lastCharacterBought.role +
                lastCharacterBought.id;

            if(!isGoalkeeper) hastString += lastCharacterBought.position;

            lastCharacterBought.hash = Utils.SHA(hastString);

            string json = JsonUtility.ToJson(lastCharacterBought, true);

            DBManager.Instance.Request(DBManager.Instance.UrlBuyPlayerData, json, OnSuccess, "POST", Data.Instance.texts.Get("http_updating_players"));

            //Analytics

            TextsData.CharacterData textData = Data.Instance.textsData.GetCharactersData(lastCharacterBought.id, lastCharacterBought.role == "GOALKEEPER");
            CharactersData.CharacterData charData = CharactersData.Instance.GetCharacterData(lastCharacterBought.id, lastCharacterBought.role == "GOALKEEPER");

            
        }
        public DBCharacterData GetLastCharacterBought()
        {
            if(lastCharacterBought == null)
            {
                Debug.LogError("Not bought player");
                return null;
            }
            if (lastCharacterBought.role == "GOALKEEPER")
                return data.players_goalkeepers[data.players_goalkeepers.Count - 1];
            else
                return data.players_characters[data.players_characters.Count - 1];
        }

        #region //SELLING
        class SellPlayersData
        {
            public int sell_price;
            public string device;
            public string version;
            public string hash;
        }
        public void SellPlayer(DBCharacterData uData, System.Action<bool, string> OnSuccess)
        {
            SellPlayersData thisData = new SellPlayersData();

            thisData.device = Application.platform.ToString();
            thisData.version = Application.version;
            thisData.sell_price = uData.sell_price;

            string hastString =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 +
                uData.id +
                uData.sell_price;

            thisData.hash = Utils.SHA(hastString);

            string json = JsonUtility.ToJson(thisData, true);
            DBManager.Instance.Request(DBManager.Instance.UrlSellPlayerData(uData.id), json, OnSuccess, "PUT", Data.Instance.texts.Get("http_updating_players"));

            //Analytics
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["role"] = uData.role;
            param["power"] = uData.GetTotalStats();
            param["rarity"] = uData.rarity;
            param["characterName"] = uData.AvatarName(); //Más intuitivo de leer que pasar el número del ID
            param["sellPrice"] = uData.sell_price;


            Events.OnTrack("CharacterSold", param);
        }
        #endregion







        //GUEST
        void SaveUserDataAsGuest(UserData data)
        {
            this.data.user = data.user;
            this.data.shortName = data.shortName;
            this.data.style = data.user;
            this.data.twitter = data.twitter;

            PlayerPrefs.SetString("user", data.user);
            PlayerPrefs.SetString("shortName", data.shortName);
            PlayerPrefs.SetString("style", data.style);
            PlayerPrefs.SetString("twitter", data.twitter);
        }
        void LoadUserDataAsGuest()
        {
            this.data.user = PlayerPrefs.GetString("user");
            this.data.shortName = PlayerPrefs.GetString("shortName");
            this.data.style = PlayerPrefs.GetString("style");
            this.data.twitter = PlayerPrefs.GetString("twitter");
        }
        public void SetWonCharacters(List<int> arr)
        {
            ResetWonCharacters();
            foreach (int id in arr)
                wonCharacters.Add(id);
        }
        public List<DBCharacterData> GetWonCharacters()
        {
            List<DBCharacterData> arr = new List<DBCharacterData>();
            foreach (int id in wonCharacters)
                arr.Add(data.GetPlayerByID(id));
            wonCharacters = null;
            return arr;
        }
        public void ResetWonCharacters()
        {
            wonCharacters = new List<int>();
        }
        public void Delete(System.Action<bool, string> OnSuccess)
        {
            string email = DB.DBManager.Instance.Email;
            DBManager.Instance.Request(DBManager.Instance.URL + "users/" + email, "{}", OnSuccess, "DELETE", "");
        }
    }
}
