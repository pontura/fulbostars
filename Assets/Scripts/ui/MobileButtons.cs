using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class MobileButtons : MonoBehaviour
    {
        [SerializeField] Text paseField;
        [SerializeField] Text kickField;

        [SerializeField] Image buttonPase;
        [SerializeField] Image buttonKick;
        [SerializeField] Image mobileJoystickCenter;
        [SerializeField] GameObject panel;
        [SerializeField] GameObject screenshotButton;

        //public Sprite pase;
        //public Sprite kick;
        //public Sprite hit;
        //public Sprite dash;

        void Start()
        {
            if (!Data.Instance.isMobile)
                Destroy(this);
            else
            {
                panel.SetActive(true);
                Events.CharacterCatchBall += CharacterCatchBall;
                Events.OnBallKicked += OnBallKicked;
                Events.OnGameStatusChanged += OnGameStatusChanged;
                if(Data.Instance.myTeam.GetGamesPlayed()==0)
                {
                    if (screenshotButton != null)
                        screenshotButton.SetActive(false);
                }
            }
        }

        void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnBallKicked -= OnBallKicked;
            Events.OnGameStatusChanged -= OnGameStatusChanged;
        }
        void OnGameStatusChanged(Fulbo.Game.GameManager.states state)
        {
            if (state == Fulbo.Game.GameManager.states.GOAL)
            {
                panel.transform.localScale = Vector2.zero;
                mobileJoystickCenter.transform.localPosition = Vector3.zero;
            }
            else if (state == Fulbo.Game.GameManager.states.PLAYING)
                panel.transform.localScale = Vector2.one;
        }
        void OnBallKicked(CharacterStates.kickTypes t, float a, Character ch)
        {
            Idle();
        }
        void CharacterCatchBall(Character character)
        {
            if (character.teamID == 1)
                Idle();
            else
                Attacking();
        }
        void Idle()
        {
            //buttonPase.sprite = hit;
            //buttonKick.sprite = dash;

            paseField.text = Data.Instance.texts.Get("button_pass_2");
            kickField.text = Data.Instance.texts.Get("button_shoot_2");
        }
        void Attacking()
        {
            //buttonPase.sprite = pase;
            //buttonKick.sprite = kick;

            paseField.text = Data.Instance.texts.Get("button_pass");
            kickField.text = Data.Instance.texts.Get("button_shoot");
        }
    }
}
