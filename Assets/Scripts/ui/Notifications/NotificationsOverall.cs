using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Notifications
{
    public class NotificationsOverall : MonoBehaviour
    {
        [SerializeField] GameObject panel;

        string key = "NotificationsOverall";
        System.Action OnDone;

        public void Init(System.Action OnDone)
        {
            SetState(false);
            OnDone();
            return;
            //if (PlayerPrefs.GetInt(key, 0) == 1)
            //{
            //    SetState(false);
            //    OnDone();
            //}
            //else
            //{
            //    this.OnDone = OnDone;
            //    SetState(true);
            //}
        }
        void SetState(bool isOn)
        {
            panel.SetActive(isOn);
        }
        //public void OnClicked()
        //{
        //    GetComponent<AudioSource>().Play();
        //    PlayerPrefs.SetInt(key, 1);
        //    SetState(false);
        //    OnDone();
        //}
    }
}
