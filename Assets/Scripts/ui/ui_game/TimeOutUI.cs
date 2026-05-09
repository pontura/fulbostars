using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TimeOutUI : MonoBehaviour
    {
      //  [SerializeField] Text field;
        [SerializeField] Image image;
        [SerializeField] GameObject panel;
        [SerializeField] Sprite[] nums;

        void Start()
        {
            panel.SetActive(false);
            Events.OnInitTimeout += OnInitTimeout;
            Events.OnGoal += OnGoal;
        }
        void OnDestroy()
        {
            Events.OnInitTimeout -= OnInitTimeout;
            Events.OnGoal -= OnGoal;
        }
        void OnGoal(int i, Character ch)
        {
            panel.SetActive(false);
        }
        void OnInitTimeout(bool isOn)
        {
            panel.SetActive(isOn);
            if (isOn)
                Loop();
            else
                CancelInvoke();
        }
        int lastSecond;
        void Loop()
        {
            int sec = Data.Instance.matchData.secs;
            if (sec < 1)
            {
                Done();
                return;
            }
            else if (sec != lastSecond)
            {
                if(sec > 6)
                    AudioManager.Instance.PlaySoundOneShot("ingame", "ui/game_clock");
                else if(sec > 3)
                    AudioManager.Instance.PlaySoundOneShot("ingame", "ui/game_clock6");
                else if(sec > 0)
                    AudioManager.Instance.PlaySoundOneShot("ingame", "ui/game_clock"+sec);

                panel.SetActive(true);
                image.enabled = true;
                lastSecond = sec;
                panel.GetComponent<Animation>().Play();

                if(sec-1 <= nums.Length)
                    image.sprite = nums[sec-1];
            }
            Invoke("Loop", 0.1f);
        }
        void Done()
        {
            image.enabled = false;
            CancelInvoke();
            panel.SetActive(false);
        }
    }
}
