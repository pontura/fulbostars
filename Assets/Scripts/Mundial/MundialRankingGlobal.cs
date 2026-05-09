using Fulbo.DB;
using Fulbo.Mundial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Mundial
{
    public class MundialRankingGlobal : CascadeList
    {
        [SerializeField] Transform containter;
        [SerializeField] MundialRankingButton button;
        [SerializeField] ButtonCustom playButton;
        [SerializeField] ButtonCustom termsButton;
        [SerializeField] MainMenuButtons mainMenuButtons;
        Animation anim;

        public void Init()
        {
            Utils.RemoveAllChildsIn(containter);
            playButton.Init(0, UIClicked, Data.Instance.texts.Get("gotoplay"));
            termsButton.Init(1, UIClicked, Data.Instance.texts.Get("baseAndCond"));
            GetComponent<MundialRankingLocal>().Init();
            anim = GetComponent<Animation>();
            LoadContent();
            anim.Play("in");
            Invoke("BackDelayed", 0.75f);
        }
        void BackDelayed()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }
        void UIClicked(int id)
        {
            Back();
            switch (id)
            {
                case 0:
                  //  mainMenuButtons.GotoStoryMode();
                    break;
                case 1:
                    Application.OpenURL("https://medium.com/fulbo-galaxy"); 
                    break;
            }
        }
        public void Back()
        {
            anim.Play("out");
            Events.Back();
            Invoke("Reset", 0.5f);
        }
        private void Reset()
        {
            gameObject.SetActive(false);
        }
        void LoadContent()
        {
            MundialData.Instance.LoadRanking(OnReady);
        }
        void OnReady()
        {
            InitCascade();
            Utils.RemoveAllChildsIn(containter);
            int id = 0;
            foreach(DBMundial.ResultsData data in MundialData.Instance.rankings.results)
            {
                MundialRankingButton b = Instantiate(button, containter);
                b.Init(id, OnClicked);
                id++;
                b.OnInit(id, data);
                AddToCascade(b);
            }
            StartCascade();
        }
        void OnClicked(int id)
        {

        }
    }
}
