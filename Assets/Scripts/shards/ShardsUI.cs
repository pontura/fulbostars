using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ShardsUI : MonoBehaviour
    {
        [SerializeField] Text field;
        Animator anim;
        [SerializeField] UIParticleSystem uiParticleSystem;
        float lastScore;
        int value = 0;

        states state;

        enum states
        {
            IDLE,
            UPDATING // particles flying            
        }

        void Start()
        {
            anim = GetComponent<Animator>();
            Events.OnFlyingParticles += OnFlyingParticles;
            Events.OnFlyingPArrives += OnFlyingPArrives;

            RefreshDataFromServer();

            if (value == 0)
                gameObject.SetActive(false);

        }
        void RefreshDataFromServer()
        {
            value = DB.DBManager.Instance.DbUserData.data.shards;
            field.text = value.ToString();
        }
        void OnDestroy()
        {
            Events.OnFlyingParticles -= OnFlyingParticles;
            Events.OnFlyingPArrives -= OnFlyingPArrives;
        }
        private void OnFlyingParticles(int arg1, FlyingParticlesUI.types type, Vector2 arg3, float b, float arg4)
        {
            if (type != FlyingParticlesUI.types.SHARDS) return;
            state = states.UPDATING;
        }
        private void OnFlyingPArrives(FlyingParticlesUI.types type, float percent, float init, float final)
        {
            if (type != FlyingParticlesUI.types.SHARDS) return;
            lastScore = -1;
            float _score = final;

            //int coinID = (int)Mathf.Round(percent * 7);
            //if (coinID > 7) coinID = 7;
            //if (coinID < 1) coinID = 1;
            string soundName = "ui/coins/ui_coin_bell" + 1;

            if (percent == 1)
            {
                state = states.IDLE;
                RefreshDataFromServer();
            }
            else
            {
                _score = init + (int)((float)(final - init) * percent);
            }

            ForceValue(_score, "scoreUp", soundName);

        }
        public void EmptyField()
        {
            field.text = "";
        }
        void AddScore(int scoreToAdd) // add some score to main-score
        {
            value += scoreToAdd;
            SetValue(value);
            if (scoreToAdd > 0)
                SetAnim();
        }
        void ResetParticles()
        {
            if (uiParticleSystem != null)
            {
            }
        }
        public void ForceValue(float score, string animName, string audioName = "ui/game_grab_coin")
        {
            SetValue(score);
            if (field.text == "")
                field.text = "0";

            if (lastScore == 0)
            {
                lastScore = score;
                return;
            }
            if (lastScore == score) return;
            lastScore = score;
            if (uiParticleSystem != null)
            {
                CancelInvoke();
                if (animName == "scoreUp")
                {
                    AudioManager.Instance.ChangeVolume("fx", 1);
                    AudioManager.Instance.PlaySound("fx", audioName, false);
                    uiParticleSystem.Play();
                    Invoke("ResetParticles", 2);
                }
                else
                {
                    AudioManager.Instance.ChangeVolume("fx", 1);
                    AudioManager.Instance.PlaySound("fx", "ui/coin_reward", false);
                }
            }
            if (anim != null && animName != "")
            {
                anim.Play(animName, 0, 0);
            }
        }
        void SetValue(float _value)
        {
            this.value = (int)_value;
            field.text = Utils.FormatNumbers(this.value, false);
        }
        public void SetAnim()
        {
            if (uiParticleSystem != null)
            {
                CancelInvoke();
                uiParticleSystem.Play();
                Invoke("ResetParticles", 2);
            }
            anim.Play("scoreUp", 0, 0);
        }
    }
}