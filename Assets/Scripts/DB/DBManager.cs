using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Fulbo.Tournamets;

namespace Fulbo.DB
{
    public class DBManager : MonoBehaviour
    {
        public versionModes versionMode;
        public enum versionModes
        {
            PROD,
            DEV
        }
        public usersDEV user;
        public enum usersDEV
        {
            BRENDA,
            DARIO,
            RUD,
            CAPI,
            PELU,
            PONTURA,
            RORI,
            DAMU,
            DEMO1,
            DEMO2,
            DEMO3,
            DEMO4,
            DEMO5
        }
        public TournamentsManager tournamentsManager;
        public string ranking_url = "https://ranking.fulbogalaxy.com/rankingByStadium/";
        public string myStats_url = "https://ranking.fulbogalaxy.com/myStats/";

        const string url_prod = "https://y0e8xxplkh.execute-api.us-east-1.amazonaws.com/";
        const string url_dev = "https://o0fgc808k9.execute-api.us-east-1.amazonaws.com/";
        const string url_auth_prod = "https://cognito-idp.us-east-1.amazonaws.com/";
        const string url_auth_dev = "https://cognito-idp.us-east-1.amazonaws.com/";

        const string clientId_prod = "5t3avnbg4lf9bm765q7vg0j7t7";
        const string clientId_dev = "4k4ape9ba9o45jhuoa0pnjh9sf";


        public static string HASH_SALT1 = "mVb64YjgfL8Gv7z";
        public static string HASH_SALT2  = "QzC39aC3GzkPL3W";

        public DBTokens tokens; // amazon:

        public string GetURL()
        {
            switch(versionMode)
            {
                case versionModes.DEV: return url_dev;
                default: return url_prod;
            }
        }
        public string GetURLAuth()
        {
            switch (versionMode)
            {
                case versionModes.DEV: return url_auth_dev;
                default: return url_auth_prod;
            }
        }
        public string GetClientID()
        {
            switch (versionMode)
            {
                case versionModes.DEV: return clientId_dev;
                default: return clientId_prod;
            }
        }
        [SerializeField] string email;
        [SerializeField] string socialUserName;

        static DBManager mInstance;
        DBGameData dBGameData;
        public DBServerConfig dBServerConfig;
        DBTrack dBTrack;
        DBStats dBStats;
        DBChangeCharacterPosition dBChangeCharacterPosition;
        DBRegisterTeam dbRegisterTeam;
        DBMatches dbMatches;
        DBMatchesPerLevel dbMatchesPerLevel;
        DBUserData dBUserData;
        DBCoins dBCoins;
        DBEnergy dBEnergy;
        DBTier dBTier;
        DBMundial dBMundial;
        DBAds dBAds;
        DBPlayerPacks dBPlayerPacks;
        string editorEmail = "@GooglePlayGames";

        DateTime initDateTime;
        float startUpOffset;

        public void SetSocialUserName(string _socialUserName)
        {
            this.socialUserName = _socialUserName;
        }
        public static DBManager Instance { get { return mInstance; } }

        void Awake()
        {            
            if (!mInstance)
                mInstance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }

#if UNITY_EDITOR
            editorEmail = DB.DBManager.Instance.user.ToString() + editorEmail;
            SetEmail(editorEmail);
            SetSocialUserName(DB.DBManager.Instance.user.ToString());
#endif


            DontDestroyOnLoad(this.gameObject);
            dBServerConfig = new DBServerConfig();
            dBTrack = new DBTrack();
            dBStats = new DBStats();
            dbRegisterTeam = new DBRegisterTeam();
            dBChangeCharacterPosition = new DBChangeCharacterPosition();
            dBGameData = new DBGameData();
            dBCoins = new DBCoins();
            dBEnergy = new DBEnergy();
            dBTier = new DBTier();
            dBPlayerPacks = new DBPlayerPacks();

