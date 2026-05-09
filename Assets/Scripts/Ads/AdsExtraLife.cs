using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Ads
{
    public class AdsExtraLife : MonoBehaviour
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
            Events.AdsCheckForExtraLife += AdsCheckForExtraLife;
            closeButton.Init(0, OnClicked);
            watchButton.Init(1, OnClicked);
            cancelButton.Init(0, OnClicked);
            Close();
        }
        private void OnDestroy()
        {
            Events.AdsCheckForExtraLife -= AdsCheckForExtraLife;
        }
        void AdsCheckForExtraLife(System.Action<bool> OnReady)
        {
            field.text = Data.Instance.texts.Get("watch_video_for_extralife");
            watchButton.SetText(Data.Instance.texts.Get("buyEnergyTitle_1"));
            cancelButton.SetText(Data.Instance.texts.Get("no_thanks"));
            this.OnReady = OnReady;
            panel.SetActive(true);

            if (Time.timeScale > 0)
                this.timeScale = Time.timeScale;

            Time.timeScale = 0;
        }
        void OnClicked(int id)
        {
            switch (id)
            {
                case 0:
                    OnReady(false);
                    break;
                case 1:
                    Events.AdsWatchVideo(OnReady);
                    break;
            }
            Close();
        }
        void Close()
        {
            if (timeScale > 0)
                Time.timeScale = timeScale;
            else Time.timeScale = 1;
            panel.SetActive(false);
        }
    }
}
