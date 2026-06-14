using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void TrackGA4Event(string eventName, string jsonParams);
    #endif

    void Start()
    {
        Events.OnTrack += OnTrack;
    }

    void OnDestroy()
    {
        Events.OnTrack -= OnTrack;
    }

    private void OnTrack(string trackName, Dictionary<string, object> dictionary)
    {
        dictionary["platform"] = Application.platform.ToString();
        dictionary["session_id"] = SystemInfo.deviceUniqueIdentifier;

        string json = DictionaryToJson(dictionary);
        trackName = trackName.ToLower();
        #if UNITY_WEBGL && !UNITY_EDITOR
            TrackGA4Event(trackName, json);
        #else
            Debug.Log($"[GA4] Tracked: {trackName} → {json}");
        #endif
    }

    private string DictionaryToJson(Dictionary<string, object> dict)
    {
        var entries = new System.Text.StringBuilder("{");
        bool first = true;

        foreach (var kvp in dict)
        {
            if (!first) entries.Append(",");
            first = false;

            string key = $"\"{kvp.Key}\"";
            string value = kvp.Value switch
            {
                string s => $"\"{s}\"",
                bool b   => b ? "true" : "false",
                null     => "null",
                _        => kvp.Value.ToString()
            };

            entries.Append($"{key}:{value}");
        }

        entries.Append("}");
        return entries.ToString();
    }
}