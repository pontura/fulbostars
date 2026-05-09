using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class AccountPopup : MonoBehaviour
    {
        [SerializeField] GameObject panel;

        [SerializeField] GameObject discordWarning;
        [SerializeField] Text discordWarningTextField;

        [SerializeField] Text discordTitleField;
        [SerializeField] Text twitterTitleField;

        [SerializeField] ButtonCustom saveButton;
        [SerializeField] ButtonCustom myDataButton;
        [SerializeField] ButtonCustom deleteAccount;

        [SerializeField] InputField discord;
        [SerializeField] InputField twitter;

        [SerializeField] ButtonCustom closeButton;

        void Start()
        {
            Events.OpenAccountSettings += OpenAccountSettings;
            panel.SetActive(false);
            closeButton.SetType(ButtonCustom.types.CLOSE);
        }
        void OnDestroy()
        {
            Events.OpenAccountSettings -= OpenAccountSettings;
        }
        void OpenAccountSettings()
        {
            Open();
        }
        public void Open()
        {
            discordWarning.SetActive(false);
            discordTitleField.text = Data.Instance.texts.Get("discord");
            twitterTitleField.text = Data.Instance.texts.Get("twitter");
            panel.SetActive(true);

            if (DB.DBManager.Instance.DbUserData.data.discord != "")
                discord.text = DB.DBManager.Instance.DbUserData.data.discord;
            else
            {
                discordWarningTextField.text = Data.Instance.texts.Get("discordWarningText");
                discordWarning.SetActive(true);
                discord.placeholder.GetComponent<Text>().text = Data.Instance.texts.Get("enterText");
            }

            if (DB.DBManager.Instance.DbUserData.data.twitter != "")
                twitter.text = DB.DBManager.Instance.DbUserData.data.twitter;
            else
                twitter.placeholder.GetComponent<Text>().text = Data.Instance.texts.Get("enterText");

            closeButton.Init(0, Clicked);
            saveButton.Init(1, Clicked, Data.Instance.texts.Get("save"));
            myDataButton.Init(2, Clicked, Data.Instance.texts.Get("myDataButton"));

            if (DB.DBManager.Instance.versionMode == DBManager.versionModes.DEV)
            {
                deleteAccount.gameObject.SetActive(true);
                deleteAccount.Init(3, Clicked, Data.Instance.texts.Get("delete_account"));
            } else
                deleteAccount.gameObject.SetActive(false);
        }
        void Clicked(int id)
        {
            switch (id)
            {
                case 0: Close(); break;
                case 1: Save(); break;
                case 2: MyData(); break;
                case 3: OnAccountDeleteClicked(); break;
            }
        }
        public void Close()
        {
            panel.SetActive(false);
            Time.timeScale = 1;
        }
        void Save()
        {
            saveButton.SetInteraction(false);

            DBRegisterTeam.RData d = new DBRegisterTeam.RData();
            d.user = DBManager.Instance.DbUserData.data.user;
            d.email = DBManager.Instance.Email;
            d.twitter = twitter.text;
            d.discord = discord.text;
            if(twitter.text != "")
                DBManager.Instance.UpdateTwitter(twitter.text);
            if (discord.text != "")
                DBManager.Instance.UpdateDiscord(discord.text);
            DBEvents.OnRegisterTeam(d, OnSaved);

            Close();
        }
        void OnSaved(bool isOK, string response)
        {
            if (!isOK)
            {                
                Events.OnPopup(response, null);
            }
            else
            {
                string text = Data.Instance.texts.Get("rrss_user_saved");
                Events.OnPopup(text, null);
                saveButton.SetInteraction(true);
            }
        }
        void MyData()
        {
            Application.OpenURL(DB.DBManager.Instance.myStats_url + DB.DBManager.Instance.Email);
            Close();
        }
        void OnAccountDeleteClicked()
        {
            string title = Data.Instance.texts.Get("delete_account");
            string subtitle = Data.Instance.texts.Get("delete_accoun_confirm");
            Events.OnConfirmPanel(title, subtitle, OnDelete, "confirm", "cancel");
        }
        void OnDelete(bool doIt)
        {
            if(doIt)
            {
#if UNITY_EDITOR
                new DBAuthentication().DeleteAccount(OnDeleted);
#endif
                DB.DBManager.Instance.DbUserData.Delete(OnDeleted);

                Events.OnLoadingPanel(true);
            }
        }
        void OnDeleted(bool done, string result)
        {
            if(done)
            {
                Application.Quit();
                Close();
            }
            else
            {
                Events.OnLoadingPanel(false);
                Events.OnPopup(result, null);
            }
        }
    }
}