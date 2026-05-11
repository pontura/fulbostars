using System;
using System.Collections;
using System.Collections.Generic;
using Fulbo.UI;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
#if UNITY_IOS
using Apple.GameKit;
#endif

namespace Fulbo.Auth {
    public class Authentication : MonoBehaviour {
        [SerializeField] GameObject panel;
        [SerializeField] Text resultField;
        [SerializeField] GameObject buttons;
        [SerializeField] ButtonCustom button;
        [SerializeField] ButtonCustom buttonEmails;
        RegisterScreen registerScreen;
        bool initial_opening;
        string socialString = "GooglePlayGames";
        string oldEmail = "";

        private void Awake() {
            panel.SetActive(true);
            buttons.SetActive(false);
#if UNITY_IOS && !UNITY_EDITOR
            socialString = "AppleGameCenter";
#endif
        }
        async public void Init() {
            initial_opening = true; // chequea al user al abrir la apps
            registerScreen = GetComponent<RegisterScreen>();
            dbAuth = new DB.DBAuthentication();

            panel.SetActive(true);
            button.Init(0, Clicked, "ENTER");
            buttonEmails.Init(1, Clicked, "LOGIN WITH EMAIL");
            resultField.text = "Loading...";

#if UNITY_EDITOR || UNITY_STANDALONE
            DB.DBManager.Instance.SetSocialUserName(DB.DBManager.Instance.user.ToString());
            // PlayGamesPlatform.DebugLogEnabled = true;
            // PlayGamesPlatform.Activate();
            registered = true;

            LoadUserData();
            // ProcessAuthentication(SignInStatus.Success);
#elif UNITY_ANDROID
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate().Authenticate(ProcessAuthentication);            
#elif UNITY_IOS
            Debug.Log("iOs");
            await AppleGameCenterLogin();
#endif
        }

#if UNITY_IOS
        string Signature;
        string TeamPlayerID;
        string Salt;
        string PublicKeyUrl;
        string Timestamp;
        public async Task AppleGameCenterLogin()
        {
            Debug.Log("AppleGameCenterLogin");
            if (!GKLocalPlayer.Local.IsAuthenticated)
            {
                // Perform the authentication.
                var player = await GKLocalPlayer.Authenticate();
                Debug.Log($"GameKit Authentication: player {player}");

                // Grab the display name.
                var localPlayer = GKLocalPlayer.Local;
                Debug.Log($"Local Player: {localPlayer.DisplayName}");

                // Fetch the items.
                var fetchItemsResponse = await GKLocalPlayer.Local.FetchItems();

                Signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
                TeamPlayerID = localPlayer.TeamPlayerId;
                Debug.Log($"Team Player ID: {TeamPlayerID}");

                Salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
                PublicKeyUrl = fetchItemsResponse.PublicKeyUrl;
                Timestamp = fetchItemsResponse.Timestamp.ToString();

                Debug.Log($"GameKit Authentication: signature => {Signature}");
                Debug.Log($"GameKit Authentication: publickeyurl => {PublicKeyUrl}");
                Debug.Log($"GameKit Authentication: salt => {Salt}");
                Debug.Log($"GameKit Authentication: Timestamp => {Timestamp}");
            }
            else
            {
                Debug.Log("AppleGameCenter player already logged in.");
            }
            LoadUserData();
        }
#endif
        bool registered;
        private void Update() {
            if (!registered && Social.localUser.authenticated) {
                LoadUserData();
                registered = true;
            }
        }

        private void OnApplicationFocus(bool focus) {
            if (focus) {
                if (authTriesCount > 0) {
#if UNITY_ANDROID
                    PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
#endif
                }
            }
        }

