using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PopupTextData : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] ButtonCustom closeBtn;
        [SerializeField] Text title;
        [SerializeField] Text subtitle;
        [SerializeField] Scrollbar scroll;

        System.Action<bool> OnDone;

        void Start()
        {
            Events.PopupText += PopupText;
            Close();
        }
        void OnDestroy()
        {
            Events.PopupText -= PopupText;
        }
        void PopupText(string _title, string _subtitle, System.Action<bool> OnDone)
        {
            print("PopupText");
            this.OnDone = OnDone;
            title.text = _title;
            subtitle.text = _subtitle;
          
            closeBtn.Init(1, ButtonClicked);

            panel.SetActive(true);

            scroll.value = 1;
        }
        void ButtonClicked(int id)
        {
            Close();
        }
        void Close()
        {
            print("Close PopupTextData");
            panel.SetActive(false);
        }
    }
}
