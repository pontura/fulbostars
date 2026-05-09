using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Fulbo.AssetsBundle
{
    public class ForceAcceptAll : CertificateHandler
    {
         protected override bool ValidateCertificate(byte[] certificateData)
         {
             return true;
         }
    }

    public class AssetsBundleLoader : MonoBehaviour
    {
        states state;
        enum states
        {
            FIRST_BUNDLES,
            SECOND_BUNDLES
        }
        private float downloadProgress = 0.0f;
        private List<string> dataPaths;
        string loadedHashes;
        private float downloadedBytes = 0f;
        private string currentPack;
        public Dictionary<string, AssetBundle> bundles;
        public bool allLoaded;
        int totalFirstBundles = 0;
        int totalGirls = 0;
        string url;
        bool isFirstTime;

#if UNITY_IOS
    string mainBundlePath = "iOS";
#elif UNITY_WEBGL
    string mainBundlePath = "WebGL";
#elif UNITY_STANDALONE
    string mainBundlePath = "Standalone";
#else
    string mainBundlePath = "Android";
#endif

        public string CurrentPack
        {
            get => currentPack;
            set => currentPack = value;
        }

        public float Progress
        {
            get => downloadProgress;
        }
        public void ResetAll()
        {
            foreach (string assetName in dataPaths)
            {
                try
                {
                    AssetBundle ab = bundles[assetName];
                    if (ab != null)
                        ab.Unload(false);
                }
                catch
                {
                    Debug.Log("Dictionary empty");
                }

            }
        }
        void SetAssetsBundleServer()
        {
          //  Debug.LogError("______");
            dataPaths = new List<string>();
            dataPaths.Add("referis.1_100");

            dataPaths.Add("players.1_100");
          //  dataPaths.Add("pinballs.generic");
          //  dataPaths.Add("stadium.generic");
            dataPaths.Add("goalkeepers.1_100");
            //thumbs
            dataPaths.Add("thumbs.1_100");
            // audios:
            string lang = Data.Instance.langsManager.GetLang();
            dataPaths.Add(lang + "/voices.1_100"); // relatos
            dataPaths.Add(lang + "/goals.1_100");
            dataPaths.Add(lang + "/players.1_100"); // names
            dataPaths.Add(lang + "/goalkeepers.1_100"); // names
            dataPaths.Add(lang + "/referis.1_100"); // names

            dataPaths.Add("sheets");// google sheets Data
            dataPaths.Add("jsons");// jsons data

            totalFirstBundles = dataPaths.Count;
            bundles = new Dictionary<string, AssetBundle>();
        }
        public string GetHashFor(string key)
        {
            return manifest.GetAssetBundleHash(key).ToString();
        }
        public float DownloadedMegas()
        {
            return downloadedBytes / 1000000.0f;
        }
        AssetBundleManifest manifest;
        Action<string> onSuccess;
        AssetBundle mainBundle;
        int manifestHash = 0;

        public IEnumerator DownloadAll(string _url, Action<string> onSuccess)
        {
            loadedHashes = "";
            Events.OnLoading("Bundles");
            this.url = _url + mainBundlePath + "/";

            if (dataPaths == null)
            {
                SetAssetsBundleServer();
                isFirstTime = true;
            }
            else
            {
                isFirstTime = false;
            }

            Debug.Log("Downloading isFirstTime: " + isFirstTime + "   mainBundle " + mainBundle + " dataPaths: " + dataPaths);
            this.onSuccess = onSuccess;

            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url + mainBundlePath))
            {
                var cert = new ForceAcceptAll();
                request.certificateHandler = cert;
                Debug.Log("Loading from url : " + url + mainBundlePath);
                AsyncOperation op = request.SendWebRequest();
                while (!op.isDone)
                {
                    downloadProgress = request.downloadProgress;
                    downloadedBytes = request.downloadedBytes;
                    Events.OnLoadingProgress(request.downloadProgress);
                    yield return new WaitForEndOfFrame();
                }
                Debug.Log("Loading Manifest done");
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error + " url: " + url + mainBundlePath);
                    onSuccess("error");
                }
                else
                {
                    mainBundle = DownloadHandlerAssetBundle.GetContent(request);
                    manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    int hash = manifest.GetHashCode();
                    if (hash == manifestHash)
                    {
                        onSuccess("nothing_new");
                        yield return null;
                    }
                    else
                    {
                        manifestHash = hash;
                        StartCoroutine(LoadBundlesFromManifest());
                    }
                    mainBundle.Unload(false);
                }
            }

        }


        public IEnumerator LoadBundlesFromManifest()
        {
            int bundleID = 0;
            foreach (string key in dataPaths)
            {
                Debug.Log("__________ " + key);
                Hash128 hash = manifest.GetAssetBundleHash(key);
                if (isFirstTime)
                {
                    loadedHashes += hash.ToString() + "_";
                    Debug.Log("__________ " + key + " loadedHashes: " + hash.ToString());
                }
                else
                {
                    if (CheckIfHashIsNew(hash.ToString(), bundleID))
                    {
                        StopAllCoroutines();
                        // Data.Instance.ResetAll();
                    }
                }
                bundleID++;
                if (isFirstTime)
                {
                    CurrentPack = key;
                    yield return DownloadAndCacheAssetBundle(key, hash, OnLoaded);
                }
            }
            if (!isFirstTime)
                onSuccess("nothing_new");
        }
        bool CheckIfHashIsNew(string hash, int id)
        {
            string[] arr = loadedHashes.Split("_"[0]);
            if (id < arr.Length)
            {
                Debug.Log("saved hash: " + arr[id] + "   the hash: " + hash);
                if (arr[id] == hash)
                    return false;
            }
            return true;
        }

        int loadedParts = 0;
        void OnLoaded(bool isLoaded)
        {
            loadedParts++;
            if (loadedParts >= dataPaths.Count)
                onSuccess("ok");
        }
        IEnumerator DownloadAndCacheAssetBundle(string uri, Hash128 hash, System.Action<bool> OnLoaded)
        {
            Events.OnLoading(uri);
            string realURL = url + uri;
            Debug.Log("Load: " + realURL);
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(realURL, hash);

            var cert = new ForceAcceptAll();
            uwr.certificateHandler = cert;

            using (uwr)
            {
                var operation = uwr.SendWebRequest();
          
                while (!operation.isDone)
                {
                    Events.OnLoadingProgress(uwr.downloadProgress);
                    yield return null;
                }
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error downloading assetBundle: " + realURL);
                   // onSuccess("error");
                    yield break;
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);                    

                    if (bundles == null)
                        bundles = new Dictionary<string, AssetBundle>();
                    bundles.Add(uri, bundle);

                    if(OnLoaded != null)
                        OnLoaded(true);
                    OnLoaded = null;
                   // bundle.Unload(false);
                }
            }
        }





        public GameObject GetAsset(string bundleName, string asset)
        {
           // Debug.Log("GET Asset  bundleName: " + bundleName + " asset: " + asset);
            AssetBundle assetBundle = bundles[bundleName];
            GameObject go = assetBundle.LoadAsset(asset) as GameObject;
            return go;
        }
        public TextAsset GetAssetAsText(string bundleName, string asset)
        {
            //   print("GET Asset  bundleName: " + bundleName + " asset: " + asset);
            AssetBundle assetBundle = bundles[bundleName];
            TextAsset go = assetBundle.LoadAsset(asset) as TextAsset;
            return Instantiate(go);
        }
        public AudioClip GetAssetAsAudioClip(string bundleName, string asset)
        {
            //Debug.Log("Get AudioClip: " + bundleName + " asset: " + asset);
            AssetBundle assetBundle = bundles[bundleName];
            AudioClip go = assetBundle.LoadAsset(asset) as AudioClip;
            return go;
        }
        public Texture2D GetAssetAsTexture2D(string bundleName, string asset)
        {
            //Debug.Log("Get sprite: " + bundleName + " asset: " + asset);
            AssetBundle assetBundle = bundles[bundleName];
            Texture2D go = assetBundle.LoadAsset(asset) as Texture2D;
            return go;
        }

        public string GetSheetText(string asset) {
            if (!bundles.ContainsKey("sheets")) { Debug.LogError("Bundle not contains 'sheets' key"); return ""; }
            AssetBundle assetBundle = bundles["sheets"];
            if (assetBundle.Contains(asset)) {
                TextAsset go = assetBundle.LoadAsset("assets/assetbundles/" + asset) as TextAsset;
                return go.text;
            }else
                return "";
        }

        public string GetJsonText(string asset) {
            AssetBundle assetBundle = bundles["jsons"];
            TextAsset go = assetBundle.LoadAsset("assets/assetbundles/" + asset) as TextAsset;
            return go.text;
        }
    }
}