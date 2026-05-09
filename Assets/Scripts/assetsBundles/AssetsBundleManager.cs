using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.AssetsBundle
{
    public class AssetsBundleManager : MonoBehaviour
    {
        public bool forceServerDownload;

        static AssetsBundleManager mInstance = null;
        public AssetsBundleLoader assetsBundleLoader;
        public static AssetsBundleManager Instance  { get  {  return mInstance; }  }
        System.Action<string> OnDone;

        void Awake()
        {
            if (!mInstance)
                mInstance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            Events.LoadAssetBundles += LoadAssetBundles;
        }
        private void OnDestroy()
        {
            Events.LoadAssetBundles -= LoadAssetBundles;
        }
        void LoadAssetBundles(System.Action<string> OnDone)
        {
            this.OnDone = OnDone;
#if UNITY_EDITOR  || UNITY_STANDALONE
            if (Data.Instance.mode == Data.modes.PARTYMODE)
            {
                LoadLocal();
                return;
            }
            if (forceServerDownload)
            {
                LoadFromServer(Data.Instance.GetURL());
                return;
            }
            LoadLocal();
#elif UNITY_WEBGL
            LoadFromServer("");//mismo path:
#elif UNITY_ANDROID || UNITY_IOS
            LoadFromServer(Data.Instance.GetURL());
#endif
        }
        void LoadLocal()
        {
            Debug.Log("LoadLocal");
            if(Data.Instance.mode == Data.modes.PARTYMODE)
                 StartCoroutine(assetsBundleLoader.DownloadAll(Application.streamingAssetsPath + "/", AllLoaded));
            else
                StartCoroutine(assetsBundleLoader.DownloadAll(Application.dataPath + "/AssetBundles/", AllLoaded));
        }
        string url;
        void LoadFromServer(string _url) // los trata de bajar locales primero:
        {
            Debug.Log("Load Assets bundles from " + _url);
            this.url = _url;
            StartCoroutine(assetsBundleLoader.DownloadAll(url + Data.Instance.GetAssetsBundleFolder() + "/", AllLoaded));         
        }
        void AllLoaded(string response)
        {
            Debug.Log("_______________AllLoaded________________");
            if(OnDone!= null)
                OnDone(response);
            OnDone = null;
        }
        bool assetsLoaded;
        public void InstantiateAssets()
        {
            if (assetsLoaded) return;
            assetsLoaded = true;
            //Debug.Log("_______________InstantiateAssets________________");
            //GameObject go;
            //go = assetsBundleLoader.GetAsset("stadium.generic", "Assets/stadiums/StadiumsData.prefab");
            //if (go != null) Instantiate(go);

            //go = assetsBundleLoader.GetAsset("pinballs.generic", "Assets/PinballObstacles/PinballsManager.prefab");
            //if (go != null) Instantiate(go);

            //Debug.Log("_______________assets instantiated________________");

            //StoryModeData.Instance.Init();
        }
       
    }    
}