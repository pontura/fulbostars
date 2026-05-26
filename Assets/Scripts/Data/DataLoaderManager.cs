using UnityEngine;
using System.Collections;
using Fulbo.Dashoard;
using Fulbo.UI;
using Fulbo.Mundial;

namespace Fulbo
{
    public class DataLoaderManager : MonoBehaviour
    {
        System.Action OnDone;
        int filesLoaded;

        bool assetsBundleRequested;
        public void Load(System.Action OnDone) {
            this.OnDone = OnDone;
            print("____________________LoadAssetBundles Loaded:" + assetsBundleRequested);
            if (assetsBundleRequested) return;
            assetsBundleRequested = true;
            Events.LoadAssetBundles(AssetsBundleLoaded);
        }

        void AssetsBundleLoaded(string result) {
            LoadSettings();
        }

        void LoadSettings()
        {
            Events.OnLoading("Settings");
            Data.Instance.settings.LoadData(SettingsLoaded);
            Data.Instance.settings.mainSettings.music_on = PlayerPrefs.GetInt("music", 1) == 1;
            Data.Instance.settings.mainSettings.announcer_on = PlayerPrefs.GetInt("announcer", 1) == 1;
            Data.Instance.GetComponent<Fulbo.Voices.VoicesOnScene>().SetMute(); //Checkear si hay que mutearlo
            Data.Instance.settings.mainSettings.speech_bubbles_on = PlayerPrefs.GetInt("bubbles", 1) == 1;
        }
        public void SettingsLoaded()
        {
            Events.OnLoading("My Team");
            Data.Instance.myTeam.LoadData(LoadCups);
        }
        //void LoadStoryModeData()
        //{
        //    Events.OnLoading("Story Mode");
        //    StoryModeData.Instance.OnLoad(LoadCups);
        //}
        void LoadCups()
        {
            Events.OnLoading("Load Cups");
            CupsData.Instance.OnLoad(LoadPositions);
        }
        //void LoadCupsLevels()
        //{
        //    Events.OnLoading("Load Cups Levels");
        //    CupsData.Instance.levels.OnLoad(LoadChests);
        //}
        //void LoadChests()
        //{
        //    Events.OnLoading("Load Cofres");
        //    ChestsData.Instance.OnLoad(LoadPositions);
        //}
        //void LoadMundialData()
        //{
        //    Events.OnLoading("Mundial Data");
        //    MundialData.Instance.OnLoad(LoadPositions);
        //}
        //void LoadFigus()
        //{
        //    Events.OnLoading("Figus Data");
        //    FigusData.Instance.LoadData(LoadPositions, "");
        //}
        void LoadPositions()
        {
            Events.OnLoading("Positions");
            Data.Instance.charactersPositions.LoadData(LoadCharacters);
        }

        void LoadCharacters() {
            Events.OnLoading("Characters Data");
            CharactersData.Instance.Init(InitVoices);
        }

        void InitVoices()
        {
            GetComponent<Fulbo.Voices.VoicesManager>().Init(LoadCharactersDefaultData);
        }
        public void LoadCharactersDefaultData()
        {
            CharactersData.Instance.LoadCharactersDefaultData(LoadTexts);
        }
        public void LoadTexts()
        {
            Events.OnLoading("Texts");
            Data.Instance.texts.Load(LoadDashboard);
        }
        void LoadDashboard()
        {
            Events.OnLoading("Dashboard");
            DashboardData.Instance.Init(LoadTextsData);
        }
        public void LoadTextsData()
        {
            Events.OnLoading("TextsData");
            Data.Instance.textsData.Init(LoadTournamentsTextsData);
        }  
         public void LoadTournamentsTextsData()
        {
            Events.OnLoading("TournamentsTextsData");
            Data.Instance.tournamentsData.Init(LoadLevelBonus);
        }        
        void LoadLevelBonus()
        {
            Events.OnLoading("LevelBonus");
            Data.Instance.levelBonusData.Init(LoadNotifications);
        }
        void LoadNotifications()
        {
          //  Events.OnLoading("LoadNotifications");
            OnAllLoaded();
        }
        void OnAllLoaded()
        {
            print("OnAllLoaded");
           
            Events.OnLoading("");
            if(OnDone != null)
                OnDone();
            OnDone = null;
        }
    }
}