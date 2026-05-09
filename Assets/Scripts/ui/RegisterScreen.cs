using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Fulbo;
using System.Text.RegularExpressions;
using System;
using Fulbo.Auth;
using Fulbo.Onboarding;
using System.Globalization;

public class RegisterScreen : MonoBehaviour {

    //upgrade_by_device
    const string upgrade_webgl = "https://play.fulbogalaxy.com";
    const string upgrade_android = "https://y0e8xxplkh.execute-api.us-east-1.amazonaws.com/";
    const string upgrade_ios = "https://y0e8xxplkh.execute-api.us-east-1.amazonaws.com/";

    [SerializeField] Text versionField;
    public Text responseField;
    string response_error;
    string response_ok;
    DBAuthentication dbAuth;
    //CODE:
    [SerializeField] GameObject mainButtons;

    //CODE:
    [SerializeField] GameObject responsePopup;
    [SerializeField] GameObject codePopup;
    [SerializeField] Text codeTitleField;
    [SerializeField] InputField codeField;
    [SerializeField] Text codeButtonField;
    [SerializeField] Text title;

    [SerializeField] Text loadingField;

    //INPUT:
    // [SerializeField] GameObject inputPopup;
    //  [SerializeField] Text submitButtonField;
    [SerializeField] InputField inputField;

    //oldversion:
    [SerializeField] GameObject oldVersionPopup;
    [SerializeField] Text oldVersionPopupField;
    [SerializeField] Text oldVersionPopupButtonField;


    [SerializeField] Fulbo.UI.ButtonCustom mainButton;
    [SerializeField] Fulbo.UI.ButtonCustom toggleButton;

    [SerializeField] string email;
    [SerializeField] string session; // lo da el login:
    string RefreshToken;

    inputTypes inputType;
    enum inputTypes {
        REGISTER,
        LOGIN
    }
    string emailText = "Enter your email...";

    void Start() {
        //#if UNITY_EDITOR
        //        //Application.targetFrameRate = 120;
        //#elif UNITY_ANDROID
        //        if(Screen.currentResolution.refreshRate >= 60)
        //            Application.targetFrameRate = 60;
        //        else
        //            Application.targetFrameRate = 40;
        //#endif
        GetComponent<Fulbo.Notifications.NotificationsOverall>().Init(OnInit);
    }
    void OnInit() {
        if (Data.Instance != null) {
            Data.Instance.mode = Data.modes.STORYMODE;
            DBManager.Instance.DbUserData.SetType(DBUserData.types.REGISTERED);
        } else {
            StartRegister();
        }
    }
    void StartRegister() {
        oldVersionPopup.SetActive(false);
        versionField.text = Application.version;
        dbAuth = new DBAuthentication();
        ShowMainMenu(false);
        codePopup.SetActive(false);
        // inputPopup.SetActive(false);

        Reset();
        inputField.text = emailText;
        response_error = "A valid email is required!";
        RefreshToken = PlayerPrefs.GetString("RefreshToken", "");

        mainButton.Init(0, MainButtonClicked, "REGISTER");
        toggleButton.Init(0, ToggleButtonClicked, "ALREADY HAVE AN ACCOUNT");

        DBManager.Instance.LoadServerConfigFile(LoadServerConfigFileDone);// Get Server Basic Data...

    }
    //private void Update()
    //{
    //    if (inputField.isFocused && inputField.text == emailText)
    //        inputField.text = "";
    //}
    public void BackToMainMenu() {
        ShowMainMenu(true);
        codePopup.SetActive(false);
    }
    void LoadServerConfigFileDone(bool isOn, string text) // check if build is online and can be played:
    {
        Debug.Log("LoadServerConfigFileDone " + isOn + " text: " + text);
        if (!isOn)
            OnFeedback(text, true, 10);
        else
            DBManager.Instance.SetConfigFile(ResultsFromServerConfig);
    }
    void ResultsFromServerConfig(bool isOn, string text) {
        Debug.Log("ResultsFromServerConfig " + isOn + " text: " + text);
        if (!isOn) {
            if (text == "offline")
                OnFeedback("Fulbo Galaxy is currently offline... Please come back in a few minutes", true, 10);
            else
                OldVersion(text);
        } else
            Init(text);
    }
    private void Init(string versionType) {
        loadingField.text = "";
        if (versionType == "DEV")
            OnFeedback(versionType + " version!", true);

        RefreshToken = PlayerPrefs.GetString("RefreshToken", "");

        email = PlayerPrefs.GetString("email", "");
        if (email != "")
            inputField.text = email;

        if (email != null)
            DBManager.Instance.DbAds.Load(null);

#if UNITY_EDITOR
        if (RefreshToken != "") {
            dbAuth.RefreshToken(RefreshToken, OnTokenDone);
            return;
        }
#endif
        //if (RefreshToken != "")
        //{
        //    dbAuth.RefreshToken(RefreshToken, OnTokenDone);
        //}
        //else
        //{
        GetComponent<Authentication>().Init();
        // }
    }

