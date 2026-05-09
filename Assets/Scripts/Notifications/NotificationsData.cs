using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fulbo.Stadiums;
using Fulbo.Game;
using Fulbo.DB;
using Random = UnityEngine.Random;
using Fulbo.AssetsBundle;

namespace Fulbo
{
    public class NotificationsData : DataLoader
    {
        public int cupID = 1;

        public static NotificationsData mInstance;
        public static NotificationsData Instance { get { return mInstance; } }

        public List<NotifData> all;

        [Serializable]
        public class NotifData
        {           
            public int id;
            public bool read;
            public string date;
            public string title;
            public string text;
            public string important;
        }

        void Awake()
        {
            if (mInstance != null)
                Destroy(gameObject);
            else
            {
                mInstance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        Action OnDone;
        System.Action OnReady;
        public override void LoadData(System.Action OnReady) {
            this.OnReady = OnReady;
            if (Data.Instance.loadType == Data.loadTypes.DATABASE)
                Data.Instance.sheetLoader.LoadFromTo(url, OnLoaded, yutokun.Delimiter.Comma);
            else if (Data.Instance.loadType == Data.loadTypes.LOCAL)
                OnLoaded(yutokun.CSVParser.LoadFromString(file_in_server.text, yutokun.Delimiter.Comma));
            else {
                AssetsBundleLoader abl = AssetsBundleManager.Instance.assetsBundleLoader;
                OnLoaded(yutokun.CSVParser.LoadFromString(abl.GetSheetText(file_in_server.name + ".txt"), yutokun.Delimiter.Comma));
                //Data.Instance.sheetLoader.LoadFromTo(Data.Instance.GetURL() + "AssetBundles/" + file_in_server.name + ".txt" + "?rand=" + UnityEngine.Random.Range(1000, 10000), OnLoaded);
            }
        }
        public void OnLoad(Action OnDone)
        {
            this.OnDone = OnDone;
            LoadData(null);
        }
        public override void OnLoaded(List<List<string>> d)
        {
            OnDataLoaded(d);
            if (OnDone != null)
            {
                OnDone();
                OnDone = null;
            }
        }        
        void OnDataLoaded(List<List<string>> d)
        {
            int colID = 0;
            int rowID = 0;
            NotifData contentLine = null;
            foreach (List<string> line in d)
            {
                foreach (string value in line)
                {
                    if (rowID >= 1)
                    {
                        if (colID == 0)
                        {
                            if (value != "")
                            {
                                Debug.Log("Val: " + value);
                                contentLine = new NotifData();
                                contentLine.id = int.Parse(value);
                                all.Add(contentLine);
                            }
                            else
                                return;
                        }
                        else
                        {
                            if (colID == 1 && value != "")
                            {
                                contentLine.date = value;
                            }
                            if (colID == 2 && value != "")
                            {
                                contentLine.title = value;
                            }
                            if (colID == 3 && value != "")
                            {
                                contentLine.text = value;
                            }
                            if (colID == 4 && value != "")
                            {
                                contentLine.important = value;
                            }
                        }
                    }
                    colID++;
                }
                colID = 0;
                rowID++;
            }
        }
    }
}