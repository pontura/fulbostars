using Fulbo.DB;
using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Energy.UI
{
    public class ScoreWinPopup : MonoBehaviour
    {
        int totalAvailables = 10;
        [SerializeField] GameObject panel;
        [SerializeField] ButtonCustom closeBtn;
        [SerializeField] Text title;
        [SerializeField] Text desc;

        int total_new_energy;

        void Start()
        {
            closeBtn.Init(0, ButtonClicked);
            Events.ShowNewCoinsPopup += ShowNewCoinsPopup;
            Close();
        }
        private void OnDestroy()
        {
            Events.ShowNewCoinsPopup -= ShowNewCoinsPopup;
        }
        void ShowNewCoinsPopup(int qty)
        {
            panel.SetActive(true);
            desc.text = qty.ToString();
        }
        void ButtonClicked(int id)
        {
            Close();
        }
        void Close()
        {
            panel.SetActive(false);
        }
    }
}