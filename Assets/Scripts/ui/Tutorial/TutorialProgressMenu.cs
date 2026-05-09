using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TutorialProgressMenu : ProgressMenu
    {
        [SerializeField] GameObject panel;
        [SerializeField] Text stepsTitle;
        [SerializeField] GameObject openButton;
        [SerializeField] TutorialStepsUIScreen stepsUIScreen;
        [SerializeField] ButtonCustom skipButton;
        [SerializeField] ButtonCustom skipAllButton;

        private void Start()
        {
            SetStepsUIScreen(false);
            panel.SetActive(false);

            Events.OnTutorialProgressMenu += OnTutorialProgressMenu;
            Events.ShowTutorialProgressMenu += ShowTutorialProgressMenu;
            Events.OnSkipTutorialShow += OnSkipTutorialShow;
            Events.ResetApp += ResetApp;
        }
        public void Init()
        {
            skipButton.Init(0, Skip, Data.Instance.texts.Get("skip"));
            //if (Data.Instance.onBoardingManager.IsTutorialDone())
            //    skipAllButton.gameObject.SetActive(false);
            //else
                skipAllButton.Init(1, Skip, Data.Instance.texts.Get("skipAll"));
        }
        void OnDestroy()
        {
            Events.OnTutorialProgressMenu -= OnTutorialProgressMenu;
            Events.ShowTutorialProgressMenu -= ShowTutorialProgressMenu;
            Events.OnSkipTutorialShow -= OnSkipTutorialShow;
            Events.ResetApp -= ResetApp;
        }
        void ResetApp()
        {
            panel.SetActive(false);
        }
        void OnSkipTutorialShow(bool isOn)
        {
            skipButton.gameObject.SetActive(isOn);
            if(Data.Instance.onBoardingManager.IsTutorialDone())
                skipAllButton.gameObject.SetActive(false);
            else
                skipAllButton.gameObject.SetActive(!isOn);
        }
        void ShowTutorialProgressMenu(List<ItemData> all, bool isOn = false)
        {
            if(isOn)
            {
                skipButton.gameObject.SetActive(false);
                skipAllButton.gameObject.SetActive(true);
                panel.SetActive(true);
                Init(all);
                if (Data.Instance.myTeam.GetTutorial() > 0)
                    SetStepsUIScreen(true);
            } else
                panel.SetActive(false);
        }
        public override void SetProgress(int id)
        {
            base.SetProgress(id);
            stepsTitle.text = "STEP " + (id + 1);
        }
        void OnTutorialProgressMenu(int id)
        {
            if (!isOpened)
            {
                SetProgress(id);
                if (id >= Fulbo.Game.Tutorial.TutorialData.Instance.all.Length - 1)
                {
                    skipAllButton.gameObject.SetActive(false);
                    skipButton.gameObject.SetActive(true);
                    openButton.SetActive(false);
                }
                else
                    openButton.SetActive(true);
            }
        }
        public void Skip(int id)
        {
            if (id == 0) //skip one step:
            {
                if (Data.Instance.newScene == "Game")
                {
                    SkipGame();
                    SetStepsUIScreen(false);
                }
                else
                {
                    skipButton.gameObject.SetActive(false);
                    skipAllButton.gameObject.SetActive(true);
                    Events.OnSkipTutorial();
                }
            }
            else//skipall skip all steps:
            {
                if (Data.Instance.newScene == "Game")
                    SkipGame();
                else
                    Events.OnSkipAllTutorialSteps();

                SetStepsUIScreen(false);
            }
        }
        void SkipGame()
        {
            Events.GameOver();
            Data.Instance.LoadLevel("MainMenu");

        }
        public void Open(int id)
        {
            SetStepsUIScreen(false);
            CharactersManagerTutorial cm = (CharactersManagerTutorial)GameManager.Instance.charactersManager;
            cm.Kick();
            cm.tutorialsManager.id = id;
            cm.tutorialPopupUI.DoId(id);
        }
        bool isOpened;
        public void SetStepsUIScreen(bool isOn)
        {
            isOpened = isOn;
            stepsUIScreen.gameObject.SetActive(isOn);
            if(isOn)
            {
                print("RESET tutorial on open");
                Events.OnSkipTutorialShow(false);
                Events.OnTutorialPopup(0, "", null, false, false);
                if (GameManager.Instance.GetComponent< CharactersManagerTutorial>() != null)
                {
                    CharactersManagerTutorial cm = (CharactersManagerTutorial)GameManager.Instance.charactersManager;
                    cm.ResetAll();
                    cm.tutorialsManager.SetControllToMainCharacter();
                    cm.Kick();
                    cm.tutorialsManager.OnDoneClicked();
                    cm.tutorialsManager.OnSetDone(false);
                    Events.OnGameStatusChanged(Fulbo.Game.GameManager.states.GOAL);
                    Events.OntutorialStepDone(false);
                    stepsUIScreen.Init(this);
                    Events.OntutorialStepDone(false);
                }
            }
            openButton.SetActive(!isOn);
        }
    }
}
