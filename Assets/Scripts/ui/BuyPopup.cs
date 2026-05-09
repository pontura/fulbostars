using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class BuyPopup : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Image icon;
        [SerializeField] Sprite soft;
        [SerializeField] Sprite hard;

        [SerializeField] GameObject panel_popup;

        [SerializeField] Text title;
        [SerializeField] Text priceField;
        [SerializeField] Text buyField;
        [SerializeField] Text cancelField;
        [SerializeField] ButtonCustom closeButton;
        [SerializeField] ButtonCustom confirmButton;
        [SerializeField] ButtonCustom cancelButton;

        System.Action<bool> OnDone;
        float price;

        void Start()
        {
            SetActive(false);
            Events.ConfirmBuy += ConfirmBuy;
            Events.OnBuyReady += OnBuyReady;
            closeButton.Init(0, Close);
            closeButton.SetType(ButtonCustom.types.CLOSE);
            confirmButton.Init(0, Buy, Data.Instance.texts.Get("yes"));
            cancelButton.Init(1, Buy, Data.Instance.texts.Get("cancel"));
        }
        void OnDestroy()
        {
            Events.ConfirmBuy -= ConfirmBuy;
            Events.OnBuyReady -= OnBuyReady;
        }
        void SetActive(bool isOn)
        {
            panel.SetActive(isOn);
        }
        void ConfirmBuy(float price, System.Action<bool> OnDone, string currency = "soft")
        {
            if (currency == "soft")    icon.sprite = soft;
            else                       icon.sprite = hard;

            this.price = price;
            this.OnDone = OnDone;
            SetActive(true);
            SetContent();
            panel_popup.SetActive(true);
            Events.OnLoadingPanel(false);
        }
        void SetContent()
        {
            title.text = "Confirm?";
            priceField.text = Utils.FormatNumbers((int)price, false);
            buyField.text = Data.Instance.texts.Get("yes");
            cancelField.text = Data.Instance.texts.Get("cancel");
        }
        public void Buy(int i)
        {
            panel.SetActive(false);

            if (i == 1)//cancel;
                OnDone(false);
            else
            {
                Events.OnLoadingPanel(true);
                Invoke("DelayedDone", 0.25f);
            }
        }
        void DelayedDone()
        {
            OnDone(true);
        }
        public void Cancel(int i)
        {
            OnDone(false);
            SetActive(false);
        }
        void OnBuyReady()
        {
            SetActive(false);
        }
        void Close(int id)
        {
            SetActive(false);
        }
    }
}