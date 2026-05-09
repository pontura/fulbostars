using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] Text field;
        Animator anim;
        [SerializeField] bool isMain;
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
            if (isMain)
            {
                anim = GetComponent<Animator>();
                Events.RefreshScore += RefreshScore;
                Invoke("RefreshScore", 0.1f);
                ForceScore(DB.DBManager.Instance.DbUserData.data.score, "");
                Events.AddScore += AddScore;// add some score to main-score
                Events.ScoreFreezed += ScoreFreezed;
                Events.OnFlyingParticles += OnFlyingParticles;
                Events.OnFlyingPArrives += OnFlyingPArrives;
            }

            if (lastScore == 0 && field.text == "")
                field.text = "0";
        }
        void OnDestroy()
        {
            if (isMain)
            {
                Events.RefreshScore -= RefreshScore;
                Events.ScoreFreezed -= ScoreFreezed;
                Events.AddScore -= AddScore;
                Events.OnFlyingParticles -= OnFlyingParticles;
                Events.OnFlyingPArrives -= OnFlyingPArrives;
            }
        }
        void ScoreFreezed(bool isFreezed)
        {
            if (!isMain) return;
            if (isFreezed) state = states.UPDATING;
            else state = states.IDLE;
        }
        private void OnFlyingParticles(int arg1, FlyingParticlesUI.types type, Vector2 arg3, float b, float arg4)
        {
            if (type != FlyingParticlesUI.types.COINS) return;
            state = states.UPDATING;
        }

        private void OnFlyingPArrives(FlyingParticlesUI.types type, float percent, float init, float final)
        {
            if (type != FlyingParticlesUI.types.COINS) return;
            lastScore = -1;
            float _score = final;

            int coinID = (int)Mathf.Round(percent * 7);
            if (coinID > 7) coinID = 7;
            if (coinID < 1) coinID = 1;
            string soundName = "ui/coins/ui_coin_bell" + coinID;

            if (percent==1) // ready:
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
        void AddScore(int scoreToAdd) // add some score to main-score
        {
            score += scoreToAdd;
            SetScore(score);
            lastScore = score;
            if (scoreToAdd>0)
                SetAnim();
        }
        int scoreFromServer;
        void RefreshScore(int score)
        {
            if (isMain)
            {
               // print("_____________RefreshScore " + score + " state: " + state);
                scoreFromServer = score;
                if (state == states.UPDATING) return;
            }
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
            //if (isMain)
            //    print("_____________ForceScore " + score + " state: " + state + " anim: " + animName);
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
            //print("SET SCORE: " + score);
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