using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Fulbo.DB
{
    public class DBRegisterTeam
    {
        [Serializable]
        public class RData
        {
            public string email;
            public string user;
            public string twitter;
            public string discord;
            public string hash;

            public static object Instance { get; internal set; }
        }
       
        public void Register(DBRegisterTeam.RData data, string url, System.Action<bool, string> OnDone)
        {
            url += data.email;

            if (data.twitter == null) data.twitter = "";
            if (data.discord == null) data.discord = "";

            DBManager.Instance.DbUserData.data.user = data.user;
            DBManager.Instance.DbUserData.data.twitter = data.twitter;
            DBManager.Instance.DbUserData.data.discord = data.discord;

            string hash =
                DBManager.Instance.Email +
                DBManager.HASH_SALT1 +
                DBManager.Instance.DbUserData.data.user +
                DBManager.Instance.DbUserData.data.twitter +
                DBManager.Instance.DbUserData.data.discord;

            data.hash = Utils.SHA(hash);

            Debug.Log("Register Team : " + url + " hash: " + data);

            string json = JsonUtility.ToJson(data, true);
            DBManager.Instance.Request(url, json, OnDone, "PUT", Data.Instance.texts.Get("http_updating_user"));
        }

    }      
}
