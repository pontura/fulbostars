using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Dashoard;
using Fulbo.Stadiums;
using Fulbo.UI.Mundial;
using Fulbo.UI.Pvp;
using Fulbo.Onboarding;

namespace Fulbo.UI
{

    public class MainMenuButtons : CascadeList
    {
        [SerializeField] EditTeam.ClubShield clubShield;
        [SerializeField] Image[] myTeamThumbs;
        [SerializeField] ButtonCascade[] buttons;
        [SerializeField] CupProgress cupProgress;

        [SerializeField] ButtonCustom tranfsersButton;
        [SerializeField] ButtonCustom notificationButton;
        [SerializeField] ButtonCustom dailyButton;
        [SerializeField] GameObject notificationsIcon;

        [SerializeField] Animation mainMenuAnim;
        [SerializeField] Animation bgAnim;

        [SerializeField] Marketplace.MarketplaceUI marketplace;
        [SerializeField] EditTeam.EditTeamScreen editTeamScreen;
        [SerializeField] MyTeamUI myTeam;
       // [SerializeField] GalaxyMapUI galaxyMap;
        [SerializeField] PvpMainScreen pvpScreen;

        [SerializeField] ButtonCustom referrerLinkButton;

        [SerializeField] ButtonCustom mundialBtn;
        [SerializeField] ButtonCustom pvpButton;
      //  [SerializeField] ButtonCustom storyModeBtn;

      //  [SerializeField] MundialRankingGlobal mundialRankingGlobal;
        bool mainButtonPressed;

        MainMenu mainMenu;

