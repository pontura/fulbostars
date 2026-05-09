using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using yutokun;

namespace Fulbo {
    public class SheetLoader : MonoBehaviour {
        [Serializable]
        public class Line {
            public string[] data;
        }
        public void LoadFromTo(string googleURL, System.Action<List<List<string>>> onDone, yutokun.Delimiter delimiter = yutokun.Delimiter.Tab) {
            Debug.Log("[URL]: " + googleURL);
            StartCoroutine(GetData(googleURL, onDone, delimiter));
        }
        IEnumerator GetData(string url, System.Action<List<List<string>>> onDone, yutokun.Delimiter delimiter = yutokun.Delimiter.Tab) {
            using (WWW www = new WWW(url)) {
                yield return www;
                if (www.error != null) {
                    StartCoroutine(GetData(url, onDone));
                    Events.OnPopup("Connection error. Lets try again...", null);
                } else if(onDone != null)
                {
                    //Debug.Log(www.text);
                    onDone(CSVParser.LoadFromString(www.text, delimiter));
                    onDone = null;
                }
            }
        }        
    }
}
