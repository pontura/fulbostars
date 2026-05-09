using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Onboarding;
using System;
namespace Fulbo.UI.Shop
{
    public class DailyRewards : Fulbo.UI.Shop.ShopPanel
    {
        [SerializeField] Text title;
        [SerializeField] Image bg;
        [SerializeField] List<Color> bgColors;
        [SerializeField] Transform container;        
        DailyRewardButton[] buttons;
        int id;
        bool hasSeenDaily;
        System.Action OnClose;
        DailyRewardButton lastRewardClicked;
        string bigPrize;

        public List<DailyIcon> dailyIcons;
        [Serializable]
        public class DailyIcon {
            public GameObject icon;
            public string key;
        }

        [SerializeField] GameObject refreshAnim;
        [SerializeField] GameObject extra_flag_anim;

        public bool CheckForDailyRewards(bool forceOpen, System.Action OnClose)
        {
            if (
                (!forceOpen && 
                    (
                    DB.DBManager.Instance.DbAds.NoMoreAdsForToday()
                    || hasSeenDaily 
                    || !Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.DAILY_REWARDS_OPENED))
                    )
                )
            {
                if (OnClose != null)
                    OnClose();
            }
            else
            {
                Init();
                this.OnClose = OnClose;
                return true;
            }
            return false;
        }
        private void OnDisable()
        {
            if(OnClose != null)
            {
                OnClose();
                OnClose = null;
            }
        }
        public bool HasSeenDaily()
        {
            return hasSeenDaily;
        }
        public void Init()
        {
            extra_flag_anim.SetActive(false);
            hasSeenDaily = true;
            title.text = DB.DBManager.Instance.DbAds.data.adsDisplayed < 5?Data.Instance.texts.Get("DailyReward_title"): Data.Instance.texts.Get("DailyReward_title_2");
            bg.color = DB.DBManager.Instance.DbAds.data.adsDisplayed < 5 ? bgColors[0] : bgColors[1];
            int _id = DB.DBManager.Instance.DbAds.data.adsDisplayed<5?1:6;
            buttons = container.GetComponentsInChildren<DailyRewardButton>();
            bigPrize = DB.DBManager.Instance.DbAds.data.bigPrize;
            foreach (DailyRewardButton c in buttons)
            {
                string text;
                if (_id == 5 || _id == 10)
                {
                    text = Data.Instance.texts.Get("DailyReward_" + _id + "_text_"+ bigPrize);
                }
                else
                    text = Data.Instance.texts.Get("DailyReward_" + _id + "_text");

                string numbersOnly = Regex.Replace(text, "[^0-9]", "");
                int value = 0;
                if (int.TryParse(DB.DBManager.Instance.DbAds.data.prizes[_id-1], out value))
                {
                   // Debug.Log("parsed: " + numbersOnly);
                }
                c.SetPrize(value);
                c.Init(_id, Clicked, text);
                _id++;
            }
            Delayed();

//#if UNITY_EDITOR
//            lastRewardClicked = buttons[0];
//            ForceOpenAtInit();
//#endif
        }
        void ForceOpenAtInit()
        {
            id = 0;
            Invoke("AnimateLastReward", 0.5f);
        }
        void UnableButtonsUsed()
        {
            Invoke("Delayed", 0.2f);
        }
        void AnimateLastReward()
        {                       
            Invoke("animDelayed", 0.1f);
        }
        private void animDelayed()
        {
            DailyRewardButton c = lastRewardClicked;
            c.SetState("claim");
            int totalParticles = 5 * lastRewardClicked.buttonID;
            if (c.value > 0)
            {
                //Vector2 pos = RectTransformUtility.CalculateRelativeRectTransformBounds(c.GetComponentInParent<Canvas>().transform, c.transform).center;
                Vector2 pos = c.transform.position;
                float from = Fulbo.DB.DBManager.Instance.DbUserData.data.score;
                FlyingParticlesUI.types type = FlyingParticlesUI.types.COINS;
                if (DB.DBManager.Instance.DbAds.data.currentPrize == DB.DBAds.PrizeTypes.HARD.ToString()) {
                    type = FlyingParticlesUI.types.HARD;
                    from = Fulbo.DB.DBManager.Instance.DbUserData.data.hard_currency;
                }
                Events.OnFlyingParticles(totalParticles, type, pos, from, c.value);
            } /*else {
                Vector2 pos = c.transform.position;
                Events.OnFlyingParticles(1, FlyingParticlesUI.types.CARD, pos, 1);
            }*/

            if (!DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedDailyExtras) {
                if (c.buttonID == 1 || c.buttonID == 3) {
                    Invoke("ShowExtraFlag", 2);
                }
            }

        }

        void ShowExtraFlag() {
            extra_flag_anim.SetActive(true);
            Invoke("HideExtraFlag", 15);
        }

        void HideExtraFlag() { extra_flag_anim.SetActive(false); }

