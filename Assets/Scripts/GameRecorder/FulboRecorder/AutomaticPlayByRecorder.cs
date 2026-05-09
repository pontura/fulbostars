using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class AutomaticPlayByRecorder : MonoBehaviour
    {
        RecordedCharacters recordedCharacters;
        RecordedBall recordedBall;
        RecordedVoices recordedVoices;
        RecordedEvents recordedEvents;

        void Start()
        {
            Events.GameOver += GameOver;
        }
        void OnDestroy()
        {
            Events.GameOver -= GameOver;
        }
        void GameOver()
        {
            Reset();
        }
        public void Init() { 
            recordedCharacters = new RecordedCharacters();
            recordedBall = new RecordedBall();
            recordedVoices = new RecordedVoices();
            recordedEvents = new RecordedEvents();
            recordedCharacters.Init(GameRecorder.Manager.Instance().timeLine);
            recordedBall.Init(GameRecorder.Manager.Instance().timeLine);
            recordedVoices.Init(GameRecorder.Manager.Instance().timeLine);
            recordedEvents.Init(GameRecorder.Manager.Instance().timeLine);
            Reset();
        }
        public void Reset()
        {
            recordedCharacters.Reset();
            recordedBall.Reset();
            recordedVoices.Reset();
            recordedEvents.Reset();
        }
        public void StopAllTweens()
        {
            recordedCharacters.StopTween();
            recordedBall.StopTween();
        }
        void Update()
        {
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.recordType != GameManager.recordTypes.PLAYING) return;
            if (GameManager.Instance.state != GameManager.states.PLAYING) return;

            float now = Data.Instance.matchData.time;

            recordedEvents.Updated(now);
            //if (lastEvent == "goal") return;

            recordedCharacters.Updated(now);
            recordedBall.Updated(now);
            recordedVoices.Updated(now);
        }
    }
}