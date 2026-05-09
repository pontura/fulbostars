using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Marketplace
{
    public class MarketplacePopupUI : MonoBehaviour
    {
      

        [SerializeField] GameObject panel;
        [SerializeField] Text nameField;
        [SerializeField] Text descField;
        [SerializeField] PriceAsset priceAsset;
        [SerializeField] Text subField;
        [SerializeField] Text positionsTitleField;
        [SerializeField] Text rarityField;
        [SerializeField] GameObject positionsContainer;
        [SerializeField] GameObject statsContainer;
        [SerializeField] ButtonCustom[] positions;
        [SerializeField] GameObject[] rarityColorizables;
        [SerializeField] GameObject[] rarityAssets;
        [SerializeField] GameObject[] rarityAssets2;
        [SerializeField] GameObject[] rarityAssetCard;
        [SerializeField] ButtonCustom close;
        [SerializeField] ButtonCustom submit;
        [SerializeField] CardAsset cardAsset;

        //
        [SerializeField] GameObject positionForSalePanel;
        [SerializeField] Text positionForSaleTextfield;
        [SerializeField] Image positionForSaleIcon;
        [SerializeField] Sprite[] positionForSaleSprites;

        MarketplaceUI marketplaceUI;
        Character.types type;
        int price;
        [SerializeField]    
        CharactersData.CharacterData data;

        private void Start()
        {
            marketplaceUI = GetComponent<MarketplaceUI>();
            Close();
        }
        public void Init(CharactersData.CharacterData data, int price)
        {
            SetTexts(rarityField, Data.Instance.texts.Get("rarity_" + (int)(data.rarity-1)) );
            this.price = price;
            this.data = data;
            Open();
            cardAsset.Init(data);
            cardAsset.HidePanels(false, false, false);
            TextsData.CharacterData textData = Data.Instance.textsData.GetCharactersData(data.id, data.isGoalkeeper);

            if (textData != null)
                SetTexts(nameField, data.avatarName.ToUpper());

            SetTexts(descField, data.text);

            string submitTitle;
            if (marketplaceUI.state == MarketplaceUI.states.BUY)
            {
                positionForSalePanel.SetActive(false);
                priceAsset.Init(price, true, "hard");
               // Events.OnboardingCheckStep(Onboarding.OnboardingPanel.panels.marketplace, 5, null);
                submitTitle = Data.Instance.texts.Get("buy");

                statsContainer.SetActive(false);

                if (data.isGoalkeeper)
                {
                    positionsContainer.SetActive(false);
                    positionsTitleField.text = Data.Instance.texts.Get("position_goalkeeper_full");
                }
                else
                {
                    positionsTitleField.text = Data.Instance.texts.Get("select_position");
                    positionsContainer.SetActive(true);
                    SetPositions();
                }
            }
            else
            {
                ShowPositionForSale();
                positionsTitleField.text = "";
                priceAsset.Init(price, false);
                subField.text = Data.Instance.texts.Get("total_stats") + ":" + data.GetTotalStats(false).ToString();
                submitTitle = Data.Instance.texts.Get("sell");
                positionsContainer.SetActive(false);
                SetStats(data);
            }

            submit.Init(0, Submit, submitTitle);
            close.Init(0, Close);
            SetRarity(data.rarity);
        }
        void ShowPositionForSale()
        {
            if (data.isGoalkeeper)
                positionForSalePanel.SetActive(false);
            else { 
                positionForSalePanel.SetActive(true);
                positionForSaleTextfield.text = Data.Instance.textsData.GetPositionName(data, true);
                int originalTypeIDByPosition = Data.Instance.myTeam.GetCharacterType(data.uniqueID);
                positionForSaleIcon.sprite = positionForSaleSprites[originalTypeIDByPosition];
            }
        }
        void SetRarity(FigusData.rarities rarity)
        {
            Color color = Data.Instance.settings.GetRaritySettingFor((rarity-1)).color;
            foreach(GameObject go in rarityColorizables)
            {
                if (go.GetComponent<Image>()) go.GetComponent<Image>().color = color;
                if (go.GetComponent<Text>()) go.GetComponent<Text>().color = color;
            }
            string title = Data.Instance.texts.Get("rarity_range_title");
            int value1 = 0; int value2 = 0;
            if (marketplaceUI.state == MarketplaceUI.states.BUY)
            {
                //TO-DO : estos datos no estan en una database:
                switch (rarity)
                {
                    case FigusData.rarities.normal:
                        value1 = 5;  value2 = 15; break;
                    case FigusData.rarities.rare:
                        value1 = 16; value2 = 32; break;
                    case FigusData.rarities.epic:
                        value1 = 33; value2 = 55; break;
                    case FigusData.rarities.galaxy:
                        value1 = 56; value2 = 90; break;
                }
                subField.text = title + " = " + value1 + "/" + value2;
                SetStatsForBuy(data, value1, value2);
            }
            foreach (GameObject go in rarityAssetCard)
                go.SetActive(false);
            rarityAssetCard[(int)(data.rarity)-1].SetActive(true);

            foreach (GameObject go in rarityAssets)
                go.SetActive(false);
            rarityAssets[(int)(data.rarity)-1].SetActive(true);

            foreach (GameObject go in rarityAssets2)
                go.SetActive(false);
            rarityAssets2[(int)(data.rarity)-1].SetActive(true);
        }
        void SetPositions()
        {
            int id = 0;
            foreach (ButtonCustom button in positions)
            {
                string positionText;
                if (id == 0) positionText = Data.Instance.texts.Get("position_def_full");
                else if (id == 1) positionText = Data.Instance.texts.Get("position_mid_full");
                else positionText = Data.Instance.texts.Get("position_for_full");

                button.Init(id, SetPositionButtons, positionText);
                id++;
            }
        }
        void SetStats(CharactersData.CharacterData data)
        {
            statsContainer.SetActive(true);
            int id = 0;
            foreach (CharacterStatLine newStatLine in statsContainer.GetComponentsInChildren<CharacterStatLine>())
            {
                string name = data.stats.GetStatName(id);
                int value = data.stats.GetStatByName(id);
                newStatLine.Init(name, value, 0, id, Data.Instance.settings.statsSettings[id]);
                newStatLine.gameObject.SetActive(true);
                id++;
            }
        }
        void SetStatsForBuy(CharactersData.CharacterData data, int value1, int value2)
        {
            statsContainer.SetActive(true);
            int id = 0;
            foreach (CharacterStatLine newStatLine in statsContainer.GetComponentsInChildren<CharacterStatLine>())
            {
                string name = data.stats.GetStatName(id);
                newStatLine.Init(name, value2, 0, id, Data.Instance.settings.statsSettings[id]);
                newStatLine.InitSecondBar(value1);
                newStatLine.gameObject.SetActive(true);
                id++;
            }
        }
        public void Open()
        {
            panel.SetActive(true);
        }
        public void Close() { Close(0); }
        public void Close(int id = 0)
        {
            cardAsset.OnClose();
            panel.SetActive(false);
        }
        public void Submit(int id = 0)
        {
            if(marketplaceUI.state == MarketplaceUI.states.BUY)
            {
                if (price > DB.DBManager.Instance.DbUserData.data.hard_currency)
                {

                    Events.OnPopup(Data.Instance.texts.Get("not_money_buy"), null);
                    return;
                }
                if (!data.isGoalkeeper)
                {
                    bool selectedPos = false;
                    foreach (ButtonCustom button in positions)
                    {
                        if (button.isSelected) selectedPos = true;
                    }
                    if (!selectedPos)
                    {
                        Events.OnPopup(Data.Instance.texts.Get("selectPosition"), null);
                        return;
                    }
                }
                Events.ConfirmBuy(price, OnConfirm, "hard");
            } else
                Events.ConfirmBuy(price, OnConfirm, "soft");
        }
        Dictionary<string, object> param;
        void OnConfirm(bool doIt)
        {
            if (doIt)
            {
                if (marketplaceUI.state == MarketplaceUI.states.BUY)
                {
                    param = new Dictionary<string, object>();
                    if(data.isGoalkeeper)
                        param["role"] = "GOALKEEPER";
                    else
                        param["role"] = "PLAYER";
                    param["characterName"] = data.avatarName; //Más intuitivo de leer que pasar el número del ID
                    param["price"] = price;
                    param["rarity"] = data.rarity;             
                    DB.DBManager.Instance.DbUserData.BuyPlayer(data.id, data.isGoalkeeper, GetPositionSelected(), OnConfirmDone);
                }
                else
                    Sell();
            } else Close();
        }
        void Sell()
        {
            DB.DBUserData.DBCharacterData uData = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(data.uniqueID);
            DB.DBManager.Instance.DbUserData.SellPlayer(uData, OnConfirmDone);
        }
        void OnConfirmDone(bool isOk, string text)
        {           
            if (isOk)
            {
                DB.DBEvents.LoadUserData(CharactersUpdated);
                Events.OnTrack("CharacterBought", param);
            }
            else
            {
                Close();
                Events.OnBuyReady();
            }
        }
        void CharactersUpdated()
        {
            Close();
            if (marketplaceUI.state == MarketplaceUI.states.BUY)
            {
                DB.DBUserData.DBCharacterData uData = DB.DBManager.Instance.DbUserData.GetLastCharacterBought();
                if (uData != null) GetComponent<NewCharacterUI>().Init(uData);
            }
            else if (marketplaceUI.state == MarketplaceUI.states.SELL)
                DB.DBManager.Instance.DbUserData.data.gameData.OnCharacterSold(data.uniqueID);

            Events.OnBuyReady();
            
        }
        void SetTexts(Text field, string text)
        {
            if (field != null) field.text = text;
        }
        int GetPositionSelected()
        {
            int id = 0;
            foreach (ButtonCustom button in positions)
            {
                if (button.isSelected)
                    return id;
                id++;
            }
            return 0;
        }
        void SetPositionButtons(int buttonID)
        {
            foreach (ButtonCustom button in positions)
                button.OnSelected(false);
            positions[buttonID].OnSelected(true);

            if (buttonID == 0) type = Character.types.DEF;
            if (buttonID == 1) type = Character.types.MID;
            if (buttonID == 2) type = Character.types.FOR;
        }
    }
}
