using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Dashoard;
using Fulbo.Stadiums;

namespace Fulbo.UI
{

    public class MainMenu : Fulbo.UI.UIMainScreen
    {
        [SerializeField] GameObject panel;
        MainMenuButtons mainMenuButtons;

        void Start()
        {
            Events.CheckForImportantNotifications("mainmenu");
            AssetsBundle.AssetsBundleManager.Instance.InstantiateAssets();
#if UNITY_MOBILE
            controls.SetActive(false);
#endif
            Data.Instance.mode = Data.modes.STORYMODE;
            mainMenuButtons = GetComponent<MainMenuButtons>();
            mainMenuButtons.Init();
            //Data.Instance.matchData.ResetOponents();
            Events.OnSkipOff();
            Data.Instance.ui.SetBackButton(false);
            Data.Instance.ui.SetScore(true);
            Data.Instance.ui.SetLifes(false);

            AudioManager.Instance.PlaySound("music", "music/music_summary", true);
            //AudioManager.Instance.ChangeVolume("music", 0);
            AudioManager.Instance.FadeVolume("music", 0.35f,0.02f);

            AudioManager.Instance.PlaySound("ambience", "", true);

            if(Data.Instance.shortcut_upgrade_playerID > 0)
            {
                mainMenuButtons.GotoMyTeam(Data.Instance.shortcut_upgrade_playerID);
                Data.Instance.shortcut_upgrade_playerID = 0;
            }
            //if (DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep<2) // onboarding:
            //    mainMenuButtons.GotoEditor();
            //else
                panel.SetActive(true);

        }
        private void OnDisable()
        {
            //Reset();
        }
        
        public void GotoSelector()
        {
            Data.Instance.LoadLevel("TeamSelector");
            Reset();
        }
        public void Controls()
        {
            Data.Instance.LoadLevel("ControlRemapping");
            ResetDashBoard();
        }
        public void gotoParsec()
        {
            Data.Instance.LoadLevel("Parsec");
            Reset();
        }
        public void Show()
        {
            panel.SetActive(true);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_transicion4");
        }
        public void Reset()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_transicion3");
            Data.Instance.matchData.ResetAll();
            ResetDashBoard();
            Invoke("ResetPanel", 0.4f);
        }
        void ResetPanel()
        {
            panel.SetActive(false);
        }
        public void OnNotifications()
        {
            DashboardData.Instance.ShowDashboard(true);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_noti");
        }
        public void ResetDashBoard()
        {
            if (DashboardData.Instance != null)
                DashboardData.Instance.ShowDashboard(false);
        }
        public void Rankings()
        {
            Application.OpenURL(Data.Instance.GetURLRankings());
        }

        public void WatchMatch() //DEMO saved match:
        {
            GameRecorder.Manager.Instance().InitPlaying();
            GameRecorder.ParsedMatchSettings settings = GameRecorder.Manager.Instance().timeLine.parsedSettings;
            InitGameByParsedSettings(settings);
            Data.Instance.LoadLevel("GameIntro");
            ResetDashBoard();
        }
        void InitGameByParsedSettings(GameRecorder.ParsedMatchSettings settings)
        {
            //Data.Instance.matchData.SetActualStadium(settings.stadiumID);
            //Data.Instance.matchData.SetActualLevel(settings.levelID);

            //LevelData levelData = CupsData.Instance.GetActualLevel();
            //StadiumsData.Instance.SetActiveStadium(levelData.stadium_id, levelData.size);

            //Data.Instance.matchData.SetTeams(settings.team1, settings.team2);
            //Data.Instance.matchData.totalTime = settings.duration;            
        }




        public void GotoPartyMode()
        {
            Data.Instance.mode = Data.modes.PARTYMODE;
            AudioManager.Instance.PlaySoundOneShot("ui", "_new/ui/clickPlay");

            if (Data.Instance.settings.mainSettings.isArcade) // multiplayer:
                Data.Instance.LoadLevel("PlayersTeamSelector");
            else
                Data.Instance.LoadLevel("TeamSelector"); // Ruleta

            Reset();
            GameRecorder.ParsedMatchSettings settings = new GameRecorder.ParsedMatchSettings();
            settings.levelID = 1;
            settings.stadiumID = 6;
            settings.referee = 1;
            settings.duration = 180;
            settings.team1 = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            settings.team2 = new List<int>() { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            InitGameByParsedSettings(settings);
        }

    }
}