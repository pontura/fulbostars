using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ConfirmPanel : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] ButtonCustom okBtn;
        [SerializeField] ButtonCustom cancelBtn;
        [SerializeField] ButtonCustom closeBtn;
        [SerializeField] Text title;
        [SerializeField] Text subtitle;

        [SerializeField] GameObject panel3buttons;
        [SerializeField] ButtonCustom ok3buttons;
        [SerializeField] ButtonCustom cancel3buttons;
        [SerializeField] ButtonCustom button3; // optional
        [SerializeField] Text title3buttons;
        [SerializeField] Text text3buttons;


        System.Action<bool> OnDone;
        System.Action<int> OnDone3;

        void Start()
        {
            Events.OnConfirmPanel += OnConfirmPanel;
            Events.OnConfirmPanel3Buttons += OnConfirmPanel3Buttons;
            Close();
        }
        void OnDestroy()
        {
            Events.OnConfirmPanel -= OnConfirmPanel;
            Events.OnConfirmPanel3Buttons -= OnConfirmPanel3Buttons;
        }
        void OnConfirmPanel3Buttons(string title, string text, System.Action<int> OnDone3, string confirm = "confirm", string cancel = "cancel", string btn3Text = "")
        {
            this.OnDone3 = OnDone3;
            title3buttons.text = title;
            text3buttons.text = text;

            ok3buttons.Init(1, ButtonClicked3, confirm);
            cancel3buttons.Init(2, ButtonClicked3, cancel);
            button3.Init(3, ButtonClicked3, btn3Text);

            panel3buttons.SetActive(true);
        }
        void ButtonClicked3(int id)
        {
            OnDone3(id);
            Close();
        }
        void OnConfirmPanel(string _title, string _subtitle, System.Action<bool> OnDone, string confirm = "confirm", string cancel = "cancel")
        {
            this.OnDone = OnDone;
            title.text = _title;
            subtitle.text = _subtitle;

            if (cancel == "")
                cancelBtn.gameObject.SetActive(false);
            else
            {
                cancelBtn.gameObject.SetActive(true);
                cancelBtn.Init(1, ButtonClicked, Data.Instance.texts.Get(cancel));
            }
            okBtn.Init(0, ButtonClicked, Data.Instance.texts.Get("yes"));
            closeBtn.Init(1, ButtonClicked);

            panel.SetActive(true);
        }
        void ButtonClicked(int id)
        {
            switch (id)
            {
                case 0:
                    OnDone(true); break;
                case 1:
                    OnDone(false); break;
            }
            Close();
        }
        void Close()
        {
            panel.SetActive(false);
            panel3buttons.SetActive(false);
        }
    }
}
