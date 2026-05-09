using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Ugi.PlayInstallReferrerPlugin;
#endif

public class InstallReferrer : MonoBehaviour
{
    string referrer = "";
    public string GetReferrer()  {  return referrer;  }

    private string txtInstallReferrerFromCallback;
    private string txtReferrerClickTimestampFromCallback;
    private string txtInstallBeginTimestampFromCallback;
    private string txtReferrerClickTimestampServerFromCallback;
    private string txtInstallBeginTimestampServerFromCallback;
    private string txtInstallVersionFromCallback;
    private string txtGooglePlayInstantFromCallback;

    void Start()
    {
#if UNITY_WEBGL
        var parameters = URLParameters.GetSearchParameters();
        string _referrer;
        if (parameters.TryGetValue("referrer", out _referrer))
        {
            this.referrer = _referrer;
        }
#elif UNITY_ANDROID
        PlayInstallReferrer.GetInstallReferrerInfo((installReferrerDetails) =>
        {
            Debug.Log("Install referrer details received!");

            // check for error
            if (installReferrerDetails.Error != null)
            {
                Debug.LogError("Error occurred!");
                if (installReferrerDetails.Error.Exception != null)
                {
                    Debug.LogError("Exception message: " + installReferrerDetails.Error.Exception.Message);
                }
                Debug.LogError("Response code: " + installReferrerDetails.Error.ResponseCode.ToString());
                return;
            }

            // print install referrer details
            if (installReferrerDetails.InstallReferrer != null)
            {
                txtInstallReferrerFromCallback = installReferrerDetails.InstallReferrer;
                Debug.Log("Install referrer: " + installReferrerDetails.InstallReferrer);
                referrer = installReferrerDetails.InstallReferrer;
            }
            if (installReferrerDetails.ReferrerClickTimestampSeconds != null)
            {
                txtReferrerClickTimestampFromCallback = installReferrerDetails.ReferrerClickTimestampSeconds.ToString();
                Debug.Log("Referrer click timestamp: " + installReferrerDetails.ReferrerClickTimestampSeconds);
            }
            if (installReferrerDetails.InstallBeginTimestampSeconds != null)
            {
                txtInstallBeginTimestampFromCallback = installReferrerDetails.InstallBeginTimestampSeconds.ToString();
                Debug.Log("Install begin timestamp: " + installReferrerDetails.InstallBeginTimestampSeconds);
            }
            if (installReferrerDetails.ReferrerClickTimestampServerSeconds != null)
            {
                txtReferrerClickTimestampServerFromCallback = installReferrerDetails.ReferrerClickTimestampServerSeconds.ToString();
                Debug.Log("Referrer click server timestamp: " + installReferrerDetails.ReferrerClickTimestampServerSeconds);
            }
            if (installReferrerDetails.InstallBeginTimestampServerSeconds != null)
            {
                txtInstallBeginTimestampServerFromCallback = installReferrerDetails.InstallBeginTimestampServerSeconds.ToString();
                Debug.Log("Install begin server timestamp: " + installReferrerDetails.InstallBeginTimestampServerSeconds);
            }
            if (installReferrerDetails.InstallVersion != null)
            {
                txtInstallVersionFromCallback = installReferrerDetails.InstallVersion;
                Debug.Log("Install version: " + installReferrerDetails.InstallVersion);
            }
            if (installReferrerDetails.GooglePlayInstant != null)
            {
                txtGooglePlayInstantFromCallback = installReferrerDetails.GooglePlayInstant.ToString();
                Debug.Log("Google Play instant: " + installReferrerDetails.GooglePlayInstant);
            }
        });
#endif
    }
}
