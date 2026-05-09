using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class MyTeamCharacterData : MonoBehaviour
    {
        DBUserData.DBCharacterData dbCharacterData;
        [SerializeField] ButtonCustom tierButton;
        [SerializeField] Text tierField;
        [SerializeField] Text levelField;
        [SerializeField] Text xpProgressField;
        [SerializeField] Text pointstTitleField;
        [SerializeField] Text pointsField;
        [SerializeField] Text rarityField;
        [SerializeField] Image xpProgressBar;
        [SerializeField] GameObject pointsPanel;
        [SerializeField] MyTeamUI myTeamUI;
        float _width = -351;
        DBUserData.DBCharacterData uData;
        int tier;

        private void Start()
        {
            tierButton.Init(0, Clicked, Data.Instance.texts.Get("upgrade"));            
        }
        void Clicked(int id)
        {
            if(id == 0)
            {
                Events.ConfirmTierUpgrade(uData, OnTierDone);
            }
        }
        void OnTierDone(bool isOk)
        {
            if(isOk)
            {
                DB.DBManager.Instance.DbTier.Save(uData, OnTierSaved);
                Events.OnLoadingPanel(true);
            }
        }
        void OnTierSaved(bool isOk, string response)
        {
            Events.OnLoadingPanel(false);
            if(isOk)
            {
                Events.OnLoadingPanel(true);                
                DB.DBManager.Instance.DbUserData.LoadUserData(UpdateUserData);
            }
            else
            {
                Events.OnPopup(response, null);
            }
        }

        void UpdateUserData() {
            uData = DB.DBManager.Instance.DbUserData.data.GetPlayerByID(uData.id);
            tier = uData.tier;
            CharacterButton chB = myTeamUI.cards.Find(x => x.dbCharacterData.id == uData.id);
            if (chB != null)
                chB.dbCharacterData = uData;
            SetTierField();
            levelField.text = "LEVEL " + uData.level + "/" + (tier * 10);
            Events.OnLoadingPanel(false);
        }

        public void RefreshData(DBUserData.DBCharacterData uData)
        {
            if (uData == null || uData.role == "") return;
            this.uData = uData;

            tier = uData.tier;
            SetTierField();

            xpProgressField.text = Data.Instance.myTeam.GetCharacterLevelProgress(uData);
            levelField.text = "" + uData.level;
            rarityField.text = ((FigusData.rarities)(uData.rarity)).ToString().ToUpper();

            if (uData.available_stats>0)
            {
                pointsPanel.SetActive(true);
                pointstTitleField.text = Data.Instance.texts.Get("points");
                pointsField.text = uData.available_stats.ToString();
            }
            else
                pointsPanel.SetActive(false);

            float levelProgressValue = Data.Instance.myTeam.GetCharacterLevelProgressValue(uData);
           // print("levelProgressValue" + levelProgressValue);
            xpProgressBar.fillAmount = levelProgressValue;
        }
        void SetTierField()
        {
            tierField.text = "TIER " + tier;
        }
    }

}