using GameRecorder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class RecordedItem
    {
        protected int keyframeID = 0;

        protected Ball ball;
        protected CharactersManager charactersManager;
        protected TimeLine timeline;

        public void Init(TimeLine timeline)
        {
            this.timeline = timeline;
            charactersManager = GameManager.Instance.charactersManager;
            ball = GameManager.Instance.ball;
            OnInit();
        }
        public virtual void Reset()
        {
            keyframeID = 0;
        }
        public virtual void StopTween() { }
        public virtual void OnInit() { }
        public virtual void Updated(float now) { }
        public virtual void Record() { }
    }
}
