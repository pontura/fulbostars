using UnityEngine;
using System.Collections;
using System;

namespace Fulbo.DB
{
    [Serializable]
    public class DBTokens
    {
        public AuthenticationResultData AuthenticationResult;

        [Serializable]
        public class AuthenticationResultData
        {
            public string AccessToken;
            public int ExpiresIn;
            public string IdToken;
            public string RefreshToken;
            public string TokenType;
            public string Session;
        }
        public void Save()
        {
            PlayerPrefs.SetString("AccessToken", AuthenticationResult.AccessToken);
            PlayerPrefs.SetInt("ExpiresIn", AuthenticationResult.ExpiresIn);
            PlayerPrefs.SetString("IdToken", AuthenticationResult.IdToken);
            PlayerPrefs.SetString("RefreshToken", AuthenticationResult.RefreshToken);
            PlayerPrefs.SetString("TokenType", AuthenticationResult.TokenType);
        }
        public void Load()
        {
            AuthenticationResult.AccessToken = PlayerPrefs.GetString("AccessToken");
            AuthenticationResult.ExpiresIn = PlayerPrefs.GetInt("ExpiresIn");
            AuthenticationResult.IdToken = PlayerPrefs.GetString("IdToken");
            AuthenticationResult.RefreshToken = PlayerPrefs.GetString("RefreshToken");
            AuthenticationResult.TokenType = PlayerPrefs.GetString("TokenType");
        }
        public void DeleteAll()
        {
            AuthenticationResult.AccessToken = ""; 
            AuthenticationResult.ExpiresIn =    0;
            AuthenticationResult.IdToken =      "";
            AuthenticationResult.RefreshToken = "";
            AuthenticationResult.TokenType =    "";
        }
    }
    
}
