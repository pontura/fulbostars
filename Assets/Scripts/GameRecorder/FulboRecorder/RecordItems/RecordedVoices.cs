using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Xtras
{
    public class RecordedVoices : RecordedItem
    {
        public override void Updated(float now)
        {
            if (timeline.voices.Count <= keyframeID - 1) return;
            GameRecorder.KeyFrame.KeyFrameKeyValue data = timeline.voices[keyframeID];
            if (data != null && data.time < now)
            {
                keyframeID++;
                Events.OnRelatorSayRecorded(data.value);
            }
        }
    }
}
