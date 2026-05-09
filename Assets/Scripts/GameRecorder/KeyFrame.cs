using UnityEngine;
using System.Collections;
using System;

namespace GameRecorder
{
    [Serializable]
    public class KeyFrame
    {
        [Serializable]
        public class KeyFrameData
        {
            public float time;
            public float x;
            public float z;
        }
        [Serializable]
        public class KeyFrameCharacterData : KeyFrameData
        {
            public string action;
            public int characterID;
            public int teamID;
        }
        [Serializable]
        public class KeyFrameBallData : KeyFrameData
        {
            public float y;
        }
        [Serializable]
        public class KeyFrameKeyValue
        {
            public float time;
            public string value;
        }
        [Serializable]
        public class KeyFrameEventData
        {
            public float time;
            public string key;
            public string value;
            public int characterID;
            public int teamID;
        }
    }
}
