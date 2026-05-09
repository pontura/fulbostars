using Fulbo.UI.Paginator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Marketplace
{
    public class MarketplaceUI : CascadeList 
    {
        [SerializeField] Text title;

        [SerializeField] Text feedbackField;
        [SerializeField] GameObject feedback;

        MarketplacePopupUI marketplacePopupUI;
        public Transform container;
        [SerializeField] MarketplaceCharacterButton marketplaceCharacterButton;
        public ButtonCustom[] topButtons;
        public ButtonCustom[] filterButtons;
        public ButtonCustom[] filterRarityButtons;
        [SerializeField] Image[] backgrounds;
        [SerializeField] GameObject[] backgroundsPanels;
        List<MarketplaceCharacterButton> buttons;
        [SerializeField] PaginatorManager paginatorManager;
        Animation anim;

        int from;
        int to;
        int totalButtonsInPage = 20;
        int arrayID = 0;

        public bool filterByRarity;
        public FigusData.rarities rarity;

        List<CharactersData.CharacterData> allCharacters;
        List<DB.DBUserData.DBCharacterData> allMyCharacters;

        public states state;
        public enum states
        {
            NONE,
            BUY,
            SELL
        }

        public tabs tab;
        public enum tabs
        {
            PLAYERS,
            GOALKEEPERS,
            FIGUS
        }

        public void Init()
        {
            feedback.SetActive(false);
            anim = GetComponent<Animation>();
            anim.Play("in");
            from = 0;
            to = totalButtonsInPage;

            marketplacePopupUI = GetComponent<MarketplacePopupUI>();
            Events.OnSkipOff();

            title.text = Data.Instance.texts.Get("marketplace");
            feedbackField.text = Data.Instance.texts.Get("not_players_by_filter");

            topButtons[0].Init(1, OnStateButtonClick, Data.Instance.texts.Get("marketplace_buy"));
            topButtons[1].Init(2, OnStateButtonClick, Data.Instance.texts.Get("marketplace_sell"));


            filterButtons[0].Init(0, OnFilter, Data.Instance.texts.Get("players"));
            filterButtons[1].Init(1, OnFilter, Data.Instance.texts.Get("goalkeepers"));

            int id = 0;
            foreach (ButtonCustom button in filterRarityButtons)
            {
                button.Init(id, OnFilterRarity, Data.Instance.texts.Get("rarity_" + id), false);
                button.OnSelected(false);
                id++;
            }
            Invoke("OnAnimReady", 0.85f);
            Events.OnBuyReady += OnBuyReady;
            Invoke("BackDelayed", 0.75f);
        }
        void BackDelayed()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }
        void OnAnimReady()
        {
            OnStateButtonClick(1);
            //OnFilter(0);
        }
        private void OnDestroy()
        {
            Events.OnBuyReady -= OnBuyReady;
        }
        void OnBuyReady()
        {
            if (state == states.SELL)
            {
                Refresh();
                Events.OnLoadingPanel(false);
            }
        }
        public void OnSelectCharacter(CharactersData.CharacterData cData)
        {
            int price = GetPrice(cData);
            if (state == MarketplaceUI.states.SELL)
            {
                if (cData.isGoalkeeper && DB.DBManager.Instance.DbUserData.data.players_goalkeepers.Count <= 1)
                {
                    Events.OnPopup(Data.Instance.texts.Get("marketplace_condition_goalkeeper"), null);
                    return;
                }
                else if (DB.DBManager.Instance.DbUserData.data.players_characters.Count <= 10)
                {
                    Events.OnPopup(Data.Instance.texts.Get("marketplace_condition_players"), null);
                    return;
                }
            }
            marketplacePopupUI.Init( cData, price);
        }
        void OnBackToScreen()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }

        void Refresh()
        {
            arrayID = 0;
            if (state == states.BUY)
                AddCharacters();
            else
                AddMyTeamCharacters();
        }
        void AddCharacters()
        {
            buttons = new List<MarketplaceCharacterButton>();
            Utils.RemoveAllChildsIn(container);

            allCharacters = CharactersData.Instance.GetAvailablePlayers(tab == tabs.GOALKEEPERS);

            InitCascade();
            foreach (CharactersData.CharacterData cData in allCharacters)
                AddButton(cData);
            StartCascade();
        }
        void AddMyTeamCharacters()
        {
            buttons = new List<MarketplaceCharacterButton>();
            Utils.RemoveAllChildsIn(container);
            allMyCharacters = DB.DBManager.Instance.DbUserData.data.GetCharacters(tab == tabs.GOALKEEPERS);

            InitCascade();
            foreach (DB.DBUserData.DBCharacterData dbData in allMyCharacters)
            {               
                CharactersData.CharacterData cData = CharactersData.Instance.GetCharacterData(dbData.player_id, tab == tabs.GOALKEEPERS);
                cData.SetDataFromDB(dbData);
                AddButton(cData, dbData);
            }
            StartCascade();
        }
        void AddButton(CharactersData.CharacterData cData, DB.DBUserData.DBCharacterData dbData = null)
        {
             if (filterByRarity == true && cData.rarity != rarity) return;
            arrayID++;           
            if (arrayID-1 < from || arrayID-1 >= to) return;            

            MarketplaceCharacterButton button = Instantiate(marketplaceCharacterButton, container);
            button.Init(buttons.Count, OnCharacterCardClicked);
            int price = GetPrice(cData);
            button.SetData(cData, state, price, dbData);
            bool available = true;

            if(state == states.BUY)
                available = price <= DB.DBManager.Instance.DbUserData.data.score;

            button.SetAvailable(available);
            button.transform.localScale = Vector2.one;
            buttons.Add(button);

            AddToCascade(button);
        }
        int GetPrice(CharactersData.CharacterData cData)
        {
            if(state == states.BUY)
                return Data.Instance.marketplaceData.GetPriceFor(cData.rarity);
            else
            {
                DB.DBUserData.DBCharacterData uData=  DB.DBManager.Instance.DbUserData.data.GetPlayerByID(cData.uniqueID);
                if (uData == null)
                    Debug.LogError("No player in DB for: " + cData.uniqueID);
                return uData.sell_price;
            }
        }
        void OnCharacterCardClicked(int id)
        {
            if (state == states.SELL)
                buttons[id].data.SetDataFromDB(buttons[id].dbData);
            OnSelectCharacter(buttons[id].data);
        }
        public void Back()
        {
            marketplacePopupUI.Close();
            anim.Play("out");
            Events.Back();
            Invoke("Reset", 0.5f);
        }
        void OnFilter(int id)
        {
            foreach (ButtonCustom button in filterButtons)
                button.OnSelected(false);
            filterButtons[id].OnSelected(true);

            switch (id)
            {
                case 0:
                    tab = tabs.PLAYERS;
                    break;
                case 1:
                    tab = tabs.GOALKEEPERS;
                    break;
                case 2:
                    //AddCharacters(true);
                    break;
            }
            //Refresh();
            // InitPaginator(); 
            ResetFilterRarity();
        }
        void ResetFilterRarity()
        {
            foreach (ButtonCustom button in filterRarityButtons)
            {
                if (button.isSelected)
                {
                    OnFilterRarity(button.buttonID);
                    return;
                }
            }
            Refresh();
            InitPaginator();
        }
        void OnFilterRarity(int id)
        {
            print("OnFilterRarity " + id + "   rarity: " + rarity + "    (FigusData.rarities)id: " + (FigusData.rarities)(id+1));
            foreach (ButtonCustom button in filterRarityButtons)
                button.OnSelected(false);

            if (filterByRarity != false && (int)rarity == (id+1))
                filterByRarity = false;
            else
            {
                filterByRarity = true;
                filterRarityButtons[id].OnSelected(true);
            }
            rarity = (FigusData.rarities)(id+1);

            Refresh();
            InitPaginator();
        }
       
        void OnStateButtonClick(int buttonID)
        {
            switch(buttonID)
            {
                case 1:
                    if (state == states.BUY) return;
                    state = states.BUY;
                    break;
                case 2:
                    if (state == states.SELL) return;
                    state = states.SELL;
                    break;
            }
            ResetFilterRarity();
            foreach (ButtonCustom b in topButtons) b.OnSelected(false);
            foreach (Image bg in backgrounds) bg.enabled = false;
            foreach (GameObject bg in backgroundsPanels) bg.SetActive(false);
            topButtons[buttonID - 1].OnSelected(true);
            backgrounds[buttonID-1].enabled = true;
            backgroundsPanels[buttonID-1].SetActive(true);

            //Refresh();
            InitPaginator();
        }
        //Paginator:
        void InitPaginator()
        {
            //print("totalButtons " + arrayID);
            paginatorManager.Init(arrayID, totalButtonsInPage, OnPaginatorClicked);
            SetFeedbackOn(buttons.Count);
        }
        void OnPaginatorClicked(int from, int to)
        {
            this.from = from;
            this.to = to;
            Refresh();
        }
        private void Reset()
        {
            gameObject.SetActive(false);
        }
        void SetFeedbackOn(int buttonsQTY)
        {
            feedback.SetActive(buttonsQTY == 0);
        }

    }

}