            dBAds = GetComponent<DBAds>();
            dBMundial = GetComponent< DBMundial>();
            dbMatches = GetComponent<DBMatches>();
            dbMatchesPerLevel = GetComponent<DBMatchesPerLevel>();
            dBUserData = GetComponent<DBUserData>();
            tournamentsManager = GetComponent<TournamentsManager>();    
            
        }
        private void Start()
        {
            email = PlayerPrefs.GetString("email", "");
            tokens.Load();
            DBEvents.UpgradeStat += UpgradeStat;
            DBEvents.Track += Track;
            DBEvents.OnChangeCharacterPosition += OnChangeCharacterPosition;
            DBEvents.OnRegisterTeam += OnRegisterTeam;            
        }
        void OnDestroy()
        {
            DBEvents.UpgradeStat -= UpgradeStat;
            DBEvents.Track -= Track;
            DBEvents.OnChangeCharacterPosition -= OnChangeCharacterPosition;
            DBEvents.OnRegisterTeam -= OnRegisterTeam;
        }
        public void LoadServerConfigFile(System.Action<bool, string> OnDone)
        {
            dBServerConfig.Load(OnDone);
        }
        public bool IsOldVersion(string _appVersion, string _prodVersion)
        {
            float appVerion = GetVersionNum(_appVersion);
            float prodVersion = GetVersionNum(_prodVersion);
          //  Debug.Log("__________" + _appVersion + " _prodVersion" + _prodVersion +  "_______________appVerion: " + appVerion + "     prodVersion: " + prodVersion);
            if (appVerion < prodVersion)
                return true;
            return false;
        }
        float GetVersionNum(string num)
        {
            string[] numArr = num.Split("."[0]);
            if (numArr.Length == 2)
                return (float.Parse(numArr[0])* 100000) + (float.Parse(numArr[1])* 1000);
            if (numArr.Length == 3)
                return (float.Parse(numArr[0]) * 100000) + (float.Parse(numArr[1]) * 1000) + (float.Parse(numArr[2]));
            else
                return float.Parse(num);
        }
        public void SetConfigFile(System.Action<bool, string> OnDone)
        {
            string version = Application.version;

            DBServerConfig.VersionsData[] versions = dBServerConfig.configData.versions;

#if UNITY_IOS
                versions = dBServerConfig.configData.versions_ios;
#endif

            // Debug.Log("Checking version " + version + " versions: " + dBServerConfig.configData.versions.Length);
            foreach (DBServerConfig.VersionsData versionData in versions)
            {
                //Debug.Log("version " + version + "  versionData from server: " + versionData.num);
                string versionNum = versionData.num;

                if (IsOldVersion(version, versionNum))
                {
                    Debug.LogError("old version");
                    OnDone(false, versionNum);
                    return;
                } else
                if (versionNum == version)
                {
                    if (versionData.status == "offline") {
                        OnDone(false, "offline");
                        return;
                    } else
                    if (versionData.from.ToUpper() == "PROD") {
                        versionMode = versionModes.PROD;
                        SetDateTime();
                    } else versionMode = versionModes.DEV;

                   // Debug.Log("Checking version DONE: " + versionMode);

                    OnDone(true, versionData.from.ToUpper());
                    return;
                }
            }
            versionMode = versionModes.DEV;
            OnDone(true, "DEV");
        }

        void SetDateTime() {
            initDateTime = Utils.NetworkTime();
            startUpOffset = Time.realtimeSinceStartup;
        }

        public void Logout()
        {
            tokens.DeleteAll();
        }
        public void SetTokens(DBTokens dbTokens)
        {
            this.tokens = dbTokens;
        }
        void OnRegisterTeam(DBRegisterTeam.RData data, System.Action<bool, string> OnDone)
        {
           dbRegisterTeam.Register(data, GetURL() + "users/", OnDone);
        }
        void UpgradeStat(int characterID, Settings.stat stat, System.Action<bool, string> OnSuccess)
        {
            dBStats.Upload(characterID, stat, GetURL() + "users/" + email + "/characters/" + characterID + "/stats/" + stat.ToString(), OnSuccess);
        }
        void Track(DBMatches.MatchData data, System.Action<bool, string> OnSuccess)
        {
            if(DBManager.Instance.DbUserData.type != DBUserData.types.GUEST)
                dBTrack.Upload(data, GetURL() + "users/" + email + "/track" , OnSuccess);
        }
        void OnChangeCharacterPosition(int characterID, int position, System.Action<bool, string> OnDone)
        {
            dBChangeCharacterPosition.Request(characterID, position, GetURL() + "users/" + email + "/characters/" + characterID + "/position", OnDone);
        }
        public void UpdateTwitter(string value)
        {
            if (value != "")
            {
                DbUserData.data.twitter = value;
                PlayerPrefs.SetString("twitter", value);
            }
        }
        public void UpdateDiscord(string value)
        {
            if (value != "")
            {
                DbUserData.data.discord = value;
                PlayerPrefs.SetString("discord", value);
            }
        }
        public string SocialUserName { get { return socialUserName; } }
        public string Email { get { return email; } }
        public void SetEmail(string email)
        {
            this.email = email;
            PlayerPrefs.SetString("email", email);
        }
        public string UrlRegister { get { return GetURL() + "users/"; } }
        public string UrlLoadMatches { get { return GetURL() + "users/" + DBManager.Instance.email + "/stats"; } }
        public string UrlLoadMatchesPerLevel { get { return GetURL() + "ranking/max_score/DESC"; } }
        public string UrlLoadUserData { get { return GetURL() + "users/" + DBManager.Instance.email; } }
        public string UrlSaveUserData { get { return GetURL() + "users/" + DBManager.Instance.email; } }
        public string UrlCoinsData { get { return GetURL() + "users/" + DBManager.Instance.email + "/coins/purchase/";  } }
        public string UrlCoinsHardData { get { return GetURL() + "users/" + DBManager.Instance.email + "/coins/purchaseHard/";  } }
        public string UrlEnergyData { get { return GetURL() + "users/" + DBManager.Instance.email + "/energy/purchase"; } }
        public string UrlEnergyPriceData { get { return GetURL() + "users/" + DBManager.Instance.email + "/energy/price"; } }
        public string UrlSellPlayerData(int characteriID) {  return GetURL() + "users/" + DBManager.Instance.email + "/characters/" + characteriID + "/sell";   }
        public string UrlBuyPlayerData { get { return GetURL() + "players/buy/" + DBManager.Instance.email; } }
        public string UrlUpdateCharacters { get { return GetURL() + "users/" + DBManager.Instance.email; } }
        public string UrlGamesPlayed { get { return GetURL() + "users/" + DBManager.Instance.email; } }
        public string URL { get { return GetURL(); } }

