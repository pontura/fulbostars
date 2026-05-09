using Fulbo.Energy.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fulbo.Onboarding;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] GameObject topBar;
        [SerializeField] GameObject topBarTutorial;
        [SerializeField] BackButton back;
        [SerializeField] GameObject score;
        [SerializeField] GameObject hard;
        [SerializeField] GameObject shards;
        [SerializeField] Text titleField;
        [SerializeField] GameObject title;
        [SerializeField] GameObject[] plusIconsOnTop;

        [SerializeField] EnergySignalUI energySignalUI;
        [SerializeField] CupsLifes cupsLifes;
        [SerializeField] GameObject settingsButton;
        [SerializeField] ButtonCustom buyGoldButton;
        [SerializeField] ButtonCustom buyHardButton;
        [SerializeField] Fulbo.UI.EditTeam.ClubShield clubShield;
        [SerializeField] Fulbo.UI.Shop.Shop shop;
        public FigusScreen figusScreen;

        [SerializeField] SettingsUI settingsUI;

        private void Start()
        {
            topBar.SetActive(false);
            settingsButton.SetActive(false);

            Events.OnFlyingParticles += OnFlyingParticles;
            Events.OnSceneLoaded += OnSceneLoaded;
            buyGoldButton.gameObject.SetActive(true);
            buyGoldButton.Init(0, OnButtonClicked);
            buyHardButton.Init(1, OnButtonClicked);

            CheckForPlusIcons();
        }
        void CheckForPlusIcons()
        {
            bool isActive = false;
            if (Fulbo.Onboarding.OnBoardingManager.SecondMatchPlayed())
                isActive = true;
            foreach (GameObject plusIcon in plusIconsOnTop)
            {
                if(plusIcon != null)
                    plusIcon.SetActive(isActive);
            }
        }
        void OnFlyingParticles(int arg1, FlyingParticlesUI.types type, Vector2 arg3, float a, float arg4)
        {
            if(type == FlyingParticlesUI.types.HARD)
                hard.gameObject.SetActive(true);
        }
        public void OnHardBought()
        {
            hard.gameObject.SetActive(true);
        }
        public void OnCupWon()
        {
            shards.gameObject.SetActive(true);
            hard.gameObject.SetActive(true);
        }

        public void ShowHardOnShop(bool enable) {
            if (DB.DBManager.Instance.DbUserData.data.hard_currency > 0)
                hard.gameObject.SetActive(true);
            else
                hard.gameObject.SetActive(enable);
            
        }

        public void OnAddShards(int shards)
        {
            print("________SHARDS WON" + shards);
            int qty = shards;
            if (qty > 20) qty = 20;
            Vector2 centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            float from = Fulbo.DB.DBManager.Instance.DbUserData.data.shards;
            Events.OnFlyingParticles(qty, FlyingParticlesUI.types.SHARDS, centerOfScreen, from, shards);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/cards/ui_prize_particle");

        }
        void OnButtonClicked(int id)
        {
            if (id == 0)
                Events.OpenShop(Shop.Shop.sectionType.SOFT_PACKS);
            else if (id == 1)
                Events.OpenShop(Shop.Shop.sectionType.HARD_PACKS);
            // BuyCoins();
        }
        //void BuyCoins()
        //{
        //    if (Application.platform == RuntimePlatform.WindowsEditor ||  Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //        Events.OnOpenInAppPurchases(Purchasing.UI.InAppPurchasesUI.types.COINS);
        //    else
        //        Events.OnPopup(Data.Instance.texts.Get("store_not_available"), null);
        //}
        void OnDestroy()
        {
            Events.OnSceneLoaded -= OnSceneLoaded;
            Events.OnFlyingParticles -= OnFlyingParticles;
        }
        public void Init()
        {
            backActions = new List<System.Action>();
            GetComponent<TutorialProgressMenu>().Init();
            energySignalUI.Init();
            cupsLifes.Init();
        }

        void OnSceneLoaded(string sceneName)
        {
            CheckForPlusIcons();
            settingsButton.SetActive(true);
            clubShield.gameObject.SetActive(false);
            switch (sceneName)
            {
                case "0_Register":
                    topBarTutorial.SetActive(false);
                    topBar.SetActive(false);
                    Events.ShowTutorialProgressMenu(null, false);
                    back.SetActive(false);
                    break;
                case "Intro":
                    topBarTutorial.SetActive(false);
                    break;
                case "Tutorial":
                    topBarTutorial.SetActive(true);
                    topBar.SetActive(true); break;
                case "MainMenu":
                    clubShield.gameObject.SetActive(true);
                    clubShield.Init();
                    Events.ShowTutorialProgressMenu(null, false);
                    topBar.SetActive(true);
                    topBarTutorial.SetActive(false);
                    break;
                case "Game":
                case "GameIntro":
                    topBar.SetActive(false);
                    break;
                case "GameOver":
                case "MyTeamSelector":
                case "EditTeam":
                    if (Data.Instance.mode == Data.modes.PARTYMODE)
                        topBar.SetActive(false);
                    else
                        topBar.SetActive(true);
                    break;
            }
        }
        string lastTitle = "";
        List<System.Action> backActions;
        public void SetBackButton(bool isOn, System.Action _BackClicked = null, string titleText = "", BackButton.types type = BackButton.types.BACK)
        {

            print("SetBackButton " + isOn + " num de backs: " + backActions.Count + " titleText: " + titleText + " type: " + type);

            if (!isOn)
            {
                if (backActions.Count > 0)
                {
                    System.Action action = backActions[backActions.Count - 1];
                    backActions.Remove(action);
                }
                if(backActions.Count <= 0)
                    back.SetActive(false);
            }
            else
            {
                back.SetActive(true);
                //// para no agregar un Back a una sección ya existente.
                //if (titleText != "" && titleText == lastTitle && backActions.Count>0)
                //{
                //    backActions[0] = _BackClicked;
                //    return;
                //}
                lastTitle = titleText;
                this.backActions.Add(_BackClicked);
            }
            title.SetActive(titleText != "");
            titleField.text = titleText;

          //  if (!Data.Instance.isMobile) return;

            back.SetType(type);
            if (isOn)
                clubShield.gameObject.SetActive(false);
            else
            {
                clubShield.gameObject.SetActive(true);
                if (DB.DBManager.Instance.DbUserData.data.shortName != "")
                    clubShield.Init(Data.Instance.myTeam.clubData);
                else
                    clubShield.Init();
            }

            //if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.SECOND_MATCH_PLAYED)) back.SetActive(false);
        }
        public void SetScore(bool isOn)
        {
            if (!Data.Instance.isMobile) return;
            score.SetActive(isOn);
        }

        public void SetLifes(bool isOn) {
            if (!Data.Instance.isMobile) return;
            cupsLifes.SetActive(isOn);
        }

        public void CheckCupLifeLose() {
            cupsLifes.CheckLifeLose();
        }

        public void Back()
        {
            //if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.DAILY_REWARDS_OPENED)) return;
            AudioManager.Instance.PlaySoundOneShot("ui", "_new/ui/click3");
            if (backActions.Count>0)
            {
                System.Action action = backActions[backActions.Count-1];
                action();
            }
        }
        public void HamburgerClicked()
        {
            Events.OpenSettings(false);
        }

        public void ShowShield(bool enable) {
            clubShield.gameObject.SetActive(enable);
            back.SetActive(!enable);
        }

        public void ShowTutorialTopBar(bool enable) {
            topBarTutorial.SetActive(enable);
        }

        public void SettingsButtonActive(bool enable) {
            settingsButton.SetActive(enable);
        }

        public bool IsShopOn() {
            return shop.IsOn();
        }

        public bool HasEnergyAvailable() {
            return shop.HasEnergyAvailable();
        }

        public void SetPauseButton(bool enable) {
            settingsUI.SetIngamePauseButton(enable);
        }
    }
}
