using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Fulbo.UI
{
    public class BackButton : MonoBehaviour
    {
        types type;
        [SerializeField] Image image;
        [SerializeField] Sprite[] assets;

        public enum types
        {
            BACK,
            HOME
        }
        void Start()
        {
            Events.OnBackActive += OnBackActive;
            Events.EndGame += EndGame;
        }
        void OnDestroy()
        {            
            Events.OnBackActive -= OnBackActive;
            Events.EndGame -= EndGame;
        }

        private void EndGame()
        {
            if(!Data.Instance.settings.mainSettings.isArcade && !gameObject.activeSelf)
                return;
            Clicked();  
        }

        void OnBackActive(bool isOn)
        {
            SetActive(isOn);
        }
        public void Clicked()
        {
            Data.Instance.Back();
        }
        public void SetActive(bool isOn)
        {
            gameObject.SetActive(isOn);
        }
        public void SetType(types t)
        {
            type = t;
            switch(t )
            {
                case types.BACK:
                    image.sprite = assets[0]; break;
                case types.HOME:
                    image.sprite = assets[1]; break;
            }
        }
    }
}
