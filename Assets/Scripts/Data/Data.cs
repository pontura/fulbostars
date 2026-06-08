using UnityEngine;
using UnityEngine.SceneManagement;
using Fulbo.Game;
using Fulbo.UI;
using Fulbo.Onboarding;
using Fulbo.Energy;
using Fulbo.UI.Pvp;
using Fulbo.Tournamets;
using Fulbo.Stadiums;
using Fulbo.Fixture;
using UnityEngine.PlayerLoop;

namespace Fulbo
{
    public class Data : MonoBehaviour
    {
        public bool hasTorneo = true;
        static Data mInstance = null;
        string storeURL = "https://play.google.com/store/apps/details?id=com.aconcaguagames.FulboGalaxy";
        string purchaseHard = "com.aconcaguagames.fulbogalaxy.fulbo";

        string rankings_url = "https://ranking.fulbogalaxy.com/";
        string url_referrer = "https://fulbogalaxy.com/referrer.html";

        string url_PROD = "https://play.fulbogalaxy.com/";
        string url_DEV = "https://play-testing.fulbogalaxy.com/";

        string assetsBundlefolder = "AssetBundles";

        public string GetAssetsBundleFolder() {   return assetsBundlefolder;  }
        public string GetPurchaseURL() {   return purchaseHard;   }
        public string GetStoreURL() {   return storeURL;   }

        public string GetURL()
        {
//#if UNITY_WEBGL
//            return ""; // RUTA RELATIVA:
//#else
            if (DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.DEV)
                return url_DEV;
            return url_PROD;
            //#endif
        }
        public string GetURLForReferrerLink() { return url_referrer; }
        public string GetURLRankings()  {return rankings_url; }

        public string newScene;

        public bool isMobile;

        public UICanvas ui;

        [HideInInspector] public TournamentsData tournamentsData;
        [HideInInspector] public MatchData matchData;
        [HideInInspector] public Settings settings;
        [HideInInspector] public CharactersPositions charactersPositions;
        [HideInInspector] public UI.Texts texts;
        [HideInInspector] public TextsData textsData;
        [HideInInspector] public LevelBonusData levelBonusData;
        [HideInInspector] public SheetLoader sheetLoader;
        [HideInInspector] public MyTeam myTeam;
        [HideInInspector] public MyFigus myFigus;
        [HideInInspector] public ClubsData clubsData;
        [HideInInspector] public LangsManager langsManager;
        [HideInInspector] public PartyModeData partyModeData;
        [HideInInspector] public MarketplaceData marketplaceData;
        [HideInInspector] public PopupManager popupManager; 
        [HideInInspector] public EnergySystem energySystem; 
        [HideInInspector] public PvpData pvpData; 
        [HideInInspector] public PoolObjects pool; 
        [HideInInspector] public PricesData pricesData;
        [HideInInspector] public WebGLGamepadFix webGLGamepadFix;
        [HideInInspector] public FixtureManager fixtureManager;
#if UNITY_ANDROID
        [HideInInspector] public ReviewRequest reviewRequest;   
#endif
        public Fulbo.Input.FulboInputs fulboInputs;

        DataLoaderManager dataLoaderManager;

        public loadTypes loadType;
        public enum loadTypes
        {
            SERVER,
            DATABASE,
            LOCAL
        }

        public modes mode;
        public enum modes
        {
            STORYMODE,
            PARTYMODE,
            PVP
        }
        public int shortcut_upgrade_playerID;

        [SerializeField] public OnBoardingManager onBoardingManager;

