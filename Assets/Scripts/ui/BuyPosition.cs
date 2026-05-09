using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class BuyPosition : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] GameObject panel;
        [SerializeField] ButtonCustom[] positions;
        [SerializeField] ButtonCustom closeButton;

        float price;
        System.Action<int> OnDone;

        int characterID;
        int positionID;

        void Start()
        {
            SetActive(false);
            Events.InitBuyPosition += InitBuyPosition;
            closeButton.Init(0, Close);
            closeButton.SetType(ButtonCustom.types.CLOSE);
        }
        void OnDestroy()
        {
            Events.InitBuyPosition -= InitBuyPosition;
        }
        void SetActive(bool isOn)
        {
            panel.SetActive(isOn);
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

                button.Init(id, Buy, positionText);
                id++;
            }
        }
       
        void InitBuyPosition(int characterID, int positionID, float price, System.Action<int> OnDone)
        {
            print("InitBuyPosition " + characterID + " positionID: " + positionID);
            SetActive(true);
            SetPositions();
            this.characterID = characterID;
            SetPositionButtons(positionID);
            this.OnDone = OnDone;
            this.price = price;
            SetContent();
        }
        void SetPositionButtons(int buttonID)
        {
            foreach (ButtonCustom button in positions)
                button.OnSelected(false);
            positions[buttonID].OnSelected(true);

            this.positionID = buttonID;
        }

        void SetContent()
        {
            title.text = Data.Instance.texts.Get("buy_position");
        }
        public void Buy(int positionID)
        {
            this.positionID = positionID;
            Events.ConfirmBuy(price, OnConfirm, "soft");
            SetActive(false);
        }
        void OnConfirm(bool doIt)
        {
            if (doIt)
            {
                DB.DBEvents.OnChangeCharacterPosition(characterID, positionID, OnBuyDone);
            }
        }
        void OnBuyDone(bool buy, string callbackText) // update score when position is ready 
        {
            Events.OnLoadingPanel(false);
            DB.DBEvents.LoadUserData(OnUserLoaded);           
        }
        void OnUserLoaded()
        {
            Events.OnBuyReady();
            Events.RefreshScore(DB.DBManager.Instance.DbUserData.data.score);
            OnDone(positionID);
            Cancel();
        }
        public void Cancel()
        {
            Events.OnButtonPressed(ButtonCustom.types.CLOSE);
            Close(0);
        }
        void Close(int id)
        {
            SetActive(false);
        }
    }
}