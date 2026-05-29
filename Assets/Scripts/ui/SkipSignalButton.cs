using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SkipSignalButton : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] Text field;
        System.Action OnReady;

        void Start()
        {     
            panel.SetActive(false);
    #if UNITY_MOBILE
            Events.OnSkipOn += OnSkipOn;
            Events.OnSkipOff += OnSkipOff;
    #endif
        }
        void OnSkipOff()
        {
            panel.SetActive(false);
        }
        void OnSkipOn(System.Action OnReady, string text = "skip")
        {
            if (Data.Instance.settings.mainSettings.isArcade)
                return;

            print("SKIP " + OnReady + " text: " + text);
            this.OnReady = OnReady;
            panel.SetActive(true);
            field.text = Data.Instance.texts.Get(text);
        }
        public void OnClicked()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/click");
            OnSkipOff();
            OnReady();
        }
    }
}
