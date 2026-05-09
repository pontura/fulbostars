using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fulbo.UI;
using Fulbo.UI.Shop;

namespace Fulbo.Onboarding
{
    public class OnBoardingManager : MonoBehaviour
    {
        [SerializeField] ButtonCustom button;
        public List<PanelData> allPanels;
        System.Action<bool> OnDone;

        [SerializeField] GameObject background;
        [SerializeField] RectTransform mask;
        [SerializeField] int team;
        [SerializeField] int mainmenu;
        [SerializeField] int levels;
        [SerializeField] int marketplace;
        [SerializeField] int account;

        static int boardingTotalSteps = 4;

        public enum BoardingStepStates {
            FIRST_TIME_GAME_LOADED = 0,
            GOT_FIRST_PLAYER_CARDS = 1,
            FIRST_MATCH_PLAYED = 2,
            SECOND_MATCH_PLAYED = 3,
            DAILY_REWARDS_OPENED = 4,
            FIRST_CUP_WON = 5,
            CUP_SELECTION_OPENED = 6
        }

        public states state;
        public enum states {
            OFF,
            PANEL,
            POPUP
        }

        public GameObject pointer_prefab;

        OnboardingPanel thisPanel;

        private bool isOpen;
        public bool IsOpen {
            get { return isOpen; }
            set { isOpen = value; }
        }

