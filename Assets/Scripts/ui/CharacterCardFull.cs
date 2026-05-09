using Fulbo.DB;
using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CharacterCardFull : MonoBehaviour
    {
        public int characterID;
        public bool isGoalKeeper;
        public CharacterForCamera characterForCamera;
        public CharacterButton characterButton;
        System.Action OnClose;
        public int totalConsumables;
        DBUserData.DBCharacterData dbCharacterData;
        [SerializeField] ButtonCustom positionButton;
        [SerializeField] GameObject editIcon;
        CharactersData.CharacterData cData;
        [SerializeField] Sprite[] positionForSaleSprites;
        [SerializeField] Image positionForSaleIcon;

        public void OpenCharacterFullCard(DBUserData.DBCharacterData dbCharacterData, System.Action OnClose)
        {
            //panel.SetActive(true);

            this.dbCharacterData = dbCharacterData;
            this.OnClose = OnClose;
            cData = CharactersData.Instance.GetCharacterData(dbCharacterData.player_id, dbCharacterData.IsGoalkeeper());
            OpenCharacter(cData);
            characterButton.fullCard = this;
            positionButton.Init(0, OpenBuyPosition);
            RefreshCharacter();
            SetPosition();
        }
        public void OpenCharacter(CharactersData.CharacterData cData)
        {
            characterForCamera.Init(cData.asset, "run");
            characterForCamera.SetCamera(true);
        }
        void RefreshCharacter()
        {
            characterButton.OnInit(dbCharacterData, dbCharacterData.IsGoalkeeper(), true);
        }
        void SetPosition()
        {
            string positionName = Data.Instance.textsData.GetPositionName(cData, true);
            positionButton.SetText(positionName);

            if (dbCharacterData.IsGoalkeeper())
            {
                if(editIcon)
                    editIcon.SetActive(false);
                positionButton.GetComponent<Button>().interactable = false;
                positionForSaleIcon.sprite = positionForSaleSprites[3]; // GK
            }
            else
            {
                positionButton.GetComponent<Button>().interactable = true;
                if (editIcon)
                    editIcon.SetActive(true);
                if (positionForSaleIcon != null)
                {
                    int originalTypeIDByPosition = Data.Instance.myTeam.GetCharacterType(cData.uniqueID);
                    positionForSaleIcon.sprite = positionForSaleSprites[originalTypeIDByPosition];
                }

            }
        }
        void OpenBuyPosition(int id)
        {
            int price = 1000;
            int pos = dbCharacterData.position;

            if (DB.DBManager.Instance.DbUserData.data.Score() < price)
                Events.OnPopup(Data.Instance.texts.Get("not_enough_money") + " " + price, null);
            else
                Events.InitBuyPosition(dbCharacterData.id, pos, price, OnDone);
        }
        void OnDone(int newPos)
        {
            print("On don2: " + newPos);
            //int pos = Data.Instance.myTeam.GetCharacterType(dbCharacterData.id);
            //if (pos != newPos)
            //{
                //Data.Instance.myTeam.UpdateCharacterPosition(dbCharacterData.id, newPos);
                string positionName = Data.Instance.textsData.GetPositionString(newPos, true);
                positionButton.SetText(positionName);
                positionForSaleIcon.sprite = positionForSaleSprites[newPos];
                Events.CharacterUpdatedData(dbCharacterData);
                DB.DBManager.Instance.DbUserData.LoadUserData(null);
               // Events.AddScore(-100);
           // }
        }
    }
}