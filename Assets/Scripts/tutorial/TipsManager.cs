using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo.Game.Tutorial
{
    public class TipsManager : MonoBehaviour
    {
        public Data[] all;

        [Serializable]
        public class Data
        {
            public Types type;
            public bool done;
        }
        public enum Types
        {
            DRAG_CHARACTERS,
            REPLACE_CHARACTERS,
            CHARACTERS_VIEW_STATS
        }
        [SerializeField] bool dragCharacters;

        void Start()
        {
            Events.CheckTip += CheckTip;
            foreach (Data data in all)
            {
                if (PlayerPrefs.GetInt(data.type.ToString()) == 1)
                    data.done = true;
            }
        }
        void OnDestroy()
        {
            Events.CheckTip -= CheckTip;
        }
        void CheckTip(Types type, System.Action OnDone)
        {
            Data data = GetTip(type);
            if (data != null && !data.done)
            {
                data.done = true;
                PlayerPrefs.SetInt(data.type.ToString(), 1);
                string text = Fulbo.Data.Instance.texts.Get(data.type.ToString());
                Events.OnPopup(text, OnDone);

            }
        }
        Data GetTip(Types type)
        {
            foreach (Data d in all)
            {
                if (d.type == type)
                    return d;
            }
            return null;
        }
    }
}
