using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Ads
{
    public class AdsExtratime : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Text field;
        [SerializeField] ButtonCustom watchButton;
        [SerializeField] ButtonCustom cancelButton;
        [SerializeField] ButtonCustom closeButton;

        float timeScale;

        System.Action<bool> OnReady;

        private void Start()
        {
            Events.AdsCheckForExtraTime += AdsCheckForExtraTime;
            closeButton.Init(0, OnClicked);
            watchButton.Init(1, OnClicked);
            cancelButton.Init(0, OnClicked);
            Close();
        }
        private void OnDestroy()
        {
            Events.AdsCheckForExtraTime -= AdsCheckForExtraTime;
        }
        void AdsCheckForExtraTime(System.Action<bool> OnReady)
        {

            AudioManager.Instance.ChangeVolume("music2", 1);
            AudioManager.Instance.PlaySound("music2", "music/music_extra_time", true);

            if (DB.DBManager.Instance.DbUserData.data.gameData.cups.IsPlayingLastLife())
                field.text = Data.Instance.texts.Get("watch_video_for_extratime_lastlife");
            else
                field.text = Data.Instance.texts.Get("watch_video_for_extratime");            
            
            watchButton.SetText(Data.Instance.texts.Get("watch_video"));
            cancelButton.SetText(Data.Instance.texts.Get("no_thanks"));
            this.OnReady = OnReady;
            panel.SetActive(true);

            if (Time.timeScale > 0)
                this.timeScale = Time.timeScale;

            Time.timeScale = 0;
        }
        void OnClicked(int id)
        {
            Close();
            switch (id)
            {
                case 0:
                    OnReady(false);
                    break;
                case 1:
                    Events.AdsWatchVideo(OnReady);
                    break;
            }
            
        }
        void Close()
        {
            AudioManager.Instance.PlaySound("music2", "", false);
            if (timeScale > 0)
                Time.timeScale = timeScale;
            else Time.timeScale = 1;
            panel.SetActive(false);
        }
    }
}
