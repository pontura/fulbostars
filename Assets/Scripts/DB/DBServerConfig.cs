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
    public class DBServerConfig
    {
        const string congig_url = "https://s3.amazonaws.com/play.fulbogalaxy.com/config.json";
        public DBServerConfigData configData;

        [Serializable]
        public class DBServerConfigData
        {
            public VersionsData[] versions;
            public VersionsData[] versions_ios;
            public string[] loading;
        }
        [Serializable]
        public class VersionsData
        {
            public string num;      //Application.version
            public string from;     //PROD, DEV
            public string status;   //online, offlie
        }
        System.Action<bool, string> OnDone;
        public void Load(System.Action<bool, string> OnDone)
        {
            this.OnDone = OnDone;
            DBManager.Instance.LoadFromURL(congig_url + "?rand=" + UnityEngine.Random.Range(1000,10000), OnSuccess);
        }
        void OnSuccess(string text)
        {
            Debug.Log(text);
            configData = JsonUtility.FromJson<DBServerConfigData>(text);
            if(configData == null)
                OnDone(true, "[ERROR] No server config file");
            else if (configData.versions.Length == 0)
                OnDone(true, "[ERROR] No versions in congif file");
            else
            {
                OnDone(true, "");
            }
        }
    }
}