    public void Play_as_Guest() {
        GetComponent<AudioSource>().Play();
        DBManager.Instance.DbUserData.SetType(DBUserData.types.GUEST);
        if (Data.Instance != null)
            Data.Instance.ForceMode(Data.modes.PARTYMODE);
        //DBManager.Instance.DbUserData.LoadUserData(InitApp);
        InitApp();
    }
    public void MainButtonClicked(int ID) {
        Submit(inputType);

        GetComponent<AudioSource>().Play();
    }
    public void Refresh() {
        ShowMainMenu(true);
        if (inputType == inputTypes.REGISTER) {
            mainButton.SetText("REGISTER");
            toggleButton.SetText("ALLREADY HAVE AN ACCOUNT?");
            title.text = "CREATE AN ACCOUNT AND START PLAYING!";
        } else {
            mainButton.SetText("LOGIN");
            toggleButton.SetText("CREATE AN ACCOUNT");
            title.text = "ENTER YOUR EMAIL AND START PLAYING!";
        }
    }
    public void ToggleButtonClicked(int ID) {
        GetComponent<AudioSource>().Play();
        ShowMainMenu(false);
        if (inputType == inputTypes.REGISTER)
            inputType = inputTypes.LOGIN;
        else inputType = inputTypes.REGISTER;
        Invoke("Refresh", 0.1f);
    }
    void OpenPopup(inputTypes type) {
        DBManager.Instance.DbUserData.SetType(DBUserData.types.REGISTERED);
        inputType = type;
    }
    void Submit(inputTypes inputType) {
        CancelInvoke();
        email = inputField.text.Trim(' ').ToLower();
        PlayerPrefs.SetString("email", email);
        if (email == "" || !Utils.IsValidEmailAddress(email)) {
            OnFeedback(response_error, true);
            return;
        }
        ShowMainMenu(false);
        OnFeedback("Loading Data...", false);
        switch (inputType) {
            case inputTypes.REGISTER:
                dbAuth.Register(email, OnRegisterDone);
                break;
            case inputTypes.LOGIN:
                dbAuth.Login(email, OnLoginDone);
                break;
        }
        // CloseInputPopup();
    }
    void OnRegisterDone(bool isOk, string response) {
        if (!isOk) {
            BackToMainMenu();
            OnFeedback(response, true);
        } else {
            inputType = inputTypes.LOGIN;
            Submit(inputTypes.LOGIN);
        }
    }