        void Delayed() {
            id = DB.DBManager.Instance.DbAds.data.adsDisplayed<5? DB.DBManager.Instance.DbAds.data.adsDisplayed: DB.DBManager.Instance.DbAds.data.adsDisplayed-5;
            string keyName = DB.DBManager.Instance.DbAds.data.adsDisplayed < 5 ? "COINS" : "HARD";
            string prize = "";
            if (DB.DBManager.Instance.DbAds.IsNull()) {
                print("_____Load ads results");
                DB.DBManager.Instance.DbAds.Load(UnableButtonsUsed);
                return;
            }
            int _id = 1;
            foreach (DailyRewardButton c in buttons)
            {
                if (id == 0)
                    c.Reset();

                c.buttonID = _id;
                GameObject icon;
                if (_id == 5)
                    icon = dailyIcons.Find(x => x.key == DB.DBManager.Instance.DbAds.data.bigPrize).icon;
                else 
                    icon = dailyIcons.Find(x => x.key == keyName + _id).icon;

                prize = DB.DBManager.Instance.DbAds.data.prizes[_id - 1];
                if(_id == 1)
                    c.SetData(Data.Instance.texts.Get("claim"), icon);
                else
                    c.SetData(Data.Instance.texts.Get("free"), icon);
                if (id+1 == _id)
                {
                    c.SetInteraction(true);
                    c.SetState("idle");
                }
                else
                {
                    c.SetInteraction(false);
                    if (id+1>_id)
                    {
                        c.SetData(Data.Instance.texts.Get("claimed"), icon);
                        c.SetBool("claimed", true);
                        c.SetState("claimed");
                    }else
                        c.SetState("idle");
                }
                _id++;
            }
        }

        void Clicked(int id)
        {
            lastRewardClicked = buttons[id - 1];
            lastRewardClicked.SetInteraction(false);
            if (id == 1) // primero es gratis                    
                OnWatched(true);
            else {
                Events.AdsWatchVideo(OnWatched);
                Events.OnLoadingPanel(true);
                Invoke("ResetLoading", 2);
            }
        }
        void ResetLoading()
        {
            Events.OnLoadingPanel(false);
        }
        void OnWatched(bool ready)
        {
            if(ready)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["adType"] = "DAILY_REWARD";
                param["count"] = DB.DBManager.Instance.DbAds.data.adsDisplayed;
                Events.OnTrack("AdsReward", param);

                //if (lastRewardClicked.gold > 0) //Sfx only plays on coins prices
                //    AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_prize");

                AnimateLastReward();
                //UnableButtonsUsed();
                DB.DBManager.Instance.DbAds.SetNewAdWatched(OnDataReceived);
            } else {
                lastRewardClicked.SetInteraction(true);
                lastRewardClicked.SetState("idle");
            }
            Events.OnLoadingPanel(false);
        }
        
        void OnDataReceived(DB.DBAds.PrizeData data)
        {
            print(data);
            print(data.prizeType);
            if (data == null || data.prizeType == "")
            {
                return;
            }
            if (data.prizeType == "character" || data.prizeType == "character_rare")
                SetNeWCharacter(data);
            else if (data.prizeType == "chest")
                WinChest(data);
            else if(data.prizeType == "coins" || data.prizeType == "hard")
                DB.DBManager.Instance.DbUserData.LoadUserData(OnNewMoney);

            if(DB.DBManager.Instance.DbAds.CheckForSecondRound())
                DB.DBManager.Instance.DbAds.Load(null);
            else
                UnableButtonsUsed();
            //AnimateLastReward();

            Dictionary<string, object> param = new Dictionary<string, object>();
            param["adType"] = "DAILY_REWARD";
            param["count"] = DB.DBManager.Instance.DbAds.data.adsDisplayed;
            param["prizeType"] = data.prizeType;
            param["coins"] = data.coins;
            param["hard"] = data.hard;
            Events.OnTrack("DailyAdsReward", param);
        }
        DB.DBAds.PrizeData data;
        void SetNeWCharacter(DB.DBAds.PrizeData data)
        {
            this.data = data;

            List<int> arr = new List<int>();
            arr.Add(data.characterId);
            DB.DBManager.Instance.DbUserData.SetWonCharacters(arr);
            DB.DBManager.Instance.DbUserData.LoadUserData(OnUserUpdated);
        }
        void WinChest(DB.DBAds.PrizeData data)
        {
            MatchData.ResponseFromServer.ChestDataFromDB chestData = new MatchData.ResponseFromServer.ChestDataFromDB();

            chestData.energy = data.energy;
            chestData.hard = data.hard;
            chestData.shard = data.shard;

            chestData.hard_from = DB.DBManager.Instance.DbUserData.data.hard_currency;
            chestData.energy_from = DB.DBManager.Instance.DbUserData.data.gameData.energyData.available;
            chestData.shard_from = DB.DBManager.Instance.DbUserData.data.shards;

            Events.OpenChest(1, OnChestReady, chestData);
        }
        void OnChestReady()
        {
            RefreshToExtraRewards();
            DB.DBManager.Instance.DbUserData.LoadUserData(OnNewMoney);
        }
        void OnNewMoney()
        {
            UnableButtonsUsed();
            Events.EnergyUpdated();
            Events.RefreshScore(DB.DBManager.Instance.DbUserData.data.score);
            Events.RefreshHardCurrency(DB.DBManager.Instance.DbUserData.data.hard_currency);
        }
        void OnUserUpdated()
        {
            int envelope_rarity = bigPrize == "PLAYER_NORMAL" ? 1 : 2;
            Data.Instance.ui.figusScreen.Init(envelope_rarity, () => Invoke("RefreshToExtraRewards", 2));
        }

        void RefreshToExtraRewards() {
            if (DB.DBManager.Instance.DbAds.CheckForSecondRound()) {
                if(!DB.DBManager.Instance.DbUserData.data.gameData.hasVisitedDailyExtras)
                    DB.DBManager.Instance.DbGameData.Put("hasVisitedDailyExtras", "true", null);                
                Debug.Log("#ShowRefresh");
                Init();
                refreshAnim.SetActive(true);
            }
        }
    }
}
