using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Fulbo.DB
{
    public class DBAuthentication
    {
        const string ContentType = "application/x-amz-json-1.1";
        [Serializable]
        public class RegisterStoreData
        {
            public string user_id;
            public string type;
            public string email;
            public string referrer_user_id;
            public string hash;
        }
        [Serializable]
        public class RegisterData
        {
            public string Username;
            public string Password;
            public string ClientId;
            public List<UserAttributesData> UserAttributes;           
        }
        [Serializable]
        public class DeleteAccountData
        {
            public string AccessToken;
            public string Username;
            public string Password;
            public string ClientId;
            public List<UserAttributesData> UserAttributes;
        }
        [Serializable]
        public class UserAttributesData
        {
            public string Name;
            public string Value;
        }
        [Serializable]
        public class LoginData
        {
            public AuthParametersData AuthParameters;
            public string AuthFlow;
            public string ClientId;
        }
        [Serializable]
        public class AuthParametersData
        {
            public string USERNAME; //email
        }


        [Serializable]
        public class AuthChallengeData
        {
            public string ChallengeName;
            public string ClientId;
            public ChallengeResponsesData ChallengeResponses;
            public string Session;
        }
        [Serializable]
        public class ChallengeResponsesData
        {
            public string USERNAME; //email
            public string ANSWER; //email
        }

        [Serializable]
        public class RefreshTokenData
        {
            public AuthParametersTokenData AuthParameters;
            public string AuthFlow;
            public string ClientId;
        }
        [Serializable]
        public class AuthParametersTokenData
        {
            public string REFRESH_TOKEN;
        }

        public void RegisterForStoreUsers(string userID, string type, string oldEmail, System.Action<bool, string> OnSuccess)
        {
#if UNITY_EDITOR
            userID = DBManager.Instance.user.ToString();
#endif

            string url = DBManager.Instance.URL + "users";
            Debug.Log("RegisterForStoreUsers userID: " + userID + " url: " + url);


            WWWForm form = new WWWForm();

            RegisterStoreData data = new RegisterStoreData();
            data.user_id = userID;
            data.type = type;
            data.email = oldEmail; // empty if its the new email from store:

            string referrerID = DBManager.Instance.GetComponent<InstallReferrer>().GetReferrer();
            

            int number;
            if (referrerID != null && referrerID != "" && int.TryParse(referrerID, out number))
            {
                data.referrer_user_id = referrerID;
            }
            else
            {
                data.referrer_user_id = "";
            }

            string hashString = userID + DBManager.HASH_SALT2 + type + data.referrer_user_id + oldEmail;

            data.hash = Utils.SHA(hashString);

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(url, json, OnSuccess, "POST", "Registering via store");//, Data.Instance.texts.Get("http_sending_scores"));
        }

        public void Register(string email, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("Register email: " + email);

            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", ContentType);
            headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.SignUp");

            RegisterData data = new RegisterData();
            data.Username = email;
            data.Password = RandomString();
            data.ClientId = DBManager.Instance.GetClientID();
            data.UserAttributes = new List<UserAttributesData>();
            UserAttributesData userAttributes = new UserAttributesData();
            userAttributes.Name = "name";
            userAttributes.Value = "N/A";
            data.UserAttributes.Add(userAttributes);

            string referrerID = DBManager.Instance.GetComponent<InstallReferrer>().GetReferrer();
            int number;
            if (referrerID != null && referrerID != "" && int.TryParse(referrerID, out number))
            {
                UserAttributesData referrerAttributes = new UserAttributesData();
                referrerAttributes.Name = "custom:referrer_user_id";
                referrerAttributes.Value = referrerID;
                data.UserAttributes.Add(referrerAttributes);
            }   

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(DBManager.Instance.GetURLAuth(), json, OnSuccess, "POST", "Registering", headers);//, Data.Instance.texts.Get("http_sending_scores"));
        }
        public void Login(string email, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("Login email: " + email);

            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", ContentType);
            headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");

            LoginData data = new LoginData();
            data.AuthParameters = new AuthParametersData();
            data.AuthParameters.USERNAME = email;
            data.AuthFlow = "CUSTOM_AUTH";
            data.ClientId = DBManager.Instance.GetClientID();

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(DBManager.Instance.GetURLAuth(), json, OnSuccess, "POST", "Registering", headers);//, Data.Instance.texts.Get("http_sending_scores"));
        }
        public void AuthChallenge(string email, string code, string session, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("AuthChallenge email: " + email);

            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", ContentType);
            headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.RespondToAuthChallenge");

            AuthChallengeData data = new AuthChallengeData();
            data.ChallengeName = "CUSTOM_CHALLENGE";
            data.ClientId = DBManager.Instance.GetClientID();
            data.ChallengeResponses = new ChallengeResponsesData();
            data.ChallengeResponses.USERNAME = email;
            data.ChallengeResponses.ANSWER = code;
            data.Session = session;

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(DBManager.Instance.GetURLAuth(), json, OnSuccess, "POST", "Registering", headers);//, Data.Instance.texts.Get("http_sending_scores"));
        }
        public void RefreshToken(string token, System.Action<bool, string> OnSuccess)
        {
            Debug.Log("RefreshToken");

            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", ContentType);
            headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");

            RefreshTokenData data = new RefreshTokenData();
            data.AuthFlow = "REFRESH_TOKEN_AUTH";
            data.ClientId = DBManager.Instance.GetClientID();
            data.AuthParameters = new AuthParametersTokenData();
            data.AuthParameters.REFRESH_TOKEN = token;           

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(DBManager.Instance.GetURLAuth(), json, OnSuccess, "POST", "Registering", headers);//, Data.Instance.texts.Get("http_sending_scores"));
        }
        public string RandomString()
        {
            byte[] random = new byte[64];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(random);

            string randomBase64 = Convert.ToBase64String(random);
            Console.WriteLine("Random string: {0}\r\n ", randomBase64);
            return randomBase64;
        }
        public void DeleteAccount(System.Action<bool, string> OnSuccess)
        {
            Debug.Log("Delete Account");

            WWWForm form = new WWWForm();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", ContentType);
            headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.DeleteUser");

            DeleteAccountData data = new DeleteAccountData();
            data.Username = DBManager.Instance.Email;
            data.AccessToken = DBManager.Instance.tokens.AuthenticationResult.AccessToken;
            data.Password = RandomString();
            data.ClientId = DBManager.Instance.GetClientID();
            data.UserAttributes = new List<UserAttributesData>();
            UserAttributesData userAttributes = new UserAttributesData();
            userAttributes.Name = "name";
            userAttributes.Value = "N/A";
            data.UserAttributes.Add(userAttributes);

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(DBManager.Instance.GetURLAuth(), json, OnSuccess, "POST", "Deleting Account", headers);//, Data.Instance.texts.Get("http_sending_scores"));
        }

    }
}
