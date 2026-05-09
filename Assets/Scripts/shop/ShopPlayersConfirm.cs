using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.PricesData;

namespace Fulbo.UI.Shop
{
    public class ShopPlayersConfirm : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Text field;
        [SerializeField] Text playersQtyField;
        [SerializeField] Text priceField;
        [SerializeField] Text title;
        [SerializeField] ButtonCustom confirm;
        [SerializeField] ButtonCustom cancel;
        [SerializeField] GameObject[] icons;

        [SerializeField] GameObject hardIcon;
        [SerializeField] GameObject softIcon;

        System.Action<bool> OnDone;

        void Start()
        {
            Events.ConfirmBuyPlayers += ConfirmBuyPlayers;
            Close();
            confirm.Init(0, Clicked);
            cancel.Init(1, Clicked);
        }
        void OnDestroy()
        {
            Events.ConfirmBuyPlayers -= ConfirmBuyPlayers;
        }
        void ConfirmBuyPlayers(QtySoftHard data, int id, System.Action<bool> OnDone)
        {
            print(id);
            this.OnDone = OnDone;
            panel.SetActive(true);
            confirm.SetText(Data.Instance.texts.Get("confirm"));
            cancel.SetText(Data.Instance.texts.Get("cancel"));
            field.text = Data.Instance.texts.Get("PlayerPacks_" + id + "_desc");
            playersQtyField.text = "";// data.qty + " " + Data.Instance.texts.Get("players");
            title.text = data.qty + " PLAYER " + Data.Instance.texts.Get("PlayerPacks_" + id + "_title");

            //icons
            foreach (GameObject go in icons)
                go.SetActive(false);
            icons[id - 1].SetActive(true);

            int price = 0;
            hardIcon.SetActive(false);
            softIcon.SetActive(false);

            if (data.hard == 0)
            {
                softIcon.SetActive(true);
                price = data.soft;
            }
            else
            {
                hardIcon.SetActive(true);
                price = data.hard;
            }

            priceField.text = price.ToString();
        }
        void Close()
        {
            panel.SetActive(false);
        }
        void Clicked(int id)
        {
            if (id == 0)
                OnDone(true);
            else
                OnDone(false);
            Close();
        }
    }
}