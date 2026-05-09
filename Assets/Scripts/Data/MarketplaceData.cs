using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;

namespace Fulbo
{
    public class MarketplaceData : MonoBehaviour
    {
        public PricesData[] prices;
        [Serializable]
        public class PricesData
        {
            public FigusData.rarities rarity;
            public int price;
        }
        public int GetPriceFor(FigusData.rarities rarity)
        {
            //SimpleJSON.JSONNode s = Data.Instance.pricesData.jsonNode["players"]["marketplace"];
            //switch (rarity)
            //{
            //    case FigusData.rarities.normal:  return s[0];
            //    case FigusData.rarities.rare: return s[1];
            //    case FigusData.rarities.epic: return s[2];
            //    case FigusData.rarities.MASTER: return s[3]; 
            //    default: return s[3]; 
            //}
            return 0;
        }
    }
}
