using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class OutOfEnergyPopup : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] ButtonCustom closeBtn;
        [SerializeField] ButtonCustom buyBtn;
        [SerializeField] Text title;
        [SerializeField] Text subtitle;

        bool hasBeenActiveInThisSession;

        void Start()
        {
            Events.OpenOutOfEnergyPopup += OpenOutOfEnergyPopup;
            Close();
        }
        void OnDestroy()
        {
            Events.OpenOutOfEnergyPopup -= OpenOutOfEnergyPopup;
        }
        void OpenOutOfEnergyPopup()
        {
            //if (hasBeenActiveInThisSession)
            //    return;

            hasBeenActiveInThisSession = true;
            title.text = Data.Instance.texts.Get("no_energy_title");
            subtitle.text = Data.Instance.texts.Get("no_energy_subtitle");
            buyBtn.Init(1, ButtonClicked, Data.Instance.texts.Get("buy_energy"));
            closeBtn.Init(0, ButtonClicked);
            panel.SetActive(true);
        }
        void ButtonClicked(int id)
        {
            Close();
            switch (id)
            {
                case 0:
                    Close(); break;
                case 1:
                    Buy(); break;
            }
        }
        void Buy()
        {
            Events.BuyEnergyPopup(true);
            Close();
        }
        void Close()
        {
            panel.SetActive(false);
        }
    }
}
