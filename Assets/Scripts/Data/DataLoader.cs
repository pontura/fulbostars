using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fulbo.AssetsBundle;

namespace Fulbo
{
    public class DataLoader : MonoBehaviour
    {
        public string url;
        public TextAsset file_in_server;
        System.Action OnReady;

        private void Start()
        {
            Events.ResetApp += ResetApp;
        }
        private void OnDestroy()
        {
            Events.ResetApp += ResetApp;
        }
        public virtual void LoadData(System.Action OnReady)
        {
            this.OnReady = OnReady;
            if (Data.Instance.loadType == Data.loadTypes.DATABASE)
                Data.Instance.sheetLoader.LoadFromTo(url, OnLoaded);
            else if (Data.Instance.loadType == Data.loadTypes.LOCAL)
                OnLoaded(yutokun.CSVParser.LoadFromString(file_in_server.text));
            else {
                AssetsBundleLoader abl = AssetsBundleManager.Instance.assetsBundleLoader;
                OnLoaded(yutokun.CSVParser.LoadFromString(abl.GetSheetText(file_in_server.name + ".txt")));
                //Data.Instance.sheetLoader.LoadFromTo(Data.Instance.GetURL() + "AssetBundles/" + file_in_server.name + ".txt" + "?rand=" + UnityEngine.Random.Range(1000, 10000), OnLoaded);
            }
        }
        public virtual void OnLoaded(List<List<string>> d)
        {
            if (OnReady != null)
            {
                OnReady();
                OnReady = null;
            }
        }
        void ResetApp() { Reset();  }
        public virtual void Reset() { }
    }
}
