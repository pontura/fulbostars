using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Fulbo.UI.Ads
{
    public class AdsInSummary : MonoBehaviour
    {
        public Animation buttonAnim;

        [SerializeField] GameObject panel;
        [SerializeField] Transform container;

        [SerializeField] Transform cupContainer;
       // [SerializeField] ButtonCustom cupButton;

        [SerializeField] WatchVideoPoster[] posters;
        [SerializeField] ButtonCustom watchForMoneyBtn;
        [SerializeField] ButtonCustom watchForXpBtn;
        [SerializeField] ButtonCustom continueBtn;
        [SerializeField] CupProgress cupProgress;

        UIGameOverMenu uIGameOverMenu;
        int totalScorex2;

        int id = 0;
        bool hasWatchAnyAd;
        private void Start()
        {
            Close();
        }
        public void Init(UIGameOverMenu uIGameOverMenu, int id, int totalScorex2)
        {
            this.id = id; //1 = coins       2 = xp
            this.uIGameOverMenu = uIGameOverMenu;
            this.totalScorex2 = totalScorex2;
            panel.SetActive(true);
            panel.GetComponent<Animation>().Play("summaryAds");
            watchForMoneyBtn.gameObject.SetActive(false);
            watchForXpBtn.gameObject.SetActive(false);

            Utils.RemoveAllChildsIn(container);
            WatchVideoPoster poster = Instantiate(posters[id-1], container);
            poster.transform.localPosition = Vector2.one;
            poster.Init(Data.Instance.texts.Get("watch_video_for_summary"));
            string continueText = "";
            continueText = Data.Instance.texts.Get("continue");
            if (id == 1)
            {
                watchForMoneyBtn.gameObject.SetActive(true);
                string btn_text = "";
                if (Data.Instance.matchData.levelData.cupID < 30)
                    btn_text = "_12";
                else if (Data.Instance.matchData.levelData.cupID < 50)
                    btn_text = "_34";
                watchForMoneyBtn.Init(1, Clicked, Data.Instance.texts.Get("watch_ad_for_money"+ btn_text));
                switch (uIGameOverMenu.state)
                {
                    case UIGameOverMenu.states.WIN_CUP:
                    case UIGameOverMenu.states.LOST_CUP:
                        Data.Instance.ui.SetBackButton(true, () => BackClicked(), "", BackButton.types.HOME);
                        if (cupProgress != null) cupProgress.Init();
                        continueText = Data.Instance.texts.Get("continue");
                        break;
                    case UIGameOverMenu.states.LOSE:
                        continueText = DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep > 3 ? Data.Instance.texts.Get("retry") : Data.Instance.texts.Get("continue");
                        SetCupButton();
                        break;
                    default:
                        continueText = Data.Instance.texts.Get("no_thanks");
                        break;
                }
            }
            else
            {
                watchForXpBtn.gameObject.SetActive(true);
                string btn_text = "";
                if (Data.Instance.matchData.levelData.cupID < 30)
                    btn_text = "_12";
                else if (Data.Instance.matchData.levelData.cupID < 50)
                    btn_text = "_34";
                watchForXpBtn.Init(2, Clicked, Data.Instance.texts.Get("watch_ad_for_xp"+ btn_text));
                switch (uIGameOverMenu.state)
                {
                    case UIGameOverMenu.states.WIN:
                        continueText = DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep > 3 ? Data.Instance.texts.Get("next_match") : Data.Instance.texts.Get("continue");
                        SetCupButton(false);
                        break;
                    case UIGameOverMenu.states.WIN_CUP:
                    case UIGameOverMenu.states.LOST_CUP:
                        continueText = Data.Instance.texts.Get("continue");
                        break;
                }
            }
            continueBtn.Init(0, Clicked, continueText);
        }
        void SetCupButton(bool hasLose = true) {
            Data.Instance.ui.SetBackButton(true, () => BackClicked(hasLose), "", BackButton.types.HOME);
            // cupButton.gameObject.SetActive(true);
            if (cupProgress != null)
                cupProgress.Init();
           // cupButton.Init(0, CupClicked);
        }
        void BackClicked(bool hasLose = true)//)
        {
            if (hasWatchAnyAd || hasLose)
                uIGameOverMenu.GotoMainMenu();
            else
                Events.AdsWatchInterstitial((x) => uIGameOverMenu.GotoMainMenu());
        }
        void Clicked(int id)
        {
            switch(id)
            {
                case 0:
                    GoOn(); break;
                case 1:
                    Events.AdsWatchVideo(OnWatchedForMoney);
                    break;
                case 2:
                    Events.AdsWatchVideo(OnWatchedForXp);
                    break;
            }
        }
        void OnWatchedForMoney(bool watched)
        {
            if (watched)
            {
                buttonAnim.Stop();
                hasWatchAnyAd = true;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["adType"] = "SCORE_X2";
                param["stadium"] = Data.Instance.matchData.levelData.stadium_id;
                param["level"] = Data.Instance.matchData.levelData.id;
                Events.OnTrack("AdsReward", param);

                float from = DBManager.Instance.DbUserData.data.score;
                Events.OnFlyingParticles(20, FlyingParticlesUI.types.COINS, watchForMoneyBtn.transform.position, from, totalScorex2);
                DB.DBBoosts boosts = new DB.DBBoosts();
                boosts.SendBoost("score", OnBoostSaved);
                watchForMoneyBtn.SetInteraction(false);                

                continueBtn.SetText(Data.Instance.texts.Get("continue"));
            }
        }
        void OnWatchedForXp(bool watched)
        {
            if (watched)
            {
                Events.OnLoadingPanel(true);
                hasWatchAnyAd = true;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["adType"] = "XP_X2";
                param["stadium"] = Data.Instance.matchData.levelData.stadium_id;
                param["level"] = Data.Instance.matchData.levelData.id;
                Events.OnTrack("AdsReward", param);

                print("OnBoostXPSaved");
                DB.DBBoosts boosts = new DB.DBBoosts();
                boosts.SendBoost("xp", OnBoostXPSaved);
                watchForXpBtn.SetInteraction(false);
            }
        }
        bool energyPopupShown;
        void GoOn()
        {
            if (id == 1 && uIGameOverMenu.Win())
                uIGameOverMenu.SummaryOff();
            else
            {
                if(hasWatchAnyAd)
                    uIGameOverMenu.GoOn();
                else
                    Events.AdsWatchInterstitial((x) => {
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param["type"] = "matchWin";
                        if (x)
                            Events.OnTrack("IterstitialAd", param);
                        else
                            Events.OnTrack("InterstitialAdNotShown", param);
                        uIGameOverMenu.GoOn();
                    });
            }
        }
        void OnBoostSaved(bool isOk, string response)
        {
            if (!isOk)
                Events.OnPopup(response, null);
            DB.DBManager.Instance.DbUserData.LoadUserData(OnUserUpdated);
            Debug.Log("Boost saved");
        }
        void OnBoostXPSaved(bool isOk, string response)
        {
            if (!isOk)
                Events.OnPopup(response, null);

            DB.DBManager.Instance.DbUserData.LoadUserData(OnUserUpdatedForXP);
            Debug.Log("Boost saved");
        }
        void OnUserUpdated()
        {
            Debug.Log("User Updated");
        }
        void OnUserUpdatedForXP()
        {
            print("OnUserUpdatedForXP");
            Events.OnLoadingPanel(false);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_congratulations");
            GetComponent<SummaryCharactersStatsUpgrade>().Init();
            string t = Data.Instance.texts.Get("stats_upgrade_title") + " (x2 XP)";
            GetComponent<SummaryCharactersStatsUpgrade>().SetTitle(t);
        }
        public void Close()
        {
            panel.SetActive(false);
        }
    }
}
