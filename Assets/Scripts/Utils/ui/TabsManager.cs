using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fulbo.UI
{
    public class TabsManager : MonoBehaviour
    {
        [SerializeField] List<ListItemData> all;
        [Serializable]
        public class ListItemData
        {
            public int id;
            public string name;
            public GameObject content;
            public float pos_scroll_x;
            public float _width;
        }
        public types type;
        public enum types
        {
            CLASSIC,
            SCROLLEABLE
        }
        public ListItemData GetListItemData(string goName)
        {
            foreach (ListItemData l in all)
                if (l.name == goName)
                    return l;
            return all[0];
        }
        [SerializeField] ButtonCustom button;
        public List<ButtonCustom> buttons;

        [SerializeField] Transform container;
        System.Action<ListItemData> OnTabClicked;

        public void InitScroll(System.Action<ListItemData> _OnTabClicked = null)
        {
            all = new List<ListItemData>();
            type = types.SCROLLEABLE;
            this.OnTabClicked = _OnTabClicked;
        }
        public void AddScrollButton(GameObject go, string name)
        {
            ListItemData l = new ListItemData();
            l.name = name;
            l.content = go;
            all.Add(l);
        }
        public void Init()
        {
            type = types.CLASSIC;
            SetButtons();
            Select(0);
        }
        public void SetButtons()
        {
            if (all.Count == 0) return;
            buttons = new List<ButtonCustom>();
            Utils.RemoveAllChildsIn(container);
            int id = 0;
            foreach (ListItemData data in all)
            {
                data.id = id;
                ButtonCustom b = Instantiate(button, container);
                string s = Data.Instance.texts.Get(data.name);
               // print(s);
                b.Init(id, Select, s);
                buttons.Add(b);
                id++;
            }
            id = 0;
            float totalWidth = 0;
            foreach (ListItemData data in all)
            {
                data._width = data.content.GetComponent<RectTransform>().sizeDelta.x;
                if (id == 0 || id == all.Count - 1)
                    totalWidth += data._width / 2;
                else
                    totalWidth += data._width;
                id++;
            }
            id = 0;
            float mids = 0;
            foreach (ListItemData data in all)
            {
                if (id == 0)
                    data.pos_scroll_x = 0;
                else if (id == all.Count - 1)
                    data.pos_scroll_x = 1;
                else
                {
                    mids += data._width / 2;
                    float percent = mids / totalWidth;
                    data.pos_scroll_x = percent;
                }
              //  print(id + "data.pos_scroll_x:" + data.pos_scroll_x + "   totalWidth:" + totalWidth);
                mids += data._width / 2;
                id++;
            }
        }
        public void Select(int id)
        {
            if (type == types.CLASSIC)
                SelectClassic(id);
            else if (type == types.SCROLLEABLE)
                SelectScrolleable(id);
        }
        public void SelectClassic(int id)
        {
            foreach (ButtonCustom b in buttons)
                b.OnSelected(false);
            foreach (ListItemData data in all)
                data.content.SetActive(false);
            buttons[id].OnSelected(true);
            all[id].content.SetActive(true);
        }
        public void SelectScrolleable(int id)
        {
            if (id < 0) { Debug.LogError("Tab id < 0"); return; }
            foreach (ButtonCustom b in buttons)
                b.OnSelected(false);
            if(id<buttons.Count)
                buttons[id].OnSelected(true); else Debug.LogError("Tab Button > Count");
            if (OnTabClicked != null && id < all.Count)
                OnTabClicked(all[id]);else Debug.LogError("Shop Tab Item > Count");
        }

        public void HighlightTab(float scrollValue) {
            List<ListItemData> lids = all.OrderBy(x => Mathf.Abs(scrollValue - x.pos_scroll_x)).ToList();
            if (lids.Count > 0) {
                foreach (ButtonCustom b in buttons)
                    b.OnSelected(lids[0].id==b.buttonID);
            }
        }
    }
}