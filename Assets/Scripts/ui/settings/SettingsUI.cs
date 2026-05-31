using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Onboarding;
using Fulbo.DB;

namespace Fulbo.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] GameObject panel;

        [SerializeField] ButtonCustom deleteAccount;

        [SerializeField] Toggle musicToggle;
        [SerializeField] Toggle announcerToggle;
        [SerializeField] Toggle speechBubblesToggle;
        [SerializeField] GameObject controlsUI;
        [SerializeField] Text versionField;

        [SerializeField] ButtonCustom accountButton;
        [SerializeField] ButtonCustom controlsButton;
        [SerializeField] ButtonCustom endgameButton;
        [SerializeField] ButtonCustom registerLogoutButton;

        [SerializeField] ButtonCustom closeButton;

        [SerializeField] GameObject settingsBtn;
        [SerializeField] GameObject pauseBtn;

        void Start()
        {
            panel.SetActive(false);
            Events.OpenSettings += OpenSettings;
            controlsUI.SetActive(false);
            closeButton.Init(0, Close);
            closeButton.SetType(ButtonCustom.types.CLOSE);

#if UNITY_IOS || UNITY_ANDROID
            controlsButton.gameObject.SetActive(false);
#endif
        }
        public void Close(int id)
        {
            panel.SetActive(false);
            Time.timeScale = 1;
        }
        void OpenSettings(bool inGame)
        {
            if(Data.Instance.mode == Data.modes.PARTYMODE)
            {
                accountButton.Init(0, Clicked, Data.Instance.texts.Get("register"));
            }
            else
            {
                accountButton.Init(0, Clicked, Data.Instance.texts.Get("accountButton"));
            }
#if UNITY_IOS || UNITY_ANDROID
            controlsButton.gameObject.SetActive(false);
#else
            // controlsButton.Init(1, Clicked, Data.Instance.texts.Get("controlsButton"));
            // if (Data.Instance.newScene == "Game" || Data.Instance.newScene == "Tutorial")
            //     controlsButton.gameObject.SetActive(false);
            // else
            //     controlsButton.gameObject.SetActive(true);
#endif            

            // if (DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.DEV)
            // {
            //     deleteAccount.gameObject.SetActive(true);
            //     deleteAccount.Init(4, Clicked, Data.Instance.texts.Get("delete_account"));
            // }
            // else
                deleteAccount.gameObject.SetActive(false);

            registerLogoutButton.Init(3, Clicked, Data.Instance.texts.Get("accountButton"));

            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_popup_settings");
            SetRegisterLogoutField();
            panel.SetActive(true);

            musicToggle.isOn = Data.Instance.settings.mainSettings.music_on;
            announcerToggle.isOn = Data.Instance.settings.mainSettings.announcer_on;
            speechBubblesToggle.isOn = Data.Instance.settings.mainSettings.speech_bubbles_on;
            SetMusic();
            SetAnnouncements();
            SetSpeechBubbles();

            // if (Data.Instance.newScene == "Game")
            // {
            //     endgameButton.gameObject.SetActive(true);
            //     endgameButton.GetComponentInChildren<Text>().text = Data.Instance.texts.Get("end_game");
            //     endgameButton.Init(2, Clicked, Data.Instance.texts.Get("endgameButton"));
            // }
            // else
                endgameButton.gameObject.SetActive(false);

            Time.timeScale = 0;

            versionField.text = "v. " + Application.version;
        }
        void Clicked(int id)
        {
            switch (id)
            {
                case 0: Account(); break;
                case 1: SetControls(); break;
                case 2:
                    string text =   Data.Instance.texts.Get("exit_game_title");
                    string text2 =  Data.Instance.texts.Get("exit_game_text");
                    Events.OnConfirmPanel(text, text2, ConfirmEndGame, "confirm", "cancel"); break;
                case 3: Logout(); break;
                case 4: OnAccountDeleteClicked(); break;
            }
        }
        void ConfirmEndGame(bool isOk)
        {
            if (isOk)
                EndGame();
            else
                Close(0);
        }
        void SetMusic()
        {
            string label = "";
            if (Data.Instance.settings.mainSettings.music_on)
                label = Data.Instance.texts.Get("music_on");
            else
                label = Data.Instance.texts.Get("music_off");

            musicToggle.GetComponentInChildren<Text>().text = label;
        }
        void SetAnnouncements()
        {
            string label = "";
            if (Data.Instance.settings.mainSettings.announcer_on)
                label = Data.Instance.texts.Get("announcer_on");
            else
                label = Data.Instance.texts.Get("announcer_off");

            announcerToggle.GetComponentInChildren<Text>().text = label;
            Data.Instance.GetComponent<Fulbo.Voices.VoicesOnScene>().SetMute();
        }
        void SetSpeechBubbles()
        {
            string label = "";
            if (Data.Instance.settings.mainSettings.speech_bubbles_on)
                label = Data.Instance.texts.Get("speech_bubbles_on");
            else
                label = Data.Instance.texts.Get("speech_bubbles_off");

            speechBubblesToggle.GetComponentInChildren<Text>().text = label;
        }
        public void ToggleMusic()
        {
            if (Data.Instance == null || musicToggle.isOn == Data.Instance.settings.mainSettings.music_on) return;
            bool music_on = !Data.Instance.settings.mainSettings.music_on;
            Data.Instance.settings.mainSettings.music_on = music_on;

            PlayerPrefs.SetInt("music", music_on? 1 : 0);

            AudioManager.Instance.SetActive("music", music_on);
            if(!music_on)
                AudioManager.Instance.SetActive("music2", music_on);
            SetMusic();
        }
        public void ToggleAnouncements()
        {
            if (Data.Instance == null || announcerToggle.isOn == Data.Instance.settings.mainSettings.announcer_on) return;
            if (Data.Instance && Data.Instance.settings != null && Data.Instance.settings.mainSettings != null)
                Data.Instance.settings.mainSettings.announcer_on = !Data.Instance.settings.mainSettings.announcer_on;

            PlayerPrefs.SetInt("announcer", Data.Instance.settings.mainSettings.announcer_on ? 1 : 0);
            SetAnnouncements();
        }
        public void ToggleSpeechBubbles()
        {
            if (Data.Instance == null || speechBubblesToggle.isOn == Data.Instance.settings.mainSettings.speech_bubbles_on) return;
            bool speech_bubbles = !Data.Instance.settings.mainSettings.speech_bubbles_on;
            Data.Instance.settings.mainSettings.speech_bubbles_on = speech_bubbles;

            PlayerPrefs.SetInt("bubbles", Data.Instance.settings.mainSettings.speech_bubbles_on ? 1 : 0);

            SetSpeechBubbles();
        }
        void SetControls()
        {
            controlsUI.SetActive(true);
          //  Close();
        }
        void EndGame()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                Data.Instance.OnSummaryOver();
            }
            else
                Data.Instance.LoadLevel("MainMenu");
            
            SetIngamePauseButton(false);
            //Events.GameOver();
            Fulbo.Game.GameManager.Instance.state = Fulbo.Game.GameManager.states.GAMEOVER;
            Data.Instance.matchData.SaveCupDataOnGameClosed(ForceCloseMatch);

            if (DB.DBManager.Instance.DbUserData.data.gameData.cups.NoMoreLifes())
                CupsData.Instance.EndCup();

            Events.AdsWatchInterstitial((x)=> {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["type"] = "matchLose";
                if(x)
                    Events.OnTrack("IterstitialAd", param);
                else
                    Events.OnTrack("InterstitialAdNotShown", param);                
                Close(0);
            });
        }
        void ForceCloseMatch(bool isOk, string error)
        {
            if (isOk)
            {
                Data.Instance.ui.CheckCupLifeLose();
                Debug.Log("ForceCloseMatch");
            }
            else
            {
                Data.Instance.matchData.SaveCupDataOnGameClosed(ForceCloseMatch);
            }
        }
        void Logout()
        {
            Time.timeScale = 1;
            Events.ResetApp();
            PlayerPrefs.DeleteAll();
            DB.DBManager.Instance.Logout();
            Invoke("DelayedLogout", 0.5f);
        }
        void DelayedLogout()
        {
            Application.Quit();
        }
        void SetRegisterLogoutField()
        {
            if(DB.DBManager.Instance.DbUserData.type == DB.DBUserData.types.GUEST)
                registerLogoutButton.SetText(Data.Instance.texts.Get("register"));
            else
                registerLogoutButton.SetText(Data.Instance.texts.Get("logout"));
        }
        void Account()
        {
            //if (Data.Instance.mode == Data.modes.PARTYMODE)
            //{
            //    Data.Instance.LoadLevel("0_Register");
            //}
            //else
            //{
            //    GetComponent<AccountPopup>().Open();
            //}
            //Close(0);
        }
        void OnAccountDeleteClicked()
        {
            string title = Data.Instance.texts.Get("delete_account");
            string subtitle = Data.Instance.texts.Get("delete_accoun_confirm");
            Events.OnConfirmPanel(title, subtitle, OnDelete, "confirm", "cancel");
        }
        void OnDelete(bool doIt)
        {
            if (doIt)
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
            if (done)
            {
                Application.Quit();
            }
            else
            {
                Events.OnLoadingPanel(false);
                Events.OnPopup(result, null);
            }
        }

        public void SetIngamePauseButton(bool enable) {
            pauseBtn.SetActive(enable);
            settingsBtn.SetActive(!enable);

        }


        public void JoinDiscord() {
            Application.OpenURL("https://discord.gg/2HeXUGWMN");
        }

        public void JoinInstagram() {
            Application.OpenURL("https://www.instagram.com/fulbostars/");
        }

        public void JoinTwitter() {
            Application.OpenURL("https://twitter.com/FulboStars");
        }
    }

    
}
