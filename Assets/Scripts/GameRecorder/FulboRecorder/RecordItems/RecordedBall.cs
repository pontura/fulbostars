using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class RecordedBall : RecordedItem
    {
        Vector3 ballOriginalPos;
        Vector3 ballDestination;
        float framerate = 0.1f;

        public override void Reset()
        {
            base.Reset();
            ballDestination = Vector3.zero;
            StopTween();
        }
        public override void StopTween() {
            lastTimeChecked = 1000;
        }
        float lastTimeChecked;
        public override void Updated(float now)
        {
            if (ball.character != null) return;
            if (GameManager.Instance.state != GameManager.states.PLAYING) return;
            if (timeline.ball.Count <= keyframeID - 1) return;
            if (ball == null) return;

            GameRecorder.KeyFrame.KeyFrameBallData data = timeline.ball[keyframeID];
            if (data != null && data.time < now)
            {
                ballOriginalPos = new Vector3(data.x, data.y, data.z);
                keyframeID++;
                if (timeline.ball.Count <= keyframeID - 1) return;
                ballDestination = ballOriginalPos;
                GameRecorder.KeyFrame.KeyFrameBallData dest = timeline.ball[keyframeID];

                if (dest != null)
                {
                    framerate = dest.time - data.time;
                    ballDestination = new Vector3(dest.x, dest.y, dest.z);
                    lastTimeChecked = 0;
                }
                ball.transform.position = ballOriginalPos;
            }
            else
            {
                if (lastTimeChecked > framerate) return;
                lastTimeChecked += Time.deltaTime;
                float lerpValue = lastTimeChecked / framerate;
                ball.transform.position = Vector3.Lerp(ballOriginalPos, ballDestination, lerpValue);
            }
        }
    }
}
