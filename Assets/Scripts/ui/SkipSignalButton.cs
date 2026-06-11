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
            if(Data.Instance.isMobile){
                Events.OnSkipOn += OnSkipOn;
                Events.OnSkipOff += OnSkipOff;
            }
        }
        void OnSkipOff()
        {
            panel.SetActive(false);
        }
        void OnSkipOn(System.Action OnReady, string text = "skip")
        {
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
