using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public Image image;
        [SerializeField] GameObject panel;
        float duration;
        float timer;
        System.Action OnDone;
        bool stopped;

        public types type;
        public enum types
        {
            SIMPLE,
            LOOP,
            FREEZED
        }


        public void SetOff()
        {
            image.fillAmount = 0;
            panel.SetActive(false);
            OnDone = null;
            stopped = true;
        }

        public void Init(float duration, System.Action OnDone)
        {
            panel.SetActive(true);
            this.OnDone = OnDone;
            this.duration = duration;
            timer = 0;
            stopped = false;
        }
        public void SetNormalized(float value)
        {
            type = types.FREEZED;
            panel.SetActive(true);
            image.fillAmount = value;
        }
        private void Update()
        {
            if (type == types.FREEZED) return;
            if (stopped) return;
            switch(type)
            {
                case types.SIMPLE:UpdateSimple(); break;
                case types.LOOP:UpdateLoop(); break;
            }        
        }
        private void UpdateSimple()
        {
            timer += Time.deltaTime; ///Time.timeScale;
            //print("UpdateSimple " + timer + "  delta: " + Time.deltaTime + "   timescale: " + Time.timeScale + " duration: " + duration);
            if (timer >= duration)
            {
                if (OnDone != null)
                    OnDone();
                timer = duration;
                SetOff();
            }
            image.fillAmount = timer / duration;
        }
        private void UpdateLoop()
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                timer = 0;
            }
            image.fillAmount = timer / duration;
        }
        public float GetValue()
        {
            return image.fillAmount;
        }
        public void Stop()
        {
            stopped = true;
        }
    }
}
