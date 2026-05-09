using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class UIMain : MonoBehaviour
    {
        static UIMain mInstance = null;
        public TeamUI team1;
        public TeamUI team2;
        public GameObject all;
        public IngameMenu ingameMenu;
        [SerializeField] Text timerField;
        public PowerupUIButton powerupUIButton_team1;
        public PowerupUIButton powerupUIButton_team2;

        public static UIMain Instance
        {
            get { return mInstance; }
        }
        void Awake()
        {
            if (!mInstance)
                mInstance = this;
        }
        void Start()
        {
            if (Data.Instance.matchData.IsTutorial())
                all.SetActive(false);
            Events.OnGoal += OnGoal;
            Events.OnInitTimeout += OnInitTimeout;
            Loop();
            SetField();
        }
        public void Init()
        {
            if(powerupUIButton_team1 != null)
                powerupUIButton_team1.Init();

            if (powerupUIButton_team2 != null)
                powerupUIButton_team2.Init();

            if (!Data.Instance.matchData.IsTutorial())
                Data.Instance.ui.SetPauseButton(true);
        }
        void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
            Events.OnInitTimeout -= OnInitTimeout;
        }
        void OnInitTimeout(bool open)
        {
            if (!open)
            {
                Animation anim = timerField.GetComponent<Animation>();
                if (anim != null)
                    anim.Play();
                AudioManager.Instance.PlaySoundOneShot("ui", "ui/game_extra_time");
            } else {
                Animation anim = timerField.GetComponent<Animation>();
                if (anim != null)
                    anim.Play("timeAlarm");
            }
        }
        public void OnShow()
        {
            if (Data.Instance.matchData.IsTutorial())
                all.SetActive(false);
            else
            {
                AudioManager.Instance.PlaySoundOneShot("ui", "ui/game_hud");
                all.SetActive(true);
            }
        }
        void OnGoal(int i, Character c)
        {
            all.SetActive(false);
        }
        public Vector3 GetScore()
        {
            return new Vector2(Data.Instance.matchData.score.x, Data.Instance.matchData.score.y);
        }
        void Loop()
        {
            if (Fulbo.Game.GameManager.Instance.state == Fulbo.Game.GameManager.states.PLAYING)
            {
                SetField();
            }
            Invoke("Loop", 1);
        }
        void SetField()
        {
            // int hours = secs / 3600;
            int secs = Data.Instance.matchData.secs;
            int minutes = (secs % 3600) / 60;
            int seconds = (secs % 3600) % 60;

            if (timerField != null)
                timerField.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
}
