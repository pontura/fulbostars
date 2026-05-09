using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using static Fulbo.PricesData;

namespace Fulbo.UI.Shop
{
    public class PlayersPacks : Fulbo.UI.Shop.ShopPanel
    {
        [SerializeField] GameObject[] icons;
        [SerializeField] Text title;
        [SerializeField] Transform container;
        PlayersPackButton[] buttons;

        public void Init()
        {
            title.text = Data.Instance.texts.Get("PlayerPacks_title");
            int _id = 0;
            buttons = container.GetComponentsInChildren<PlayersPackButton>();
            foreach (PlayersPackButton c in buttons)
            {
                string text = Data.Instance.texts.Get("PlayerPacks_" + (_id + 1) + "_title");
                c.Init(_id, Clicked, text);

                List<QtySoftHard> all = new List<QtySoftHard>();                

                int id = _id + 1;
                for (int a = 0; a < 20; a++)
                {
                    //SimpleJSON.JSONNode json =  Data.Instance.pricesData.jsonNode["players"]["packs"]["pack" + id]["q" + a];
                    SimpleJSON.JSONNode json = Data.Instance.pricesData.GetPlayerPacks("pack" + id, "q" + a);
                    if (json != null)
                    {
                        QtySoftHard data = new QtySoftHard();
                        data.qty = a;
                        data.soft = json["soft"];
                        data.hard = json["hard"];
                        data.hardRegular = json["hardRegular"];
                        data.hardOnSalePercentage = json["hardOnSalePercentage"];
                        all.Add(data);
                    }
                }               
                c.InitPack(id, icons[_id], all);
                
                _id++;
            }
        }
        public void SetActive()
        {
            foreach (PlayersPackButton c in buttons)
                c.OnRestart();
        }
        void Clicked(int id)
        {
            
        }
        void ResetLoading()
        {
            Events.OnLoadingPanel(false);
        }
    }
}