        DB.DBAuthentication dbAuth;
        int authTriesCount;
#if UNITY_ANDROID
        void ProcessAuthentication(SignInStatus status) {
            Debug.Log("ProcessAuthentication for userName:" + Social.localUser.userName + " (" + Social.localUser.id + ")");
            if (status == SignInStatus.Success) {
                if (!registered)
                    LoadUserData(); // Load userData
            } else {

                if (authTriesCount == 0) {
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["description"] = "Auto login to Google Play Games failed";
                    param["tries_count"] = authTriesCount;
                    Events.OnTrack("GooglePlayAuthFailure", param);
                    System.Action callback = null;
                    callback = () => PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
                    Events.OnPopup("Auto login to Google Play Games failed", callback);
                }
                if (authTriesCount == 1) {
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["description"] = "Must have Google Play Games installed";
                    param["tries_count"] = authTriesCount;
                    Events.OnTrack("GooglePlayAuthFailure", param);
                    System.Action callback = null;
                    callback = () => { PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication); Application.OpenURL("https://play.google.com/store/apps/details?id=com.google.android.play.games"); };
                    Events.OnPopup("You must have Google Play Games installed.", callback);
                }
                if (authTriesCount > 1) {
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["description"] = "Google Play Games couldn't be install";
                    param["tries_count"] = authTriesCount;
                    Events.OnTrack("GooglePlayAuthFailure", param);
                    Events.OnPopup("Google Play Games Authenthication failed.", () => Application.Quit());
                }
                authTriesCount++;
            }
        }
#endif
        void LoadUserData() {
#if UNITY_EDITOR || UNITY_STANDALONE
            DB.DBManager.Instance.SetSocialUserName(DB.DBManager.Instance.user.ToString());

            // DB.DBManager.Instance.SetEmail("a_7470323465463971749@GooglePlayGames");
            // DB.DBManager.Instance.SetEmail("g08383162240248652473@GooglePlayGames");
            // DB.DBManager.Instance.SetEmail(editorEmail);
            DB.DBManager.Instance.DbUserData.LoadUserData(OnLoadedUserData);
            //  print("LoadUserData " + editorEmail);

#else
            DB.DBManager.Instance.SetSocialUserName(Social.localUser.userName);
            string email = PlayerPrefs.GetString("email", "");
            if (!email.Contains("@" + socialString)) oldEmail = email;
            else oldEmail = "";

            DB.DBManager.Instance.SetEmail(Social.localUser.id + "@" + socialString);
            DB.DBManager.Instance.DbUserData.LoadUserData(OnLoadedUserData);
#endif
        }
        void ChangeStateToRegister() {
            buttons.SetActive(true);
            print("ChangeStateToRegister");
            resultField.text = "Welcome to Fulbo Galaxy!";
        }
        void OnUserFromStoreRegister(bool isOk, string result) {
            print("OnUserFromStoreRegister isok:  " + isOk);
            if (isOk) {
                initial_opening = false;
                LoadUserData();
            } else {
                ChangeStateToRegister();
            }
        }
        void OnLoadedUserData() // se llama al abrir la app:
        {
            //to-do
            registerScreen.OnUserDataLoaded();
            return;


            print("initial_opening " + initial_opening);
            if (DB.DBManager.Instance.DbUserData != null
                && DB.DBManager.Instance.DbUserData.data != null
                && DB.DBManager.Instance.DbUserData.data.players_characters != null
                && DB.DBManager.Instance.DbUserData.data.players_characters.Count > 0) // user ok:
            {
                registerScreen.OnUserDataLoaded();
            } else if (initial_opening) {

#if UNITY_EDITOR || UNITY_STANDALONE
                dbAuth.RegisterForStoreUsers(DB.DBManager.Instance.user.ToString(), socialString, "", OnUserFromStoreRegister);

                // si estas en el editor entra al mail default:
#else
                string email = PlayerPrefs.GetString("email", "");
                if (email.Contains("@" + socialString))
                    email = ""; // empty email if is registered in social

                // if not registered on init try to Register:
                resultField.text = "Hello, " + Social.localUser.userName;
                dbAuth.RegisterForStoreUsers(Social.localUser.id, socialString, oldEmail, OnUserFromStoreRegister);
            
#endif


            } else {
                ChangeStateToRegister();// user not registered:
            }
        }

        void Clicked(int id) {
            buttons.SetActive(false);
            resultField.text = "Entering...";

            //if (id == 0)
            //{
#if UNITY_EDITOR
            registerScreen.ShowMainMenu(true);
            panel.SetActive(false);
#elif UNITY_ANDROID
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
#endif
            //}
            //if (id == 1)
            //{
            //    registerScreen.ShowMainMenu(true);
            //    panel.SetActive(false);
            //}
            //registered = false;
            //Debug.Log("Clicked");

        }
    }
}