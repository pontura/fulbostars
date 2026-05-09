using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameRecorder
{
    public class Manager : MonoBehaviour
    {
        static Manager mInstance;
        public TimeLine timeLine;
        public Fulbo.Game.Xtras.AutomaticPlayByRecorder automaticPlayByRecorder;
        public Fulbo.Game.Xtras.KeyframeRecorder KeyframeRecorder;
        public states state; 
        public enum states
        {
            IDLE,
            RECORDING,
            PLAYING
        }
        public static Manager Instance()
        {
            return mInstance;

        }
        private void Awake()
        {
            if (!mInstance)
                mInstance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            mInstance = this;          
            DontDestroyOnLoad(this.gameObject);
            automaticPlayByRecorder = GetComponent<Fulbo.Game.Xtras.AutomaticPlayByRecorder>();
            KeyframeRecorder = GetComponent<Fulbo.Game.Xtras.KeyframeRecorder>();
            Reset();
        }
        public void InitRecording()
        {
            state = states.RECORDING;
            timeLine = new TimeLine();
            timeLine.NewTimeline();
            Reset();
            KeyframeRecorder.enabled = true;
        }
        public void InitPlaying()
        {
            state = states.PLAYING;
            Reset();
            automaticPlayByRecorder.enabled = true;
        }
        private void Reset()
        {
            automaticPlayByRecorder.enabled = false;
            KeyframeRecorder.enabled = false;
        }
    }
}
