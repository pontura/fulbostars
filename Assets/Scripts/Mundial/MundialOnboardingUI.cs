using Fulbo.DB;
using Fulbo.Mundial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Mundial
{
    public class MundialOnboardingUI : MonoBehaviour
    {
        [SerializeField] GameObject[] screens;
        [SerializeField] ButtonCustom skipBtn;

        [SerializeField] ButtonCustom confirmBtn;
        [SerializeField] ButtonCustom cancelBtn;

        [SerializeField]
        EditTeam.ClubShield[] clubShields;
        MundialData.LevelData levelData;

        [SerializeField] Text field1;
        [SerializeField] Text field1_subtitle;

        [SerializeField] Text field2;
        [SerializeField] Text field3;
        [SerializeField] Text field4;
        [SerializeField] Text field5;

        int id;
        private void Start()
        {
            Data.Instance.ui.SetBackButton(true, Back);
            skipBtn.Init(0, Next, Data.Instance.texts.Get("skipTextMobile"));
            confirmBtn.Init(1, Confirm, Data.Instance.texts.Get("confirm"));
            cancelBtn.Init(-1, Confirm, Data.Instance.texts.Get("cancel"));
            id = 1;
            ShowScreen();

            //Analytics
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("SelectedCountryName", "Opened Mundial Scene");

            Events.OnTrack("MundialAdvance", param);
        }
        void Confirm(int _id)
        {
            id += _id;
            ShowScreen();
        }
        void Back()
        {
           // Data.Instance.LoadLevel("MainMenu");
        }
        void ShowScreen()
        {
            skipBtn.gameObject.SetActive(true);
            Reset();
            screens[id-1].SetActive(true);

            string text;
            switch (id)
            {
                case 1:
                    field1.text = Data.Instance.texts.Get("onboarding_mundialtitulo");
                    field1_subtitle.text = Data.Instance.texts.Get("onboarding_mundial_text1");
                    break;
                case 2:
                    field2.text = Data.Instance.texts.Get("onboarding_mundial_text2");
                    break;
                case 3:
                    field3.text = Data.Instance.texts.Get("onboarding_mundial_text3");
                    skipBtn.gameObject.SetActive(false);
                    break;
                case 4:
                    field4.text = Data.Instance.texts.Get("onboarding_confirmation");
                    skipBtn.gameObject.SetActive(false);
                    break;
                case 5:
                    field5.text = Data.Instance.texts.Get("onboarding_mundial_text4");

                    //Analytics
                    string teamName = MundialData.Instance.GetCountryData(DB.DBManager.Instance.DbUserData.data.country).name;
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("SelectedCountryName", teamName);
                    Events.OnTrack("MundialAdvance", param);

                    break;
            }
        }
        void Next(int _id)
        {
            id++;
            if (id >= screens.Length)
            {
                MundialData.Instance.openShortCut = true;
              //  Data.Instance.LoadLevel("MainMenu");
            }
            else
                ShowScreen();
        }
        public void OnClubSelected(MundialData.LevelData levelData)
        {
            this.levelData = levelData;
            Next(1);
            foreach (EditTeam.ClubShield c in clubShields)
                c.Init(levelData.clubData);
        }
        public void BackToNames()
        {
            id--;
            ShowScreen();
        }
        private void Reset()
        {
            foreach (GameObject go in screens)
                go.SetActive(false);
        }
    }
}
