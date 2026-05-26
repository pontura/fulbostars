using Fulbo.Dashoard;
using Fulbo.UI.Ads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Onboarding;
using Fulbo.Onboarding;
using Fulbo.DB;

namespace Fulbo.UI
{
    public class UIGameOverMenu : MonoBehaviour
    {
       // [SerializeField] DashboardUI dashboardUI;
        [SerializeField] AdsInSummary ads;
        [SerializeField] SummaryUI summary;
        [SerializeField] Animation anim;
        [SerializeField] GameObject guestModeRegisterCTA;
        [SerializeField] ButtonCustom replayButton;
        [SerializeField] ButtonCustom registerButton;
        [SerializeField] Text titleField;
        [SerializeField] Text subtitleField;
        [SerializeField] SummaryCharactersStatsUpgrade summaryCharactersStatsUpgrade;

        [SerializeField] GameObject winBG;
        [SerializeField] GameObject loseBG;

        bool isDone;
        bool scoreReady;
        public states state;
        public enum states
        {
            WIN,
            LOSE,
            WIN_CUP,
            LOST_CUP
        }

        void Start()
        {
            Time.timeScale = 1;
            if (Win())
                Init();
            else
                WatchInterstitial();
        }
        void WatchInterstitial()
        {
            Events.AdsWatchInterstitial((x) => {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["type"] = "matchLose";
                if (x)
                    Events.OnTrack("IterstitialAd", param);
                else
                    Events.OnTrack("InterstitialAdNotShown", param);
                Init();
            });
        }
        void Init()
        {
            if (Win())
            {
                winBG.SetActive(true);
                loseBG.SetActive(false);
                AudioManager.Instance.PlaySound("music", "music/music", true);
                AudioManager.Instance.ChangeVolume("music", 1);                
                Events.OnOutroSound();
            }
            else
            {
                WatchInterstitial();
                winBG.SetActive(false);
                loseBG.SetActive(true);
                AudioManager.Instance.PlaySound("music", "music/music_summary", true);
                AudioManager.Instance.ChangeVolume("music", 0);
                AudioManager.Instance.FadeVolume("music", 0.5f);
            }
            Data.Instance.ui.SetPauseButton(false);
            summaryCharactersStatsUpgrade.Hide();

           // replayButton.Init(1, OnButtonClicked, Data.Instance.texts.Get("play_again"));
          //  registerButton.Init(2, OnButtonClicked, Data.Instance.texts.Get("register"));
            titleField.text = Data.Instance.texts.Get("guestMode_title");
            subtitleField.text = Data.Instance.texts.Get("guestMode_subtitle");

            CheckForLastMatchOfCup();
        }

        public bool Win()
        {
            return Data.Instance.matchData.score.y > Data.Instance.matchData.score.x;
        }
        void CheckForLastMatchOfCup() // chequea si ganaste una copa o te quedaste sin vidas:
        {
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;

            bool win = Win();
            if (win)
                state = states.WIN;
            else
                state = states.LOSE;

            int cupID = Data.Instance.matchData.levelData.cupID;
            int tier = Data.Instance.matchData.levelData.tier;
            int levelID = Data.Instance.matchData.levelData.id;
            bool isLastMatch = CupsData.Instance.IsLastMatch(cupID, tier, levelID);
            bool isPlayingLastLife = cups.NoMoreLifes();

            print("win: " + win + " isLastMatch: " + isLastMatch + " isPlayingLastLife: " + isPlayingLastLife);

            if (win && isLastMatch) // si ganaste y es el ultimo partido de una copa:
            {
                state = states.WIN_CUP;
                CupsData.Instance.WonCup(cupID, tier);
                Data.Instance.onBoardingManager.CheckFirstCupWon();
                Events.ShowCupWinSignal(true, InitSummary);

                // muestra los shards de una copa ya ganada:
                MatchData.ResponseFromServer response = Data.Instance.matchData.response;
                if (response.shardsWon > 0 && !response.chestWon)
                {
                    DBGameData.Content gameData = DBManager.Instance.DbUserData.data.gameData;
                    int timesWon = gameData.cups.GetTimesWon(gameData.cups.GetActiveCupData().cupID, gameData.cups.GetActiveCupData().tier);
                  //  print("__________________timesWon: " + timesWon);
                    if (timesWon > 1) // solo hace la animacion si n oabre un chest:
                        Data.Instance.ui.OnAddShards(response.shardsWon);
                }

            }
            else if (!win && isPlayingLastLife && !Data.Instance.matchData.dataOnInit.hasWatchVideoForLife) // si ganaste y es el ultimo partido de una copa:
            {
                Events.ShowCupWinSignal(false, InitSummary);
                state = states.LOST_CUP;
            }
            else
                InitSummary();

            cupsDataSaved = false;
            CupsData.Instance.OnGameOver(CupsDataSaved);
        }
        bool cupsDataSaved;
        void CupsDataSaved(bool isOk, string error)
        {
            if (isOk)
            {
                Debug.Log("CupsDataSaved!");
                cupsDataSaved = true;
            }
            else
                CupsData.Instance.OnGameOver(CupsDataSaved);
        }
        void InitSummary()
        {
            if (!cupsDataSaved)
                Invoke("InitSummary", 0.5f);
            else
                summary.Init(SummaryReady, state);
        }
        
