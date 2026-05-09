using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Onboarding
{
    public class OnboardingPanel : MonoBehaviour
    {
        [SerializeField] Text field;
       // [SerializeField] ButtonCustom skip;
        [SerializeField] string contentIDInDB; // string for localization in DB:
        OnBoardingManager manager;
        public bool showBackground = true;
        public enum panels
        {
            storymode,
            levels,
            team,
            marketplace,
            account,
            intro,
            summary,
            cups,
            gameintro,
            myteam,
            shop,
            shards,
            training
        }
        public int id;

        public void Init(OnBoardingManager manager)
        {
            if (!manager.IsOpen) {
                manager.IsOpen = true;
                gameObject.SetActive(true);
                string s = contentIDInDB + id.ToString();
                print("onboarding text_id: " + s);
                field.text = Data.Instance.texts.Get(s);
                this.manager = manager;
            }

            //Dictionary<string, object> param = new Dictionary<string, object>();
            //param["onboardingStep"] = contentIDInDB + id;
            //Events.OnTrack("OnboardingAdvanced", param);
        }
        public void Close()
        {
            if(manager!=null)
                manager.IsOpen = false;
            gameObject.SetActive(false);
        }
        
        void OnSkipClicked(int id)
        {
            manager.Skip();
        }
    }
}
