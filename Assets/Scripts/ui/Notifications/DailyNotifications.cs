using Fulbo.UI.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Notifications {

    public class DailyNotifications : MonoBehaviour {
        [SerializeField] Text field;
        DailyRewards dailyRewards;

        private void Awake() {
            Events.OnNoMoreAdsForToday += OnNoMoreAdsForToday;
            field.transform.parent.gameObject.SetActive(false);
        }

        private void Start() {
            dailyRewards = Data.Instance.ui.GetComponent<DailyRewards>();
            if (DB.DBManager.Instance.DbAds.NoMoreAdsForToday())
                OnNoMoreAdsForToday();
        }

        private void OnDestroy() {
            Events.OnNoMoreAdsForToday -= OnNoMoreAdsForToday;
        }


        void OnNoMoreAdsForToday() {
            SetClock();
        }

        void SetClock() {
            CancelInvoke();
            if (DB.DBManager.Instance.DbAds.IsANewDay()) {
                field.transform.parent.gameObject.SetActive(false);
                return;
            }            
            field.transform.parent.gameObject.SetActive(true);
            field.text = Utils.GetDayTimeCountdown(DB.DBManager.Instance.Now());
            Invoke("SetClock", 1);
        }

    }
}
