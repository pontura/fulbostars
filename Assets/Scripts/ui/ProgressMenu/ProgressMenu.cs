using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fulbo.UI
{
    public class ProgressMenu : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] ProgressMenuItem item;

        public List<ItemData> all;
        public List<ProgressMenuItem> items;

        [Serializable] public class ItemData
        {
            public states state;
            public enum states
            {
                ACTIVE,
                INACTIVE,
                ON
            }
        }
        [SerializeField] ItemSettings itemSettings;
        [Serializable]
        public class ItemSettings
        {
            public Color color_active;
            public Color color_inactive;
            public Color color_on;
        }

        public void Init(List<ItemData> all)
        {
            if (items.Count > 0) // si ya existe reinicia
            {
                SetAllDone();
                SetProgress(0);
                return;
            }
            items.Clear();
            this.all = all;
            Utils.RemoveAllChildsIn(container);
            foreach(ItemData iData in all)
            {
                ProgressMenuItem i = Instantiate(item, container);
                i.Init(iData, itemSettings);
                items.Add(i);
            }
        }
        public virtual void SetProgress(int id)
        {
            int this_id = 0;
            foreach (ProgressMenuItem pmItem in items)
            {
                if (this_id == id)
                    pmItem.SetState(ItemData.states.ON, itemSettings);
                else if(this_id<id)
                    pmItem.SetState(ItemData.states.ACTIVE, itemSettings);
                else if (pmItem.state != ItemData.states.INACTIVE)
                    pmItem.SetState(ItemData.states.ACTIVE, itemSettings);
                this_id++;
            }
        }
        void SetAllDone()
        {
            foreach (ProgressMenuItem pmItem in items)
                pmItem.SetState(ItemData.states.ACTIVE, itemSettings);
        }
    }
}