        [Serializable]
        public class PanelData
        {
            public OnboardingPanel panelAsset;
            public Fulbo.Onboarding.OnboardingPanel.panels panel;
            public int id;
        }
        private void Awake()
        {
            Events.OnboardingCheckStep += OnboardingCheckStep;
            Events.OnSceneLoaded += OnSceneLoaded;
        }
        void OnDestroy()
        {
            Events.OnboardingCheckStep -= OnboardingCheckStep;
            Events.OnSceneLoaded -= OnSceneLoaded;
        }
        void Start()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                OnDestroy();
                return;
            }
            button.gameObject.SetActive(false);
            background.SetActive(false);
            ResetStates();
            button.Init(0, OnOkClicked, "NEXT");
        }
        void OnOkClicked(int id)
        {
            Next();
        }
        
        string sceneName;
        void OnSceneLoaded(string sceneName)
        {
            if (DB.DBManager.Instance.DbUserData.data == null || DB.DBManager.Instance.DbUserData.data.gameData == null)
                return;
            ResetStates();
            this.sceneName = sceneName;
            Invoke("Delayed", 0.2f);
        }
        void Delayed()
        {
            if (sceneName == "GameIntro")
            {
                if (!IsBoardingStepDone(BoardingStepStates.FIRST_MATCH_PLAYED)) // jugado el primer partido
                {
                    OnBoardingPanel(OnboardingPanel.panels.gameintro, 1, null);
                }
            }
            else if (sceneName == "MainMenu")
            {
                if (IsBoardingStep(BoardingStepStates.FIRST_MATCH_PLAYED) && !DB.DBManager.Instance.DbUserData.data.gameData.firstGalacticCupSign) // jugado el primer partido
                {
                    OnBoardingPanel(OnboardingPanel.panels.storymode, 1, MainMenuReady, "CupsButton");
                    DB.DBManager.Instance.DbGameData.Put("galacticCupSign", "true", null);
                    Events.OnTrack("TutorialFirstMatchIntro", null);
                } else if (IsBoardingStep(BoardingStepStates.SECOND_MATCH_PLAYED)) {
                    Events.OnTrack("TutorialDailyRewards", null);
                    OnBoardingPanel(OnboardingPanel.panels.storymode, 2, MainMenuReady, "DailyNotifications");
                } else {
                    if (DB.DBManager.Instance.DbUserData.data.gameData.cups.hasLoseLife && !DB.DBManager.Instance.DbUserData.data.gameData.knowTraining) {
                        if (!Data.Instance.ui.IsShopOn()) {
                            OnBoardingPanel(OnboardingPanel.panels.training, 0, null, "trainingBtn", true,1f);
                            DB.DBManager.Instance.DbGameData.Put("training", "true", null);
                        }
                    }
                }
            }
        }        

        public void CheckShopVisited() {
            if (!DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedShop) { // primera visita a Shop intencionalmente
                OnBoardingPanel(OnboardingPanel.panels.shop, 0, null, " ");
                Events.OnTrack("FirstShopVisit", null);
                DB.DBManager.Instance.DbGameData.Put("shop", "true", null);
            }
        }

        public void CheckStatUpgradable() {
            if (!DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedUpgradableStat) { // primera visita a myTeam
                ResetStates();
                OnBoardingPanel(OnboardingPanel.panels.myteam, 1, null, "UpgradeButton");
                Events.OnTrack("FirstUpgradableStat", null);
                DB.DBManager.Instance.DbGameData.Put("upgradableStat", "true", null);
            }
        }

        public void CheckMyTeamVisited() {
            if (!DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedMyTeam) { // primera visita a myTeam
                OnBoardingPanel(OnboardingPanel.panels.myteam, 0, null, " ");
                Events.OnTrack("FirstMyTeamVisit", null);
                DB.DBManager.Instance.DbGameData.Put("myTeam", "true", null);
            }
        }

        public void CheckFirstCupWin() {
            if (IsBoardingStep(BoardingStepStates.FIRST_CUP_WON)) // primera copa ganada
                    {
                OnBoardingPanel(OnboardingPanel.panels.cups, 1, null, "cupsContent");
                Events.OnTrack("TutorialCupSelection", null);
                DB.DBManager.Instance.DbGameData.Put("tutorialStep", "" + (int)BoardingStepStates.CUP_SELECTION_OPENED, null);
            }
        }

        public void CheckLoseCup(string goName) {
            if (DB.DBManager.Instance.DbUserData.data.gameData.cups.hasLoseCup && !DB.DBManager.Instance.DbUserData.data.gameData.knowCupsReplay) {
                OnBoardingPanel(OnboardingPanel.panels.cups, 3, null, goName, true);
                DB.DBManager.Instance.DbGameData.Put("cup_replay", "true", null);
            }
        }

        public void ShowLevelsOnFirstMatchPlayed(Action<bool> onDone) {
            OnBoardingPanel(OnboardingPanel.panels.levels, 0, onDone, "levelsContent");
            DB.DBManager.Instance.DbGameData.Put("levelsFirstMatchPlayedSign", "true", null);
        }
        
        void MainMenuReady(bool isOn = false)
        {  
            if (IsBoardingStep(BoardingStepStates.FIRST_MATCH_PLAYED))
            {
                //Events.OnBoardingPanelAction("mainmenu", 1);
                Events.OnTrack("TutorialStoryModeStart", null);
            }
            else if (IsBoardingStep(BoardingStepStates.SECOND_MATCH_PLAYED))
            {
                DB.DBManager.Instance.DbGameData.Put("tutorialStep", "" + (int)BoardingStepStates.DAILY_REWARDS_OPENED, null);
                Events.CheckForDailyRewards(true, OnFirstDailyRewardsExit);
            }
        }

        void OnFirstDailyRewardsExit() {
            //OnBoardingPanel(OnboardingPanel.panels.levels, 1, (x)=> Events.OnBoardingPanelAction("mainmenu",3), "CupsButton");            
            OnBoardingPanel(OnboardingPanel.panels.levels, 1, null, "CupsButton");
        }
        
        void LevelSelectorReady(bool isOn = false)
        {
            Events.OnTrack("TutorialLevelsPlay", null);
            Events.OnBoardingPanelAction("levels", 1);
        }
        public bool IsTheFirstMatch() // si no jugó el onboarding:
        {
            if (!IsBoardingStepDone(BoardingStepStates.FIRST_MATCH_PLAYED)) return true;
            return false;
        }
        bool textLoaded;
        void OnboardingCheckStep(Fulbo.Onboarding.OnboardingPanel.panels panel, int id, System.Action<bool> OnReady)
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return;
            Debug.Log("OnboardingCheckStep " + panel + " id: " + id);
            switch (panel)
            {
                case OnboardingPanel.panels.intro: // when editing team name:
                    OnBoardingPanel(OnboardingPanel.panels.intro, id, OnReady);
                    break;
                //case OnboardingPanel.panels.marketplace: // when selecting a player to buy:
                //    if (marketplacesDone) return;
                //    marketplace = 4;
                //    OnBoardingPanel(OnboardingPanel.panels.marketplace, 4, MarketplaceReady);
                //    break;
                case OnboardingPanel.panels.storymode: // when editing team name:
                    if (IsBoardingStepDone(BoardingStepStates.FIRST_MATCH_PLAYED)) return;
                    mainmenu = 0;
                    OnBoardingPanel(OnboardingPanel.panels.storymode, 1, MainMenuReady);
                    break;
                /*case OnboardingPanel.panels.account: // when editing team name:
                    if (GetStep() > 5) return;
                    OnBoardingPanel(OnboardingPanel.panels.account, 0, AccountReady);
                    break;*/
            }
        }
       
        void AccountReady(bool done) { 
            account++;
        }

        void MarketplaceReady() { MarketplaceReady(false); }
        void MarketplaceReady(bool isOn = false)
        {
            marketplace++;
            if (marketplace == 1)
            {
                OnBoardingPanel(OnboardingPanel.panels.marketplace, 1, MarketplaceReady);
            }
            else if (marketplace == 2)
            {
                Events.OnBoardingPanelAction("marketplace", 2);
                OnPopup(Data.Instance.texts.Get("onboarding_marketplace2"), MarketplaceReady);
            }
            if (marketplace == 3)
                OnBoardingPanel(OnboardingPanel.panels.marketplace, 3, MarketplaceReady);
            if (marketplace ==4)
            {
                marketplace++;
            }
        }


       
        System.Action OnPopupDone;
        void OnPopup(string text, System.Action _OnPopupDone)
        {
            this.OnPopupDone = _OnPopupDone;
            state = states.POPUP;
            Events.OnPopup(text, OnPopupDoneCallback);
        }
        void OnPopupDoneCallback()
        {
            if (OnPopupDone != null)
            {
                ResetStates();
                OnPopupDone();
                OnPopupDone = null;
            }
        }
        string mainObjectName;
        Transform parentForMainObject;
        GameObject mainObject;
        Vector2 maskSize;
        int sibilingIndex;
        void OnBoardingPanel(OnboardingPanel.panels panel, int id, System.Action<bool> OnDone, string mainObjectName = "", bool useMask = false, float maskDelay = -1f, Vector2 _maskSize = new Vector2())
        {
            if (!isOpen) {
                AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_popup_alien");
                this.mainObjectName = mainObjectName;

                state = states.PANEL;
                this.OnDone = OnDone;
                thisPanel = GetPanel(panel, id);

                thisPanel.Init(this);

                mainObject = null;
                parentForMainObject = null;
                print("OnBoardingPanel: mainObjectName " + mainObjectName);
                if (mainObjectName != "") {                    
                    mainObject = GameObject.Find(mainObjectName);
                    if (mainObject != null) {
                        if (useMask) {
                            maskSize = _maskSize;
                            if (maskDelay < 0f)
                                SetMask();
                            else
                                Invoke("SetMask", maskDelay);
                        } else {
                            sibilingIndex = mainObject.transform.GetSiblingIndex();
                            parentForMainObject = mainObject.transform.parent;
                            mainObject.transform.SetParent(background.transform);
                        }
                    }
                    if (thisPanel.showBackground)
                        background.SetActive(true);
                }
                if (thisPanel.showBackground)
                    button.gameObject.SetActive(true);
            }
        }

        void SetMask() {
            RectTransform rt = mainObject.GetComponent<RectTransform>();
            mask.gameObject.SetActive(true);
            if (maskSize.Equals(Vector2.zero))
                mask.sizeDelta = rt.sizeDelta * 1.1f;
            else
                mask.sizeDelta = maskSize;
            mask.position = rt.TransformPoint(rt.rect.center);
        }

        OnboardingPanel GetPanel(OnboardingPanel.panels panel, int id)
        {
            foreach (PanelData pData in allPanels)
                if (pData.panel == panel && pData.id == id)
                    return pData.panelAsset;
            return null;
        }
        public void OnSkipButtonPress()
        {
            Next();
        }
        public void Next()
        {
            Reset();
            if (OnDone != null)
                OnDone(true);
            OnDone = null;
        }
        public void Skip()
        {
        }
        private void Reset()
        {
            if(mainObject != null && parentForMainObject != null)
            {
                mainObject.transform.SetParent(parentForMainObject);
                mainObject.transform.SetSiblingIndex(sibilingIndex);
            }
            background.SetActive(false);
            button.gameObject.SetActive(false);
            mask.gameObject.SetActive(false);
            if (state == states.PANEL) thisPanel.Close();
            if (state == states.POPUP) Events.OnPopupForceSkip();
            ResetStates();
        }
        void ResetStates()
        {
            state = states.OFF;
            foreach (PanelData pData in allPanels)
                pData.panelAsset.Close();
        }

        public void CheckFirstCupWon() {
            if (!IsBoardingStepDone(BoardingStepStates.FIRST_CUP_WON))
                DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep = (int)BoardingStepStates.FIRST_CUP_WON;
        }

        public void CheckOnboardingGamesDone() {
            if (IsBoardingStep(BoardingStepStates.GOT_FIRST_PLAYER_CARDS)) {
                DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep = (int)BoardingStepStates.FIRST_MATCH_PLAYED;
            } else if (IsBoardingStep(BoardingStepStates.FIRST_MATCH_PLAYED)) {
                DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep = (int)BoardingStepStates.SECOND_MATCH_PLAYED;
            }
    }

    bool firstLifeLose;
    public void SetFirsTimeLose() {
            firstLifeLose = true;
    }
    public bool FirstLifeLose(System.Action<bool> callback) {
            if (firstLifeLose) {
                firstLifeLose = false;
                OnBoardingPanel(OnboardingPanel.panels.cups, 0, callback, "lifes");
                Events.OnTrack("TutorialFirstLifeLose", null);
                return true;
            }
            return firstLifeLose;
            
    }

    public void ShowStatHints() {
        if (DB.DBManager.Instance.DbUserData.data.gameData.cups.hasLoseLife && !DB.DBManager.Instance.DbUserData.data.gameData.knowStatHints) { 
                OnBoardingPanel(OnboardingPanel.panels.myteam, 2, null, "statsContainer", true, 1f);
                DB.DBManager.Instance.DbGameData.Put("statHints", "true", null);
        }
    }

    bool tierUnlockedSignShowed;
    public void SecondTierUnlocked() {
            if (!tierUnlockedSignShowed) {
                tierUnlockedSignShowed = true;
                OnBoardingPanel(OnboardingPanel.panels.cups, 2, null, "tiersContainer");
                DB.DBManager.Instance.DbGameData.Put("tierUnlocked", "true", null);
            }
    }

        public void CheckHasReplayCups(int cupID, int tier) {
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;
            if (cups.GetTimesWon(cupID, tier) > 0) {
                if (!DB.DBManager.Instance.DbUserData.data.gameData.cups.hasReplayCups) {
                    OnBoardingPanel(OnboardingPanel.panels.levels, 2, null, "Reward");
                    DB.DBManager.Instance.DbGameData.Put("replayCups", "true", null);
                }
            }
        } 
        
        public void OpenShards()
        {
            print("OpenShards");
            OnBoardingPanel(OnboardingPanel.panels.shards, 0, null, "ShardsWidget", true,1f, new Vector2(250,100));
        }  
        public bool IsTutorialDone()
        {
            return IsBoardingStepDone(BoardingStepStates.SECOND_MATCH_PLAYED);
        }

        public void BoardingNextScene() {
            if (IsBoardingStep(BoardingStepStates.FIRST_TIME_GAME_LOADED))
                Data.Instance.ui.figusScreen.Init(0);
            //Data.Instance.LoadLevel("Figus");
            else if (IsBoardingStep(BoardingStepStates.GOT_FIRST_PLAYER_CARDS)) {
                SetTutorialFirstMatch();
                Data.Instance.LoadLevel("GameIntro");
            } else
                Data.Instance.LoadLevel("MainMenu");
        }

        void SetTutorialFirstMatch() {
            int cupID = 10;
            int levelID = 10;
            int tier = 1;

            LevelData levelData = CupsData.Instance.GetLevelData(cupID, tier, levelID);

            if (DB.DBManager.Instance.DbUserData.data.gameData.cups == null) {
                DB.DBManager.Instance.DbUserData.data.gameData.cups = new DB.DBCupsData();
                DB.DBManager.Instance.DbUserData.data.gameData.cups.activeCup = cupID;
                DB.DBManager.Instance.DbUserData.data.gameData.cups.Init();
            }
            CupsData.Instance.InitCup(cupID, tier);
            Data.Instance.matchData.InitLevel(levelData);
        }

        public bool IsBoardingStep(BoardingStepStates state) {
            return DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep == (int)state;
        }

        public bool IsBoardingStepDone(BoardingStepStates state) {
            return DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep >= (int)state;
        }

        static public bool IsBoardingComplete() {
            return DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep >= boardingTotalSteps;
        }
        static public bool SecondMatchPlayed()
        {
            return DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep > 3;
        }

    }
}