        public DBMatches DbMatches { get { return dbMatches; } }
        public DBMatchesPerLevel DbMatchesPerLevel { get { return dbMatchesPerLevel; } }
        public DBUserData DbUserData { get { return dBUserData; } }
        public DBGameData DbGameData{ get { return dBGameData; } }
        public DBCoins DbCoins { get { return dBCoins; } }
        public DBEnergy DbEnergy { get { return dBEnergy; } }
        public DBTier DbTier { get { return dBTier; } }
        public DBMundial DbMundial { get { return dBMundial; } }
        public DBAds DbAds { get { return dBAds; } }
        public DBTrack DbTrack{ get { return dBTrack; } }
        public DBPlayerPacks DbPlayerPacks { get { return dBPlayerPacks; } }

        public void Request(string url, string bodyJsonString, System.Action<bool, string> OnSuccess, string method, string readableUserFeedback = "Saving Data", Dictionary<string, string> headers = null)
        {
            Debug.Log("[Request method:" + method + "] url: " + url + " bodyJsonString: " + bodyJsonString + " readableUserFeedback: " + readableUserFeedback);            
            StartCoroutine(RequestC(url, bodyJsonString, OnSuccess, method, readableUserFeedback, headers));
        }
        IEnumerator RequestC(string url, string bodyJsonString, System.Action<bool, string> OnSuccess, string method, string readableUserFeedback = "Saving Data", Dictionary<string, string> headers = null)
        {
            var request = new UnityWebRequest(url, method);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            if(headers == null)
                request.SetRequestHeader( "Content-Type", "application/json");
            else
            {
                foreach (KeyValuePair<string, string> attachStat in headers)
                    request.SetRequestHeader(attachStat.Key, attachStat.Value);
            }

            yield return request.SendWebRequest();

            ResponseData responseData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            if(responseData != null)
                Debug.Log(request.downloadHandler.text);
            Debug.Log("[SendWebRequest method:" + method + "] url: " + url + " readableUserFeedback: " + readableUserFeedback + " result: " +  request.result);

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                if (responseData != null && responseData.message != "")
                    Events.OnPopup("ERROR: " + responseData.message, null);
                else
                    Events.OnPopup("ERROR: " + readableUserFeedback, null);
                if (OnSuccess != null) OnSuccess(false, "Connection Error");
            } else
            if (request.error != null)
            {
                if (OnSuccess != null)
                    OnSuccess(false, responseData.message);
                if(responseData != null && responseData.message != "")
                    Events.OnPopup("ERROR: " + responseData.message, null);
                else
                    Events.OnPopup("ERROR: " + readableUserFeedback, null);
            }
            else
            {
                if (responseData != null && responseData.welcomeBonus)
                {
                    print("welcomeBonus " + responseData);
                    OnSuccess(true, "welcomeBonus");
                }
                if (OnSuccess != null)
                    OnSuccess(true, request.downloadHandler.text);
                Debug.Log("Status Code: " + request.responseCode);
            }
        }
        [SerializeField] class ResponseData
        {
            public string message;
            public bool welcomeBonus;
        }
        public void LoadFromURL(string url, System.Action<string> OnSuccess)
        {
            StartCoroutine(GetData(url, OnSuccess));
        }
        IEnumerator GetData(string url, System.Action<string> OnSuccess)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                    OnSuccess(www.text);
                else
                    LoadFromURL(url, OnSuccess);
            }
        }

        public DateTime Now() {
            DateTime now = DateTime.UtcNow;
            if (versionMode == versionModes.PROD) 
                now = initDateTime.AddSeconds(Time.realtimeSinceStartup - startUpOffset);
            return now;
        }

    }
}
