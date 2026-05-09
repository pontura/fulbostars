using UnityEngine;
using System;

namespace Fulbo.Game.Tutorial
{
    public class TutorialData : MonoBehaviour
    {
        static TutorialData mInstance = null;

        public StepData[] all;

        [Serializable]
        public class StepData
        {
            public Sprite image;
            public GameObject asset_to_add;
        }

        void Awake()
        {
            if (!mInstance)
                mInstance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
        }
        public static TutorialData Instance { get { return mInstance; } }

        public StepData GetStepData(int id)
        {
            return all[id];
        }
    }
}