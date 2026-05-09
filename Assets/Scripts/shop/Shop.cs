using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fulbo.DB;
using static Fulbo.UI.TabsManager;

namespace Fulbo.UI.Shop
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] Scrollbar scrollBar;
        [SerializeField] GameObject panel;
        [SerializeField] Transform container;

        [SerializeField] ShopPanel[] panels;

        [SerializeField] DailyRewards dailyRewards;
        [SerializeField] PlayersPacks playersPacks;
        [SerializeField] ShopChests chests;
        [SerializeField] Energy.UI.BuyEnergyPopupUI buyEnergyPopupUI;

        [SerializeField] TabsManager tabsManager;
        [SerializeField] float smoothAutomaticScroll = 0.1f;
        bool isOn;
        [Serializable] class Section
        {
            public sectionType type;
            public GameObject panel;
        }
        public enum sectionType
        {
            DEFAULT,
            DAILY_REWARDS,
            PLAYER_PACKS,
            HARD_PACKS,
            SOFT_PACKS,
            ENERGY,
            CHESTS,
            CLOSE
        }
        public scrollStates scrollState;
        public enum scrollStates
        {
            IDLE,
            AUTO,
            SCROLLING
        }
        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            Events.CheckForDailyRewards += CheckForDailyRewards;
            Events.OpenShop += OpenShop;
            Events.BuyEnergyPopup += BuyEnergyPopup;
            Events.OnOpenInAppPurchases += OnOpenInAppPurchases;
            DB.DBManager.Instance.DbAds.Load(null);
            panel.SetActive(false);
#endif
        }
        private void OnDestroy()
        {
            Events.CheckForDailyRewards -= CheckForDailyRewards;
            Events.BuyEnergyPopup -= BuyEnergyPopup;
            Events.OpenShop -= OpenShop;
            Events.OnOpenInAppPurchases += OnOpenInAppPurchases;
        }
        public bool IsOn() {
            return isOn;
        }
        void DelayedBack()
        {
            if (!isOn) return;
            string title = Data.Instance.texts.Get("shop");
            Data.Instance.ui.SetBackButton(true, Close, title);            
        }
        bool initDone;
        void Init()
        {
            Invoke("DelayedBack", 0.5f);
            StartCoroutine(DBManager.Instance.DbEnergy.GetPriceFromServerCoroutine(null));
            Data.Instance.ui.ShowHardOnShop(true);

            if (initDone) return;
            initDone = true;    
            
            playersPacks.Init();
            chests.Init();        

            tabsManager.InitScroll(OnTabClicked);

            foreach (ShopPanel s in panels)
            {
                tabsManager.AddScrollButton(s.gameObject, s.text_in_database);
            }
            tabsManager.SetButtons();
            Invoke("DelayedForFree", 0.1f);
        }
        void DelayedForFree()
        {
            int id = 0;
            foreach (ShopPanel s in panels)
            {
                bool isFree = false;
                FreeStaffTag fst = tabsManager.buttons[id].GetComponent<FreeStaffTag>();
                switch (s.type)
                {
                    case sectionType.DAILY_REWARDS:
                        if (!DB.DBManager.Instance.DbAds.NoMoreAdsForToday())
                            isFree = true;
                        fst.sectionType = sectionType.DAILY_REWARDS;
                        break;
                    case sectionType.ENERGY:
                        if (buyEnergyPopupUI.HasFreeEnergyAvailable())
                            isFree = true;
                        fst.sectionType = sectionType.ENERGY;
                        break;
                }
                /*foreach (Image go in tabsManager.buttons[id].GetComponentsInChildren<Image>())
                {
                      if (go.name == "FreeStuff")
                        go.gameObject.SetActive(isFree);
                }*/
                tabsManager.buttons[id].GetComponent<FreeStaffTag>().SetActive(isFree);
                id++;
            }
        }
        public void OnOpenInAppPurchases(Shop.sectionType type)
        {
            OpenShop(sectionType.HARD_PACKS);
        }
        void BuyEnergyPopup(bool isOn)
        {
            if (isOn)
                OpenShop(sectionType.ENERGY);
            else
            {
                Close();
                buyEnergyPopupUI.Init(false);
            }
        }

        public bool HasEnergyAvailable() {
            return buyEnergyPopupUI.HasEnergyAvailable();
        }

        public void CheckForDailyRewards(bool forceOpen, System.Action OnClose)
        {
            bool force = dailyRewards.CheckForDailyRewards(forceOpen, OnClose);
            if (force) OpenShop();
        }
        int GetPanel(sectionType t)
        {
            int id = 0;
            foreach (ShopPanel s in panels)
            {
                if (s.type == t)
                    return id;
                id++;
            }
            return 0;
        }
        void OpenShop(sectionType type = sectionType.DEFAULT)
        {
            if(type == sectionType.CLOSE)
            {
                Close();
                return;
            }
            if (isOn) return;
            if (type != sectionType.DEFAULT)
                Data.Instance.onBoardingManager.CheckShopVisited();

            isOn = true;
            panel.SetActive(true);
            Init();

            playersPacks.SetActive();

            int itemID = GetPanel(type);
            tabsManager.Select(itemID);

            dailyRewards.Init();
            buyEnergyPopupUI.Init(true);

            if (Data.Instance.newScene == "Tutorial")
                Data.Instance.ui.ShowTutorialTopBar(false);

        }
        float scrollAutoDest;
        void OnTabClicked(ListItemData listItemData)
        {
            scrollState = scrollStates.AUTO;
            scrollAutoDest = listItemData.pos_scroll_x;
        }
        private void Update()
        {
            if (scrollState == scrollStates.AUTO)
            {
                float _v = scrollBar.value;
                _v = Mathf.Lerp(_v, scrollAutoDest, smoothAutomaticScroll);                
                if(Mathf.Abs(_v- scrollBar.value)<0.0001f)
                {
                    _v = scrollAutoDest;
                    scrollState = scrollStates.IDLE;
                }
                scrollBar.value = _v;
            }
        }

        public void OnScrollChange() {
            if (scrollState != scrollStates.AUTO) {
                tabsManager.HighlightTab(scrollBar.value);
            }
        }
                
        void Close()
        {
            isOn = false;
            Data.Instance.ui.SetBackButton(false);
            Data.Instance.ui.ShowHardOnShop(false);
            panel.SetActive(false);
            if (Data.Instance.newScene == "Tutorial")
                Data.Instance.ui.ShowTutorialTopBar(true);
        }
    }
}
