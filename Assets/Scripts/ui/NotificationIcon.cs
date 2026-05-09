using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fulbo;

namespace Fulbo.UI
{
    public class NotificationIcon : MonoBehaviour
    {
        [SerializeField] private GameObject iconAlertObj;

        public enum states
        {
            POST_MATCH_NEWS,
            DAILY_REWAWRD
        }
        public int id;

        public states state;

        private void Start() {
            Events.OnNoMoreAdsForToday += OnNoMoreAdsForToday;
        }

        private void OnDestroy() {
            Events.OnNoMoreAdsForToday -= OnNoMoreAdsForToday;
        }

        void OnNoMoreAdsForToday() {
            if (state == states.DAILY_REWAWRD)
                OnEnable();
        }

        public void Refresh()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            bool showIt = false;
            switch (state)
            {
                case states.POST_MATCH_NEWS:
                    if (Data.Instance.newScene == "MainMenu" && Data.Instance.matchData.cameFromMatch)
                    {
                        showIt = Data.Instance.matchData.cameFromMatch;
                        Data.Instance.matchData.cameFromMatch = false;
                    }
                    break;
                case states.DAILY_REWAWRD:
                    if (id == 0)
                        showIt = !DB.DBManager.Instance.DbAds.NoMoreAdsForToday();
                    else
                    {
                        int displayed = DB.DBManager.Instance.DbAds.data.adsDisplayed;
                        if (displayed < id) showIt = true;
                    }
                    break;
            }
            iconAlertObj.SetActive(showIt);
        }
    }
}