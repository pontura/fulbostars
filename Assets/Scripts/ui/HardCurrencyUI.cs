using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class HardCurrencyUI : MonoBehaviour
    {
        [SerializeField] Text field;
        Animator anim;
        [SerializeField] UIParticleSystem uiParticleSystem;
        float lastScore;
        int score = 0;

        states state;
        enum states
        {
            IDLE,
            UPDATING // particles flying            
        }

        void Start()
        {
            anim = GetComponent<Animator>();
            Invoke("RefreshScore", 0.1f);
            ForceScore(DB.DBManager.Instance.DbUserData.data.hard_currency, "");

            Events.RefreshHardCurrency += RefreshHardCurrency;
            Events.AddHardScore += AddHardScore;// add some score to main-score
            Events.OnFlyingParticles += OnFlyingParticles;
            Events.OnFlyingPArrives += OnFlyingPArrives;

            if (score == 0)
                gameObject.SetActive(false);
        }
        void OnDestroy()
        {
            Events.RefreshHardCurrency -= RefreshHardCurrency;
            Events.AddHardScore -= AddHardScore;
            Events.OnFlyingParticles -= OnFlyingParticles;
            Events.OnFlyingPArrives -= OnFlyingPArrives;
        }

        private void OnFlyingParticles(int arg1, FlyingParticlesUI.types type, Vector2 arg3, float b,float arg4)
        {
            if (type != FlyingParticlesUI.types.HARD) return;
            state = states.UPDATING;
        }

        private void OnFlyingPArrives(FlyingParticlesUI.types type, float percent, float init, float final)
        {
            if (type != FlyingParticlesUI.types.HARD) return;
            lastScore = -1;
            float _score = final;

            int coinID = (int)Mathf.Round(percent * 7);
            if (coinID > 7) coinID = 7;
            if (coinID < 1) coinID = 1;
            string soundName = "ui/hardCurrency/ui_diamond" + coinID;

            if (percent==1)
            {
                state = states.IDLE;                
                if (scoreFromServer!=0)
                    _score = scoreFromServer;
                scoreFromServer = 0;
            } else
            {
                _score = init + (int)((float)(final - init) * percent);
            }
           
            ForceScore(_score, "scoreUp", soundName);

        }
        public void EmptyField()
        {
            field.text = "";
        }
        void AddHardScore(int scoreToAdd) // add some score to main-score
        {
            score += scoreToAdd;
            SetScore(score);
            if(scoreToAdd>0)
                SetAnim();
        }
        int scoreFromServer;
        void RefreshHardCurrency(int score)
        {
            scoreFromServer = score;
            if (state == states.UPDATING) return;
            if (lastScore < score)
                ForceScore(score, "scoreUp");
            else
                ForceScore(score, "scoreDown");
        }
        void ResetParticles()
        {
            if (uiParticleSystem != null)
            {
            }
        }
        public void ForceScore(float score, string animName, string audioName = "ui/game_grab_coin")
        {
            SetScore(score);
            if (field.text == "")
                field.text = "0";

            if(lastScore == 0)
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
        void SetScore(float score)
        {
            this.score = (int)score;
            field.text = Utils.FormatNumbers(this.score, false);
        }
        public void SetAnim()
        {
            if (uiParticleSystem != null)
            {
                CancelInvoke();
                uiParticleSystem.Play();
                Invoke("ResetParticles", 2);
            }
            anim.Play("scoreUp", 0,0);
        }
    }
}