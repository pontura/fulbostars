using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class UIMainScreen : MonoBehaviour
    {
        void Awake()
        {
            Events.OnButtonClick += OnButtonClick;
            Events.OnSkipButtonPress += OnSkipButtonPress;
        }
        void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
            Events.OnSkipButtonPress -= OnSkipButtonPress;
        }
        public virtual void OnButtonClick(int buttonID, int playerID)  {  }
        void OnSkipButtonPress()
        {
            if (Data.Instance.popupManager.isOn)
                Data.Instance.popupManager.OnClick();
            else if (Data.Instance.onBoardingManager.state != Onboarding.OnBoardingManager.states.OFF)
                Data.Instance.onBoardingManager.OnSkipButtonPress();
            else
                OnSkipButtonPressed();
        }
        public virtual void OnSkipButtonPressed()  {  }
    }
}
