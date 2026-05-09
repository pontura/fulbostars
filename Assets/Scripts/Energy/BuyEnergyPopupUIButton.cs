using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Energy.UI
{
    public class BuyEnergyPopupUIButton : ButtonCustom
    {
        [SerializeField] Text title;
        [SerializeField] Text subtitle;
        [SerializeField] Text buttonField;
        // id 1 = video;
        // id 2 = buy;
        public void InitData(int id)
        {
            string _title; string _text;
            _title = Data.Instance.texts.Get("buyEnergyTitle_" + id);
            _text = Data.Instance.texts.Get("buyEnergyText_" + id);
            if(title != null)   title.text = _title;
            if (subtitle != null) subtitle.text = _text;           
        }
        public void NotAvailable()
        {
            if (buttonField != null) buttonField.text = Data.Instance.texts.Get("videos_not_available");
        }
        public void SetSeen(int totalSeen, int totalAvailables)
        {
            if (buttonField != null) buttonField.text = totalSeen + "/" + totalAvailables;
        }
        public void SetPrice(int price)
        {
           // print("price: " + price);
            if (price > 0)
            {
                if (buttonField != null) buttonField.text = "$" + price;
            }
            else
            {
                if (title != null) title.text = Data.Instance.texts.Get("buy_energy_error");
                buttonField.text = "$1000";
            }
        }
    }
}