        public static Data Instance
        {
            get
            {
                return mInstance;
            }
        }
        public void LoadLevel(string aLevelName)
        {
            Debug.Log("DATA LoadLevel " + aLevelName);
            this.newScene = aLevelName;
            switch(aLevelName)
            {
                case "GameOver": OnFadeDone(); return;
            }
            Events.OnFade(true, OnFadeDone);
        }
        void OnFadeDone()
        {
            SceneManager.LoadScene(newScene);
            Events.OnSceneLoaded(newScene);
        }
        void DestroySelf()
        {
            if (Application.isPlaying)
                Destroy(this.gameObject);
            else
                DestroyImmediate(this.gameObject);
        }
        void Awake()
        {
            if (this.GetType() != typeof(Data))
                DestroySelf();

            if (mInstance == null)
                mInstance = this as Data;
            else if (mInstance != this)
            {
                DestroySelf();
                return;
            }
            fulboInputs = new Fulbo.Input.FulboInputs();
            DontDestroyOnLoad(this);
            // PlayerPrefs.DeleteAll();
#if UNITY_WEBGL
        // if (!isMobile)
        // Cursor.visible = false; 
        isMobile = true;
        //  if (!settings.mainSettings.isArcade && !isMobile)
        //  Screen.fullScreen = true;
        loadType = loadTypes.SERVER;
#elif UNITY_EDITOR

#elif UNITY_STANDALONE
            Cursor.visible = false; 
        loadType = loadTypes.LOCAL;
       // Screen.fullScreen = true;
#elif UNITY_ANDROID || UNITY_IOS
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        isMobile = true;
        loadType = loadTypes.SERVER;
#endif
            webGLGamepadFix = GetComponent<WebGLGamepadFix>();
            tournamentsData = GetComponent<TournamentsData>();
            dataLoaderManager = GetComponent<DataLoaderManager>();
            langsManager = GetComponent<LangsManager>();
            matchData = GetComponent<MatchData>();
            sheetLoader = GetComponent<SheetLoader>();
            charactersPositions = GetComponent<CharactersPositions>();
            clubsData = GetComponent<ClubsData>();
            levelBonusData = GetComponent<LevelBonusData>();
            partyModeData = GetComponent<PartyModeData>();
            marketplaceData = GetComponent<MarketplaceData>();
            popupManager = GetComponentInChildren<PopupManager>();
            energySystem = new EnergySystem();
            pvpData = GetComponent<PvpData>();
            pool = GetComponent<PoolObjects>();
            pricesData = GetComponent<PricesData>();
            fixtureManager = GetComponent<FixtureManager>();
    #if UNITY_ANDROID
            reviewRequest = GetComponent<ReviewRequest>();
#endif

            if (DB.DBManager.Instance != null && DB.DBManager.Instance.DbUserData.type == DB.DBUserData.types.GUEST)
            {
                mode = modes.PARTYMODE;
                settings.mainSettings.isArcade = false; // not multiplayer:
            }
            ForceMode(mode);
        }
        void Start()
        {
            langsManager.Init();
            dataLoaderManager = GetComponent<DataLoaderManager>();
            dataLoaderManager.Load(LoadReady);
        }
        public void InitTournament()
        {
            Data.Instance.tournamentsData.SetTournament(true);
            StadiumsData.Instance.SetRandomStadium();
            Data.Instance.matchData.SetTotalPlayers(8, 8);
            CupsData.Instance.levels.InitTournament();
            LoadLevel("TournamentSelector");
        }
        void LoadReady()
        {
           

            allLoaded = true;
            ui.Init();

            if (mode == modes.PARTYMODE)
            {
                if(Data.Instance.settings.mainSettings.isArcade)
                    Data.Instance.LoadLevel("Splash");
                else
                   InitAll();
            }
            else if (Fulbo.Game.GameManager.Instance != null)
                Fulbo.Game.GameManager.Instance.OnInit();
            else
            {
                Debug.Log("LoadReady");
                Debug.Log("state: " + DB.DBManager.Instance.DbUserData.state);
                //if(DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep == 0)
                if (DB.DBManager.Instance.DbUserData.state == DB.DBUserData.userStates.FIRST_TIME)
                {
                    Debug.Log("referrerID");
                    string referrerID = DB.DBManager.Instance.GetComponent<InstallReferrer>().GetReferrer();
                    Debug.Log("referrerID " + referrerID);
                    int number;
                    if (referrerID != null && referrerID != "" && int.TryParse(referrerID, out number))
                    {
                        Events.OnPopup(Data.Instance.texts.Get("intro_referred"), OnFirstTimeReady);
                        Debug.Log("referrerID done" + referrerID);
                    }
                    else
                        OnFirstTimeReady();
                }
                else
                {
                    Events.OnLoading("AllDone");
                    Invoke("Delayed", 1);
                }
            }
        }
        void Delayed()
        {
            Data.Instance.LoadLevel("MainMenu");
        }
        void OnFirstTimeReady()
        {
            AudioManager.Instance.PlaySound("music", "music/music", true);
            AudioManager.Instance.FadeVolume("music", 0.3f);
            Data.Instance.onBoardingManager.BoardingNextScene();
        }
        bool allLoaded;
        public bool AllLoaded()
        {
            return allLoaded;
        }
        public void ForceMode(modes _mode)
        {
            this.mode = _mode;
            switch(mode)
            {
                case modes.PARTYMODE:
                    matchData.SetTotalPlayers(8, 8);
#if UNITY_STANDALONE
                    Data.Instance.loadType = Data.loadTypes.LOCAL;
                    if (settings.mainSettings.isArcade)
                        Data.Instance.settings.totalPlayersAvailable = 4;
#elif UNITY_ANDROID || UNITY_IOS
                    Data.Instance.loadType = Data.loadTypes.SERVER;
                    Data.Instance.settings.totalPlayersAvailable = 1;
#endif
                    onBoardingManager.gameObject.SetActive(false);
                    break;
                case modes.STORYMODE:
                    onBoardingManager.gameObject.SetActive(true);
                    break;
            }
        }
        public void OnSummaryOver()
        {
            print("OnSummaryOver");
            if(Data.Instance.settings.mainSettings.isArcade)
                Data.Instance.LoadLevel("Splash");
            else if(Data.Instance.tournamentsData.IsTournament() && Data.Instance.tournamentsData.lastMatchGoles1 != Data.Instance.tournamentsData.lastMatchGoles2)                    
                Data.Instance.LoadLevel("Prensa");
            else
            {
                Reset();
                Data.Instance.LoadLevel("SplashOptions");
            }
        }
        void Reset()
        {            
            Data.Instance.matchData.Reset();
            Data.Instance.tournamentsData.SetTournament(false);
        }
        public void Back()
        {   
            Reset();
            if(Data.Instance.settings.mainSettings.isArcade)
                Data.Instance.LoadLevel("Splash");
            else
                Data.Instance.LoadLevel("SplashOptions");
        }
        public void InitAll()//cuando termina el intro
        {
            Back();
            //InitTournament();
        }
    }

}