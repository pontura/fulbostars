using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo.Pinballs
{
    public class PinballsManager : MonoBehaviour
    {
        static PinballsManager mInstance = null;

        public PinballData[] all;

        [Serializable]
        public class PinballData
        {
            public int id;
            public GameObject asset;
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
        private void Start()
        {
            Events.AddPinball += AddPinball;
        }
        private void OnDestroy()
        {
            Events.AddPinball -= AddPinball;
        }
        PinballData GetPinball(int id)
        {
            foreach(PinballData pinballData in all)
            {
                if (id == pinballData.id)
                    return pinballData;
            }
            return null;
        }
        void AddPinball(int id)
        {
            return;
            Debug.Log("Adding pinball id: " + id);
            if (id == 0) return;
            PinballData pd = GetPinball(id);

            if(pd != null)
            {               
                GameObject go = Instantiate(pd.asset, Fulbo.Game.GameManager.Instance.transform);
                go.transform.localPosition = Vector3.zero;  
            }
        }
        public static PinballsManager Instance { get { return mInstance; } }
    }
}
