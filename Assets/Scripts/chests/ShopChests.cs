using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Fulbo.UI.Shop
{
    public class ShopChests : Fulbo.UI.Shop.ShopPanel
    {
        [SerializeField] GameObject[] icons;
        [SerializeField] Text title;
        [SerializeField] Transform container;
        ChestPackButton[] buttons;

        public void Init()
        {
            title.text = Data.Instance.texts.Get("shop_chests_packs");
            int _id = 0;
            buttons = container.GetComponentsInChildren<ChestPackButton>();
            foreach (ChestPackButton c in buttons)
            {
                string text = Data.Instance.texts.Get("PlayerPacks_" + (_id+1) + "_title");    
                c.Init(_id, Clicked, text);
                c.InitPack(icons[_id]);
                _id++;
            }
        }
        void Clicked(int id)
        {
            print(id);
            Events.OnLoadingPanel(true);
            Invoke("ResetLoading", 2);

            MatchData.ResponseFromServer.ChestDataFromDB chestData = Data.Instance.matchData.response.chestData;

            Events.OpenChest(id, ResetLoading, chestData);
        }
        void ResetLoading()
        {
            Events.OnLoadingPanel(false);
        }
    }
}
