using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class FieldSelector : MonoBehaviour
    {
        public GameObject panel;
        bool isOn;

        private void Start()
        {
            Reset();
            Events.OnButtonClick += OnButtonClick;
        }
        private void OnDestroy()
        {
            Events.OnButtonClick -= OnButtonClick;
        }
        void OnButtonClick(int a, int b)
        {
            if (isOn)
                OnSelect(0);
        }
        private void Reset()
        {
            panel.SetActive(false);
            isOn = false;
        }
        public void Init()
        {
            Invoke("Delayed", 0.5f);
            panel.SetActive(true);
        }
        void Delayed()
        {
            isOn = true;
        }
        public void OnSelect(int id)
        {
            isOn = false;
            Data.Instance.settings.mainSettings.stadium_id = id;
            // StadiumsData.Instance.SetActiveStadium(id);
            Data.Instance.LoadLevel("PlayersTeamSelector");

        }
    }
}