using Fulbo.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SkipSignal : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        public bool isOn;
        public float timer = 0;
        public float totalTime = 0.5F;
        [SerializeField] private Image progressBar;
        System.Action OnReady;
        public bool pressed;

        void Start()
        {
            panel.SetActive(false);
#if UNITY_ANDROID || UNITY_IOS 
                Destroy(panel.gameObject);
                Destroy(this);
                return;
#endif
            Events.OnSkipOn += OnSkipOn;
            Events.OnSkipOff += OnSkipOff;

            InputManager.Instance.OnButtonPressed += OnButtonPressed;
            InputManager.Instance.OnButtonReleased += OnButtonReleased;
        }
        void OnSkipOff()
        {
            pressed = false;
            isOn = false;
            panel.SetActive(false);
            Reset();
        }
        private void Update()
        {
            if (!isOn || !pressed) return;

            SetProgress();
        }
        void OnButtonPressed(int buttonID, int playerID)
        {
            if (!isOn || Time.timeScale == 0) return;
            panel.SetActive(true);
            panel.GetComponent<Animation>().Play("skipSignal_on");
            pressed = true;
        }
        void OnButtonReleased(int buttonID, int playerID)
        {
            if (!isOn) return;
            Reset();
        }
        void OnSkipOn(System.Action OnReady, string text = "")
        {
            isOn = true;
            this.OnReady = OnReady;
            Reset();
            pressed = false;
        }
        private void Reset()
        {
            pressed = false;
            timer = 0;
            panel.GetComponent<Animation>().Play("skipSignal_off");
        }
        void SetProgress()
        {
            timer += Time.deltaTime;
            progressBar.fillAmount = timer / totalTime;
            if (timer >= totalTime)
            {
                Done();
            }
        }
        void Done()
        {
            isOn = false;
            OnSkipOff();
            OnReady();
        }
    }
}
