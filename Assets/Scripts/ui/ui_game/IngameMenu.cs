using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class IngameMenu : MonoBehaviour
    {
        bool isOn;
        bool isMobile;
        [SerializeField] GameObject[] mobilePanels;
        [SerializeField] GameObject[] arcadePanels;

        [SerializeField] GameObject[] buttons1; // para apagarlos en tutorial
        [SerializeField] GameObject[] buttons2; // para apagarlos en tutorial
        [SerializeField] GameObject[] buttons3; // para apagarlos en tutorial

        void Start()
        {
            isMobile = true;
#if UNITY_STANDALONE || UNITY_WEBGL
            isMobile = false;
#endif
            foreach (GameObject go in arcadePanels)
                go.SetActive(false);
            foreach (GameObject go in mobilePanels)
                go.SetActive(false);

            SetOn(true);

            Events.OnGoal += OnGoal;
            Events.OnGameStatusChanged += OnGameStatusChanged;
        }
        private void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
            Events.OnGameStatusChanged -= OnGameStatusChanged;
        }
        void OnGameStatusChanged(Game.GameManager.states state)
        {
            if (Game.GameManager.Instance.isTutorial) return;
            SetOn(true);
        }
        void OnGoal(int a, Game.Character ch)
        {
            if (Game.GameManager.Instance.isTutorial) return;
            if (!isMobile)
                SetOn(false);
        }
        void SetOn(bool on)
        {
            if (Data.Instance.settings.mainSettings.isArcade)
                return;

            if (isMobile)
                foreach (GameObject go in mobilePanels)
                    go.SetActive(on);
            else if(Data.Instance.mode != Data.modes.PARTYMODE)
                foreach (GameObject go in arcadePanels)
                    go.SetActive(on);
        }



        public void SetButtonsForTutorial(int tutorialID)
        {
            SetButtonsActive(buttons1, false);
            SetButtonsActive(buttons2, false);
            SetButtonsActive(buttons3, false);
            if (tutorialID>0)
            {
                SetButtonsActive(buttons1, true);
            }
            if (tutorialID > 1)
            {
                SetButtonsActive(buttons2, true);
            }
            if (tutorialID > 2)
            {
                SetButtonsActive(buttons3, true);
            }
        }
        void SetButtonsActive(GameObject[] arr, bool active)
        {
            foreach (GameObject go in arr)
                go.SetActive(active);
        }
    }
}