using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using static Fulbo.PricesData;
using static Fulbo.DB.DBUserData;
using System;

namespace Fulbo.UI.Shop
{
    public class PlayersPackButton : ButtonCustom
    {
        [SerializeField] Transform container;

        [SerializeField] Text charactersField;

        [SerializeField] ButtonCustom nextPlayerButton;
        [SerializeField] ButtonCustom prevPlayerButton;

        [SerializeField] ButtonCustom softButton;
        [SerializeField] ButtonCustom hardButton;

        [SerializeField] GameObject weekendDiscountBanner;
        [SerializeField] GameObject weekendGalaxiumDiscount;

        [SerializeField] Text galaxiumDiscountPrice;
        [SerializeField] Text galaxiumDiscountPercent;
        [SerializeField] Text galaxiumDiscountDay;

        int id = 0;
        [SerializeField] List<QtySoftHard> d;
        public int packID;
        string currency;
        public void InitPack(int packID, GameObject pack, List<QtySoftHard> d)
        {
         //   print("pack " + packID);
            this.packID = packID;
            Utils.RemoveAllChildsIn(container);
            GameObject go = Instantiate(pack, container);

            Button[] b = go.GetComponentsInChildren<Button>();
            foreach (Button bs in b)
                bs.enabled = false;

            go.transform.localScale = Vector2.one;
            this.d = d;
            id = 0;
            nextPlayerButton.Init(0, OnClicked);
            prevPlayerButton.Init(1, OnClicked);

            softButton.Init(2, OnClicked);
            hardButton.Init(3, OnClicked);
            
        }
        public void OnRestart()
        {
            id = 0;
            SetData();
        }
        QtySoftHard data;
        void OnClicked(int buttonID)
        {
            if (buttonID < 2)
            {
                if (buttonID == 0) id++;
                else if (buttonID == 1) id--;

                if (id < 0) id = d.Count - 1;
                else if (id > d.Count - 1)
                    id = 0;

                SetData();
            }
            else
            {
                data = new QtySoftHard();
                data.qty = d[id].qty;
                data.soft = d[id].soft;
                data.hard = d[id].hard;

                if (buttonID == 3)
                {
                    currency = "hard";
                    data.soft = 0;
                }
                if (buttonID == 2)
                {
                    currency = "soft";
                    data.hard = 0;
                }
                Events.ConfirmBuyPlayers(data, packID, OnPurchaseDone);
            }   

        }

        Dictionary<string, object> param;  
        void OnPurchaseDone(bool isDone)
        {
            if (isDone)
            {
                if(currency == "hard" && DB.DBManager.Instance.DbUserData.data.hard_currency < data.hard)
                {
                    Events.PopupText(Data.Instance.texts.Get("not_enough_hard"), Data.Instance.texts.Get("not_enough_hardText"), null);
                    return;
                }
                else if (currency == "soft" && DB.DBManager.Instance.DbUserData.data.score < data.soft)
                {
                    Events.PopupText(Data.Instance.texts.Get("not_enough_soft"), Data.Instance.texts.Get("not_enough_softText"), null);
                    return;
                }
                Events.OnLoadingPanel(true);
                QtySoftHard d = GetData();
                //print("hard: " + d.hard + " soft: " + d.soft + " qty: " + d.qty);
                param = new Dictionary<string, object>();
                param["pack"] = packID;
                param["quantity"] = d.qty;
                param["currency"] = currency;                
                DB.DBManager.Instance.DbPlayerPacks.Buy(packID, d.qty, currency, OnBought);
            }
        }
        public class DBCharacters
        {
            public string message;
            public List<DBCharacterData> characters;
        }
       
        void OnBought(bool isOk, string result)
        {
            if(isOk)
            {
                List<int> arr = new List<int>();
                DBCharacters d = JsonUtility.FromJson<DBCharacters>(result);
               // Debug.Log("OnBought: " + result);
                if (d == null)
                {
                    Debug.LogError("ERROR_ " + result);
                    Events.OnPopup("Error with characters", null);
                }
                else
                {
                  //  Debug.Log("OnBought message: " + d.message);

                   // Debug.Log("OnBought characters: " + d.characters);
                    Debug.Log("OnBought role: " + d.characters[0].role);

                    foreach (DBCharacterData cd in d.characters)
                        arr.Add(cd.id);

                    DB.DBManager.Instance.DbUserData.SetWonCharacters(arr);
                    DB.DBManager.Instance.DbUserData.LoadUserData(OnReLoadUser);

                    Events.OnTrack("PlayersPackBought", param);
                }
            } else
                Events.OnLoadingPanel(false);

        }
        void OnReLoadUser()
        {
          //  Events.OpenShop(Shop.sectionType.CLOSE);
            Events.OnLoadingPanel(false);
            Data.Instance.ui.figusScreen.Init(packID);
        }
        public void SetData()
        {
            QtySoftHard d = GetData();
         //   print("______SetData PlayersPackButton  " + d.qty + " " + d.soft + " " + d.hard);
            string description = d.qty > 1 ? "Players" : "Player";
            charactersField.text = d.qty + " " + description;            
            softButton.SetText(Utils.FormatNumbers(d.soft, true));
            int hardVal = d.hardOnSalePercentage > 0 ? d.hardRegular : d.hard;
            hardButton.SetText(Utils.FormatNumbers(hardVal));
            weekendDiscountBanner.SetActive(d.hardOnSalePercentage > 0);
            weekendGalaxiumDiscount.SetActive(d.hardOnSalePercentage > 0);
            galaxiumDiscountPrice.text = ""+d.hard;
            galaxiumDiscountPercent.text = d.hardOnSalePercentage + "% GALAXIUMS OFF";
            galaxiumDiscountDay.text = DB.DBManager.Instance.Now().DayOfWeek.ToString().ToUpper() +" OFFER";
        }
        QtySoftHard GetData()
        {
            int _id = 0;
            foreach (QtySoftHard s in d)
            {
                if (_id == id)
                    return s;
                _id++;
            }
            return d[0];
        }
    }
}
