using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Analytics : MonoBehaviour
{
    private const string MeasurementId = "G-XZE5QWSEH4";
    private const string ApiSecret = "692W8braQh2dB2Rbe5b9Ng";
    private const string Endpoint = "https://www.google-analytics.com/mp/collect";

    private string clientId;

    void Start()
    {
        clientId = SystemInfo.deviceUniqueIdentifier;
        Events.OnTrack += OnTrack;
    }

    void OnDestroy()
    {
        Events.OnTrack -= OnTrack;
    }

    private void OnTrack(string trackName, Dictionary<string, object> dictionary)
    {
        dictionary["platform"] = Application.platform.ToString();
        StartCoroutine(SendToGA4(trackName, dictionary));
    }

    private IEnumerator SendToGA4(string eventName, Dictionary<string, object> parameters)
    {
        string url = $"{Endpoint}?measurement_id={MeasurementId}&api_secret={ApiSecret}";

        string payload = $@"{{
            ""client_id"": ""{clientId}"",
            ""events"": [{{
                ""name"": ""{eventName}"",
                ""params"": {DictionaryToJson(parameters)}
            }}]
        }}";

        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"[GA4] Error: {request.error}");
        else
            Debug.Log($"[GA4] Tracked: {eventName}");
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