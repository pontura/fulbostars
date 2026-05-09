using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo;

namespace Fulbo.UI.Marketplace
{
    public class MarketplaceCharacterButton : ButtonCascade
    {
        [SerializeField] Text totalStats;
        [SerializeField] GameObject totalStatsGO;
        [SerializeField] Image thumb;
        [SerializeField] Image rarityLine;
        [SerializeField] Image priceBG;
        [SerializeField] Color colorUnavailable;
        [SerializeField] Color colorUnavailablePriceBG;
        [SerializeField] Color textUnabailableColor;
        
        [SerializeField] Color colorForSell;
        [SerializeField] Color colorForBuy;

        [SerializeField] PriceAsset priceAsset;

        [SerializeField] GameObject[] hideOnUnavailable;
        [SerializeField] GameObject[] rarityAssets;
        [SerializeField] GameObject[] rarityAssets2;

        public CharactersData.CharacterData data;
        public DB.DBUserData.DBCharacterData dbData;

        public void SetData(CharactersData.CharacterData data, MarketplaceUI.states state, int price, DB.DBUserData.DBCharacterData dbData = null)
        {   
            this.data = data;
            this.dbData = dbData;

            thumb.sprite = data.thumb;
            TextsData.CharacterData textData = Data.Instance.textsData.GetCharactersData(data.id, data.isGoalkeeper);

            if (textData != null)
                SetTexts(field, data.avatarName);


            if (state == MarketplaceUI.states.BUY)
            {             
                totalStatsGO.SetActive(false);
                priceAsset.Init((int)price, true, "hard");
            }
            else
            {
                priceAsset.Init((int)price, false);
                SetTexts(totalStats, dbData.GetTotalStats().ToString());
            }
            print("rar________________________" + data.rarity);
            rarityLine.color = Data.Instance.settings.GetRaritySettingFor(data.rarity).color;

            foreach (GameObject go in rarityAssets2)
                go.SetActive(false);
            rarityAssets2[((int)data.rarity)-1].SetActive(true);

            foreach (GameObject go in rarityAssets)
                go.SetActive(false);
            rarityAssets[((int)data.rarity-1)].SetActive(true);
        }
        void SetTexts(Text field, string text)
        {
            if(field != null) field.text = text;
        }
        public void SetAvailable(bool isAvailable)
        {
            if(isAvailable)
            {
               // thumb.color = Color.white;
                priceAsset.SetColorForText(Color.white);
                foreach (GameObject go in hideOnUnavailable)
                    go.SetActive(true);

            }
            else
            {
                priceBG.color = colorUnavailablePriceBG;
                priceAsset.SetColorForText(textUnabailableColor);
               // thumb.color = colorUnavailable;
                foreach (GameObject go in hideOnUnavailable)
                    go.SetActive(false);
            }
        }
    }
}