    class LoginResponseData { public string Session; }
    public void OnLoginDone(bool isOk, string response) {
        if (!isOk) {
            BackToMainMenu();
            OnFeedback(response, true);
        } else {
            LoginResponseData loginResponse = JsonUtility.FromJson<LoginResponseData>(response);
            session = loginResponse.Session;
            SetCodePopup();
        }
    }
    void SetCodePopup() {
        codePopup.SetActive(true);
        codeTitleField.text = "Code sent, check your email! " + email;
        codeButtonField.text = "Enter your code here...";
    }
    public void SubmitCode() {
        OnFeedback("Loading Data...", false);

        //Hago esto xq tengo las bolas llenas de copiar el código con espacios o cosas extras
        string code = "";
        try {
            Regex regexObj = new Regex(@"[\D]"); //Recortar solo lo que es números
            code = regexObj.Replace(codeField.text, ""); //úsar solo los números de la autenticación para el código
        } catch (ArgumentException ex) {
            // Syntax error in the regular expression
        }

        if (code.Length < 1) {
            codePopup.SetActive(true);
            OnFeedback("Wrong code!", true);
        } else {
            codePopup.SetActive(false);
            dbAuth.AuthChallenge(email, code, session, AllDone);
            mainButtons.SetActive(false);
        }
    }
    void SetSession(string response) {
        DBTokens.AuthenticationResultData authenticationResult = JsonUtility.FromJson<DBTokens.AuthenticationResultData>(response);

        if (authenticationResult != null && authenticationResult.Session != "") {
            session = authenticationResult.Session;
        }

    }
    void AllDone(bool isOk, string response) {
        SetSession(response);
        if (!isOk) {
            BackToMainMenu();
            OnFeedback(response, true);
        } else {
            DBTokens dbTokens = JsonUtility.FromJson<DBTokens>(response);

            if (dbTokens.AuthenticationResult.RefreshToken == null || dbTokens.AuthenticationResult.RefreshToken == "") {
                Debug.Log("Wrong code!");
                // no ingresó bien el code:
                OnFeedback("Wrong code!", true);
                SetCodePopup();
                codeField.text = "";
            } else {
                Debug.Log("SAVE TOKEN");
                dbTokens.Save();
                DBManager.Instance.SetTokens(dbTokens);



                OnRegisterReady(true, "");
            }
        }
    }
    public void ShowMainMenu(bool showIt) {
        print("_____________________ShowMainMenu " + showIt);
        mainButtons.SetActive(showIt);
    }
    void OnRegisterReady(bool isOk, string response) {
        if (!isOk) {
            OnFeedback(response, true);
            ShowMainMenu(true);
        } else {
            Go();
        }
    }
    void OnFeedback(string text, bool isError, int delay = 3) {
        responsePopup.SetActive(true);
        if (isError)
            responseField.color = Color.red;
        else
            responseField.color = Color.green;
        responseField.text = text;
        Debug.Log(text + " isError: " + isError);
        Invoke("Reset", delay);
    }
    void OnTokenDone(bool isOk, string response) {
        if (!isOk) {
            BackToMainMenu();
            OnFeedback(response, true);
        } else {
            //DBRegister.RData d = new DBRegister.RData();
            //d.email = email;

            OnRegisterReady(true, "");
            //DBEvents.OnRegister(d, OnRegisterReady);
            //OnRegisterReady();
        }
    }
    void Go() {
        DBManager.Instance.SetEmail(email);
        OnFeedback("Welcome!", false);
        Invoke("GoReal", 1.5f);
    }
    private void Reset() {
        responsePopup.SetActive(false);
        responseField.text = "";
    }
    public void Register() {
        Application.OpenURL(DBManager.Instance.UrlRegister);
    }
    void GoReal() {
        DBEvents.LoadUserData(OnUserDataLoaded);
    }
    public void OnUserDataLoaded() // can be called from Authentication:
    {
        DBEvents.LoadMatches(InitApp);
    }

    string username;
    void SaveUserName(string userName, System.Action<bool, string> OnNameSaved) {
        username = userName;
        DBUserData.UserData d = new DBUserData.UserData();
        d.user = userName;
        d.twitter = "";
        d.discord = "";
        DBEvents.SaveUserData(d, OnNameSaved);
    }

    int usernameAppend = 0;
    void OnUsernameSaved(bool isOk, string result) {
        if (!isOk && usernameAppend<3) {
            usernameAppend++;
            DBUserData.UserData d = new DBUserData.UserData();
            d.user = username+"_"+usernameAppend;
            d.twitter = "";
            d.discord = "";
            DBEvents.SaveUserData(d, OnUsernameSaved);
        }
    }

    void InitApp()
    {
        if (DBManager.Instance.DbUserData.data.gameData != null && !Fulbo.Onboarding.OnBoardingManager.IsBoardingComplete())
        {
            DBManager.Instance.DbUserData.state = DBUserData.userStates.FIRST_TIME;

            if (DBManager.Instance.DbUserData.data.gameData.tutorialStep == 0) {
#if UNITY_EDITOR
                SaveUserName(DBManager.Instance.user.ToString(), OnUsernameSaved);
#else
            if (Social.localUser != null)
                SaveUserName(Social.localUser.userName, null);


            //Analytics
          //  Dictionary<string, object> param = new Dictionary<string, object>();

           // if (Data.Instance.onBoardingManager.IsBoardingStep(OnBoardingManager.BoardingStepStates.FIRST_TIME_GAME_LOADED))
            
           Events.OnTrack("UserRegistered", null);
#endif
            }
        }
        else
            DBManager.Instance.DbUserData.state = DBUserData.userStates.LOGGED_IN;

        if (Data.Instance != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Logos");
            Debug.Log("_LOGOS");
        }
    }
    void OldVersion(string text)
    {
        Debug.LogError("old version. New is: " + text);
        oldVersionPopup.SetActive(true);
        oldVersionPopupField.text = "You are currently playing an old version of Fulbo Galaxy. Please, upgrade to the " + text + " version";
        oldVersionPopupButtonField.text = "UPGRADE";
    }
    public void UpgradeClicked()
    {

#if UNITY_WEBGL
        Application.OpenURL(upgrade_webgl);
#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.aconcaguagames.FulboGalaxy");
#elif UNITY_IOS
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.aconcaguagames.FulboGalaxy");
#endif

    }

}