        public void Init()
        {
            mainMenu = GetComponent<MainMenu>();
            editTeamScreen.gameObject.SetActive(false);
            marketplace.gameObject.SetActive(false);
            myTeam.gameObject.SetActive(false);
          //  galaxyMap.gameObject.SetActive(false);
            pvpScreen.gameObject.SetActive(false);
          //  mundialRankingGlobal.gameObject.SetActive(false);

            if (Data.Instance.myTeam.clubData != null)
                clubShield.Init(Data.Instance.myTeam.clubData);

            if (DB.DBManager.Instance.DbUserData.data.players_characters != null && DB.DBManager.Instance.DbUserData.data.players_characters.Count > 0)
            {
                for (int a = 0; a < myTeamThumbs.Length; a++)
                {
                    int characterID = DB.DBManager.Instance.DbUserData.data.players_characters[a].player_id;
                    Sprite thumb = CharactersData.Instance.GetCharacterData(characterID, false).thumb;
                    if (thumb != null)
                        myTeamThumbs[a].sprite = thumb;
                }
            }

            InitCascade();
            foreach (ButtonCascade bc in buttons)
                AddToCascade(bc);
            StartCascade();

            referrerLinkButton.Init(0, ButtonClicked, Data.Instance.texts.Get("referrer"));
            mundialBtn.Init(1, ButtonClicked, Data.Instance.texts.Get("mundial_mainmenu_title"));
          //  storyModeBtn.Init(2, ButtonClicked, Data.Instance.texts.Get("story_mode_title"));
            pvpButton.Init(3, ButtonClicked, Data.Instance.texts.Get("pvp_button_title"));
            notificationButton.Init(6, ButtonClicked);
            tranfsersButton.Init(5, ButtonClicked);
            dailyButton.Init(4, ButtonClicked);

            CheckForDailyRewards();

            if(cupProgress != null) cupProgress.Init();

            Events.OnBoardingPanelAction += OnBoardingPanelAction;
            Events.Back += Back;

            if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.SECOND_MATCH_PLAYED))
                dailyButton.gameObject.SetActive(false);
        }
        void ButtonClicked(int id)
        {
            print("ButtonClicked " + id);
            switch(id)
            {
                case 0:
                    Events.OpenShareApp();
                    break;
                //case 1:
                //    GotoMundial();
                //    break;
                //case 2:
                //    GotoStoryMode();
                //    break;
                case 3:
                    GotoPvp();
                    break;
                case 4:
                    Events.OpenShop(Shop.Shop.sectionType.DAILY_REWARDS);
                    break;
                case 5:
                    GotoMarketplace();
                    break;
                case 6:
                    Events.OpenNotifications();
                    break;
            }
        }
        private void OnDestroy()
        {
            Events.OnBoardingPanelAction -= OnBoardingPanelAction;
            Events.Back -= Back;
        }
        void Back()
        {
            mainButtonPressed = false;
            mainMenu.Show();
            mainMenuAnim.Play("in");
            bgAnim.Play("down");

            Data.Instance.ui.SetBackButton(false);
            if (Data.Instance.myTeam.clubData != null)
                clubShield.Init(Data.Instance.myTeam.clubData);
            CheckForDailyRewards();
        }
        void CheckForDailyRewards()
        {
            Events.CheckForDailyRewards(false, null);
        }
        void OnBoardingPanelAction(string onboardingName, int id)
        {
            switch (onboardingName)
            {
                case "mainmenu":
                    if (id == 1)
                    {
                        //    GotoStoryMode();
                        //else if (id == 2)
                        Data.Instance.myTeam.ForceSecondLevelOpened();
                        GotoCups();
                      //  galaxyMap.AreaClicked(1);
                    }
                    if (id == 2)
                    {
                        Events.CheckForDailyRewards(true, null);
                    }
                    if (id == 3) {
                        GotoCups();
                    }
                    break;
                case "marketplace":
                    GotoMarketplace();
                    break;
            }
        }
        public override void OnButtonCascadePressed(ButtonCascade cascadeButton)
        {
            if (mainButtonPressed) return;
            mainButtonPressed = true;
            base.OnButtonCascadePressed(cascadeButton);
            switch (cascadeButton.buttonID)
            {
                case 0: Events.CheckForDailyRewards(true, null);
                    mainButtonPressed = false; Data.Instance.onBoardingManager.CheckShopVisited(); break;
                case 1: GotoMyTeam();  break;
                case 2: GotoEditor(); break;
                case 3: Gototutorial(); break;
            }
        }
        
        public void GotoMarketplace()
        {
            marketplace.gameObject.SetActive(true);
            marketplace.Init();
            mainMenuAnim.Play("out");
            bgAnim.Play("up");
            GetComponent<MainMenu>().Reset();
        }
        public void GotoMyTeam(int playerID = 0)
        {
            myTeam.gameObject.SetActive(true);
            myTeam.Init();
            mainMenuAnim.Play("out");
            bgAnim.Play("up");
            GetComponent<MainMenu>().Reset();
            DB.DBUserData.DBCharacterData cData = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(playerID);

            if (cData != null)
                myTeam.OnSelectCharacter(cData);

            //Data.Instance.matchData.ResetOponents();
           // Data.Instance.LoadLevel("MyTeam");           
        }
        
        public void GotoEditor()
        {
            editTeamScreen.gameObject.SetActive(true);
            editTeamScreen.Init();
            mainMenuAnim.Play("out");
            bgAnim.Play("up");
            GetComponent<MainMenu>().Reset();
        }
        public void Gototutorial()
        {
            Data.Instance.LoadLevel("Tutorial");
            GetComponent<MainMenu>().Reset();
        }
        //public void GotoStoryMode()
        //{
        //    Data.Instance.matchData.levelData.isCup = false;
        //  //  galaxyMap.gameObject.SetActive(true);
        //  //  galaxyMap.Init();
        //    mainMenuAnim.Play("out");
        //    bgAnim.Play("up");
        //    GetComponent<MainMenu>().Reset();
        //}
        public void GotoCups()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_storymode");
            Data.Instance.mode = Data.modes.STORYMODE;
            Data.Instance.LoadLevel("Levels");
        }
        public void GotoPvp()
        {
            pvpScreen.gameObject.SetActive(true);
            pvpScreen.Init();
            mainMenuAnim.Play("out");
            bgAnim.Play("up");
            GetComponent<MainMenu>().Reset();        
        }
        //public void GotoMundial()
        //{
        //    if(DB.DBManager.Instance.DbUserData.data.country != "")
        //    {
        //        mundialRankingGlobal.gameObject.SetActive(true);
        //        mundialRankingGlobal.Init();
        //        GetComponent<MainMenu>().Reset();
        //    }
        //    else
        //        Data.Instance.LoadLevel("Mundial");
        //}
    }
}