        public void SummaryReady()
        {
            Data.Instance.ui.CheckCupLifeLose();
            scoreReady = true;
            Debug.Log("SummaryReady");
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                Events.OnSkipOn(SummaryOff, "continue"); // skip all:
                return;
            }
            if (Data.Instance.energySystem.IsAFreeGame())
            {
                Events.OnSkipOn(GoOn, "continue");
                return;
            }

            if (Data.Instance.onBoardingManager.IsBoardingStep(OnBoardingManager.BoardingStepStates.FIRST_MATCH_PLAYED)) {
                Events.OnSkipOn(GoOn, "continue");
                return;
            }
            CheckIfDataWasSavedOk();
        }

        void CheckIfDataWasSavedOk() {
            if (Data.Instance.matchData.Rewards().AllReady())
            {
                switch (state)
                {
                    case UIGameOverMenu.states.WIN_CUP:
                    case UIGameOverMenu.states.LOST_CUP:
                        CupsData.Instance.EndCup();
                        break;
                }
                if (state == states.LOST_CUP)
                    Events.OnSkipOn(GoOn, "continue");
                else if (state == states.LOSE)
                    Events.OnSkipOn(GoOn, "retry");
                else
                    ShowMonewAds();

                Events.ScoreFreezed(false);//recive data del server:
            }
            else
                Invoke("CheckIfDataWasSavedOk", 0.25f);
        }

        void ShowMonewAds()
        {           
            ads.Init(this, 1, Data.Instance.matchData.Rewards().totalRewards);
        }
        void ShowXpAds()
        {
            ads.Init(this, 2, 0);
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }
       
        public void SummaryOff()
        {
            anim.Play("summaryOff");
            summary.Reset();
            if(Data.Instance.mode == Data.modes.PARTYMODE)
                GoOn();
            else if (Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.DAILY_REWARDS_OPENED)) {
                ShowXpAds();
                summaryCharactersStatsUpgrade.Init();
            } else
                GoOn();


          //  dashboardUI.gameObject.SetActive(false);   // dashboardUI.EnterFromParty();

            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                Events.OnSkipOn(GoOn, "continue");
               // Invoke("GuestRegisterCallToAction", 0.5f);
            }
            //else
            //    Invoke("Delayed", 0.5f);
        }

        void GuestRegisterCallToAction()
        {
            guestModeRegisterCTA.SetActive(true);
        }
        public void GotoMainMenu()
        {
            Events.OnSkipOff();
            Data.Instance.LoadLevel("MainMenu");
            Data.Instance.matchData.ResetAll();
        }
        bool energyPopupShown;
        public void GoOn()
        {
            Events.OnSkipOff();
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                 if(Data.Instance.hasTorneo)
                {
                   Data.Instance.OnSummaryOver();
                }
                else
                    Data.Instance.LoadLevel("Splash");
                return;
            }

            if (state == UIGameOverMenu.states.WIN || state == UIGameOverMenu.states.LOSE)// still on cup!
            {
                if (Data.Instance.energySystem.GetEnergyAvailable() <= 0)
                {
                    if (energyPopupShown)
                    {
                        Data.Instance.LoadLevel("MainMenu");
                        return;
                    }
                    Events.OpenOutOfEnergyPopup();
                    energyPopupShown = true;
                    Events.OnSkipOn(GoOn, "continue");
                    ads.Close();
                    return;
                }
            }
            else if (state == UIGameOverMenu.states.WIN_CUP || state == UIGameOverMenu.states.LOST_CUP)
                CupsData.Instance.EndCup();
            else
                CupsData.Instance.jumpAutomaticallyToTheMatch = true;

            if (isDone) return;
            isDone = true;
            AudioManager.Instance.PlaySound("shouts", "", false);
           
            Events.OnSkipOff();
           
                if(Data.Instance.onBoardingManager.IsTheFirstMatch()) // si es el primer partido sigue el onboarding:
                    Data.Instance.LoadLevel("MainMenu");
                else if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.DAILY_REWARDS_OPENED))
                    Data.Instance.LoadLevel("MainMenu");
                else if(state== states.WIN_CUP)
                    Data.Instance.LoadLevel("Levels");
                else if (state == states.WIN || state == states.LOSE) // continue!
                    Data.Instance.LoadLevel("Levels");
                else
                    Data.Instance.LoadLevel("MainMenu");

                Data.Instance.matchData.ResetAll();
        }
        //void OnButtonClicked(int id)
        //{
        //    if (id == 1) GotoReplay();
        //    else
        //    {
        //        GotoRegister();

        //        //Analytics
        //        Dictionary<string, object> param = new Dictionary<string, object>();
        //        Events.OnTrack("GuestModeRegisterClicked", param);
        //    }
        //}
        //void GotoReplay()
        //{
        //    Data.Instance.LoadLevel("Selector");
        //}
        //void GotoRegister()
        //{
        //    Data.Instance.LoadLevel("0_Register");
        //}
    }
}