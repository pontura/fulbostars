using Fulbo.DB;
using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fulbo.Energy.UI
{
    public class BuyEnergyPopupUI : Fulbo.UI.Shop.ShopPanel
    {
        int totalAvailables = 5;
        int totalEnergyToBuy = 15;

       // [SerializeField] GameObject newEnergyPopup;
        [SerializeField] Text totalEnergyQty;
        [SerializeField] Text buyEnergyDesc;


        [SerializeField] BuyEnergyPopupUIButton[] buttons;

        [SerializeField] GameObject infinitySign;

        int total_new_energy;
        public void Init(bool isOn)
        {
            totalEnergyQty.text = "x1";
         //   newEnergyPopup.SetActive(false);
            if(isOn)
            {
                DateTime now = DB.DBManager.Instance.Now();
                if (now.DayOfWeek != DayOfWeek.Sunday) {
                    DBEnergy.EnergyPriceDB e = DB.DBManager.Instance.DbEnergy.GetPrice();
                    if (e != null)
                        OnPriceLoaded(e);
                    else
                        StartCoroutine(DB.DBManager.Instance.DbEnergy.GetPriceFromServerCoroutine(OnPriceLoaded));
                } else 
                    BuyEnergyPopupDataLoaded(true);

                infinitySign.SetActive(now.DayOfWeek == DayOfWeek.Sunday);
                foreach (BuyEnergyPopupUIButton button in buttons)
                    button.SetInteraction(now.DayOfWeek != DayOfWeek.Sunday);

            }
            else
                BuyEnergyPopupDataLoaded(false);
        }
        void OnPriceLoaded(DBEnergy.EnergyPriceDB e)
        {
            OnPriceReceived(e);
            BuyEnergyPopupDataLoaded(true);            
        }
        void BuyEnergyPopupDataLoaded(bool isOn)
        {
            if (isOn)
            {
                int id = 1;
                foreach (BuyEnergyPopupUIButton button in buttons)
                {
                    button.Init(id, ButtonClicked);
                    button.InitData(id);
                    id++;
                }
               // buyButton.Init(4, ButtonClicked, Data.Instance.texts.Get("buy_fulbo"));
            }
            int totalSeen = DB.DBManager.Instance.DbUserData.data.gameData.energyData.videosSeen;

            BuyEnergyPopupUIButton priceButton = buttons[0];
            if (totalSeen >= totalAvailables)
            {
                priceButton.SetInteraction(false);
                priceButton.NotAvailable();
                Events.OnFreeStaffUpdate(Fulbo.UI.Shop.Shop.sectionType.ENERGY, false);
            }
            else
            {
                priceButton.SetSeen(totalSeen, totalAvailables);
                priceButton.SetInteraction(true);
            }
        }
        int price = 0;
        int count = 0;
        public void OnPriceReceived(DB.DBEnergy.EnergyPriceDB data)
        {
            price = data.price;
            count = data.count;
            BuyEnergyPopupUIButton priceButton = buttons[1];
            priceButton.SetPrice(data.price);

            if (data.price > 0) {
                buyEnergyDesc.text = data.count + "/" + totalEnergyToBuy;
                priceButton.SetInteraction(true);
            } else {
                buyEnergyDesc.text = Data.Instance.texts.Get("videos_not_available");
                priceButton.SetInteraction(false);
            }
        }
        void ButtonClicked(int id)
        {
            switch (id)
            {
                case 1:
                    total_new_energy = 3;
                    Events.AdsWatchVideo(OnVideoWatched);
                    
                    //Events.OnPopup(Data.Instance.texts.Get("feature_not_ready"), null);
                    break;
                case 2:
                    total_new_energy = 1;
                    if (price == 0)
                        return;
                    if (DB.DBManager.Instance.DbUserData.data.score < price)
                        Events.OnPopup(Data.Instance.texts.Get("not_enough_money") + " " + price, null);
                    else
                        OnBuyReady(true);  // Events.ConfirmBuy((float)price, OnBuyReady);

                    break;
                case 3:
                   // CloseWinEnergyPopup();
                    break;
            }
        }
        void AddEnergy(int qty)
        {
            float from = DB.DBManager.Instance.DbUserData.data.gameData.energyData.available;
            Events.OnFlyingParticles(qty, FlyingParticlesUI.types.ENERGY, transform.position, from, qty);

        }
        void OnVideoWatched(bool isOk)
        {
            if (isOk)
            {
                AddEnergy(total_new_energy);
                DB.DBManager.Instance.DbUserData.data.gameData.VideoSeen();
                DB.DBManager.Instance.DbUserData.data.gameData.OnEnergyChanged(total_new_energy, OnEnergySavedToDB);

                //Analytics
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("EnergyBought", false);
                Events.OnTrack("ExtraEnergy", param);

                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("adType", "EXTRA_ENERGY");
                Events.OnTrack("AdsReward", param2);

                Dictionary<string, object> param3 = new Dictionary<string, object>();
                param2.Add("count", DB.DBManager.Instance.DbUserData.data.gameData.energyData.videosSeen);
                param2.Add("type", "ad");
                Events.OnTrack("EnergyBought", param3);
            }
        }
        void OnBuyReady(bool isOk)
        {            
            if (isOk)
            {
                DB.DBManager.Instance.DbEnergy.Save(OnDone);
                AddEnergy(1);
            }
        }
        string response = "";
        void OnDone(bool isOk, string response)
        {            
            this.response = response;
            Debug.Log("OnDone " + response);
            if (isOk)
            {
                DB.DBManager.Instance.DbUserData.data.gameData.OnEnergyChanged(1, OnEnergySavedToDB);

                //Analytics
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("EnergyBought", true);
                Events.OnTrack("ExtraEnergy", param);

                Dictionary<string, object> param2 = new Dictionary<string, object>();
                param2.Add("count", count);
                param2.Add("type", "soft");
                Events.OnTrack("EnergyBought", param2);
                
            }
            else
            {
                Events.OnPopup(response, null);
            }
            
            Events.OnLoadingPanel(false);
        }
        void OnEnergySavedToDB(bool isOk, string error)
        {
            Events.OnLoadingPanel(false);
            if (isOk)
            {
                Debug.Log("# New Energy saved! " + response);
                DBEvents.LoadUserData(OnUserLoaded);
            }
            else
                Events.OnPopup(error, null);
        }
        void OnUserLoaded()
        {
            Events.OnLoadingPanel(false);
           // Events.EnergyUpdated();
            Events.RefreshScore(DBManager.Instance.DbUserData.data.score);
            Events.RefreshHardCurrency(DBManager.Instance.DbUserData.data.hard_currency);
         //   closeNewEnergyBtn.Init(0, CloseNewEnergyPopup);
            buyEnergyDesc.text = "";

            if (gameObject.activeSelf)
                StartCoroutine(DB.DBManager.Instance.DbEnergy.GetPriceFromServerCoroutine(OnPriceLoaded));
        }
        //public void OpenEnergyPopup()
        //{
        //    newEnergyPopup.SetActive(true);
        //}
        //void OnDisable()
        //{
        //    CloseWinEnergyPopup();
        //}
        //public void CloseWinEnergyPopup()
        //{
        //    newEnergyPopup.SetActive(false);
        //}
        //public void CloseNewEnergyPopup(int id)
        //{
        //    newEnergyPopup.SetActive(false);
        //}

        public bool HasFreeEnergyAvailable() {
            int totalSeen = DB.DBManager.Instance.DbUserData.data.gameData.energyData.videosSeen;
            return totalSeen < totalAvailables;
        }

        public bool HasEnergyAvailable() {
            return HasFreeEnergyAvailable() || price > 0;
        }
    }
}