using System;
using UnityEngine;

namespace Fulbo.UI
{
    public class Controls : MonoBehaviour
    {
        void Start()
        {
            if(Data.Instance.hasTorneo)
                Events.OnButtonClick += OnButtonClick;
            else
                Events.OnSkipOn(OnSkip, "skip");
        }
        void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
        }
        private void OnButtonClick(int arg1, int arg2)
        {
            OnSkip();
        }

        public void OnSkip()
        {
            Events.OnButtonClick -= OnButtonClick;
            if(Data.Instance.tournamentsData.IsTournament())
                Data.Instance.LoadLevel("GameIntro");
            else
                Data.Instance.LoadLevel("SplashOptions");

            Events.OnSkipOff();
        }
    }

}
