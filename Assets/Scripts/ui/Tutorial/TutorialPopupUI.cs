using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TutorialPopupUI : MonoBehaviour
    {
        System.Action OnDone;
        [SerializeField] Image picturesBoard;
        [SerializeField] Image backgroundFillImage;
        [SerializeField] GameObject background;
        [SerializeField] Text field;
        [SerializeField] GameObject panel;
        [SerializeField] GameObject buttons;
        [SerializeField] GameObject retry_buttons;
        [SerializeField] GameObject nextButton;
        [SerializeField] GameObject progressBar;
        [SerializeField] Image progressBarImage;
        [SerializeField] Text progressBarField;
        [SerializeField] IngameMenu ingameMenu;
        CharactersManagerTutorial charactersManagerTutorial;
        [SerializeField] Animator referi;
        public int id = 0;
        [SerializeField] Text kickSlideField;
        [SerializeField] Text passSlideField;
        [SerializeField] Text kickSlideFieldMobile;
        [SerializeField] Text passSlideFieldMobile;

        [SerializeField] GameObject[] tutorialButtonsActive;
        [SerializeField] GameObject[] tutorialButtonsActiveMobile;

        [SerializeField] GameObject tips_mobile;
        [SerializeField] GameObject tips;
        bool isMobile;

        void Start()
        {
            panel.SetActive(true);
            Events.OnVoiceSay("trainingintro", null);
            if (Data.Instance.myTeam.GetGamesPlayed()>0)
            {
                Data.Instance.ui.SetBackButton(true, Back);
            }
#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#endif
            if (isMobile)
            {
                tips_mobile.SetActive(true);
                tips.SetActive(false);
            } else
            {
                tips_mobile.SetActive(false);
                tips.SetActive(true);
            }
            progressBar.SetActive(false);
            charactersManagerTutorial = Fulbo.Game.GameManager.Instance.GetComponent<CharactersManagerTutorial>();
            
            animName = "idle";
            referi.Play(animName);

            Events.OnTutorialPopup += OnTutorialPopup;
            Events.OnTutorialProgress += OnTutorialProgress;
            Events.OnTutorialProgressAdd += OnTutorialProgressAdd;
            if (isMobile)
            {
                foreach (GameObject tutorialButton in tutorialButtonsActiveMobile)
                    tutorialButton.SetActive(false);
            }
            else
            {
                foreach (GameObject tutorialButton in tutorialButtonsActive)
                    tutorialButton.SetActive(false);
            }
            SetImage();
        }
        void Back()
        {
            print("back");
            Data.Instance.LoadLevel("MainMenu");
        }
        void SetImage()
        {
            picturesBoard.sprite = Fulbo.Game.Tutorial.TutorialData.Instance.GetStepData(id).image;
        }
        void OnDestroy()
        {
            Events.OnTutorialPopup -= OnTutorialPopup;
            Events.OnTutorialProgress -= OnTutorialProgress;
            Events.OnTutorialProgressAdd -= OnTutorialProgressAdd;
        }
        string animName = "idle";
        void OnTutorialPopup(int id, string text, System.Action OnDone, bool showButtons, bool win)
        {
            if(id>this.id)
            {
                backgroundFillImage.enabled = true;
                background.SetActive(true);
            }
            else
            {
                backgroundFillImage.enabled = false;
                background.SetActive(false);
            }
            this.id = id;
            SetImage();
            buttons.SetActive(false);
            retry_buttons.SetActive(false);
            ingameMenu.SetButtonsForTutorial(id);
            if (!showButtons)
            {
                animName = "idle";
                Show();
            }
            else
            {
                if (win)
                    animName = "win";
                else
                    animName = "lose";

                buttons.SetActive(true);
                Invoke("Show", 1);
            }

            nextButton.SetActive(true);

            this.OnDone = OnDone;
            field.text = text;
            SetTutorialSignalButtons(id);
        }
        void SetTutorialSignalButtons(int id)
        {
            GameObject go;
            SetTipActive(0);
            if (id > 0)
            {
                SetTipActive(1);
            }
            if (id > 1)
            {
                if (isMobile)
                    passSlideFieldMobile.text = Data.Instance.texts.Get("button_pass");
                else
                    passSlideField.text = Data.Instance.texts.Get("button_pass");
                SetTipActive(2);
            }
            if (id > 2)
            {
                SetTipActive(3);
            }
            if (id == 4)
            {
                if (isMobile)
                    kickSlideFieldMobile.text = Data.Instance.texts.Get("button_shoot");
                else
                    kickSlideField.text = Data.Instance.texts.Get("button_shoot_3"); 
            }   
            else if (id == 5)
            {
                if (isMobile)
                    kickSlideFieldMobile.text = Data.Instance.texts.Get("button_shoot_2");
                else
                    kickSlideField.text = Data.Instance.texts.Get("button_shoot_2");
            }
            else if (id == 6)
            {
                SetTipActive(4);
                if (isMobile)
                    passSlideFieldMobile.text = Data.Instance.texts.Get("button_jump");
                else
                    passSlideField.text = Data.Instance.texts.Get("button_jump");
            }
            else if (id == 7)
            {
                SetTipActive(2);
                if (isMobile)
                    passSlideFieldMobile.text = Data.Instance.texts.Get("button_center"); 
                else
                    passSlideField.text = Data.Instance.texts.Get("button_center");
            }
            else
                kickSlideField.text = Data.Instance.texts.Get("button_shoot");

        }
        void SetTipActive(int id)
        {
            GameObject go;
            if (isMobile)
            {
                foreach (GameObject tutorialButton in tutorialButtonsActiveMobile)
                    tutorialButton.SetActive(false);
                go = tutorialButtonsActiveMobile[id];
                go.SetActive(true);
                go.GetComponent<Animation>().Play();
            }
            else
            {
                foreach (GameObject tutorialButton in tutorialButtonsActive)
                    tutorialButton.SetActive(false);
                go = tutorialButtonsActive[id];
                go.SetActive(true);
                go.GetComponent<Animation>().Play("buttonActive");
            }
        }
        bool showed;
        void Show()
        {
            panel.SetActive(true);
            if (!showed)
            {
                AnimPanel("popup_init");
                showed = true;
            } else
                AnimPanel("popup_on");
            referi.Play(animName);
        }
        public void DoId(int id)
        {
            Events.OnGameStatusChanged(Fulbo.Game.GameManager.states.GOAL);
            this.id = id;
            Restart();
        }
        public void OnDoIt()
        {
            //if (id >= Fulbo.Game.Tutorial.TutorialData.Instance.all.Length-1)
            //    GotoMatch();
            //else
            //{
                AudioManager.Instance.PlaySoundOneShot("ui", "ui/click");
                Events.OnVoiceSay("trainingstep" + ((int)id + 1), null);
                OnDone();
                Close();
          //  }
        }
        public void Next()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/click");
            if (charactersManagerTutorial.CheckNext())
            {
                Restart();
            }
            else
            {
                Back();
               // GotoMatch();
            }
        }
        //public void GotoMatch()
        //{
        //    Data.Instance.myTeam.OnTutorialStepDone(Fulbo.Game.Tutorial.TutorialData.Instance.all.Length - 1);
        //    Fulbo.Stadiums.StadiumsData.Instance.SetActiveStadium(-1, "small");
        //    //Data.Instance.matchData.SetActualStadium(-1);
        //    //Data.Instance.matchData.SetActualLevel(1);

        //    // hardcode levels:
        //    int cupID = 10;
        //    int levelID = 10;
        //    int tier = 1;
        //    LevelData ld = CupsData.Instance.GetLevelData(cupID, tier, levelID);
        //    Data.Instance.matchData.InitLevel(ld);

        //    Data.Instance.matchData.Reset();
        //    Data.Instance.matchData.AddOponentTutorialTeam();
        //    Data.Instance.charactersPositions.LoadPositions();
        //    Events.OnPopup(Data.Instance.texts.Get("firstGame"), GoToTutorialGame);
        //}
        //void GoToTutorialGame()
        //{
        //    Events.OnTutorialProgressMenu(Fulbo.Game.Tutorial.TutorialData.Instance.all.Length-1);
        //    Data.Instance.LoadLevel("Game");
        //    AudioManager.Instance.FadeVolume("music", 0.2f, 1f);
        //}
        public void Retry()
        {            
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/click");
            Restart();
            animName = "idle";
            referi.Play(animName);
            OnDoIt();
        }
        void Restart()
        {
            StartCoroutine(Fulbo.Game.GameManager.Instance.OnWaitToStartTutorial());
        }
        void Close()
        {
            animName = "idle";
            referi.Play(animName);
            AnimPanel("popup_off");
            //Invoke("CloseDelayed", 0.5f);
        }
        //void CloseDelayed()
        //{
        //    panel.SetActive(false);
        //}

        //Progress
        int totalBarValue;
        int progressValue;
        System.Action OnReady;
        void OnTutorialProgress(int totalBarValue, System.Action OnReady)
        {
            if (totalBarValue == 0)
            {
                progressBar.SetActive(false);
                OnReady = null;
            }
            else
            {
                this.OnReady = OnReady;
                progressValue = 0;
                this.totalBarValue = totalBarValue;
                progressBar.SetActive(true);
                SetProgress();
            }
        }
        void OnTutorialProgressAdd(int value)
        {
            progressValue += value;
            if (progressValue < 0)
                progressValue = 0;

            if (progressValue >= totalBarValue)
            {
                progressValue = totalBarValue;
                if(OnReady != null)
                    OnReady();
                OnReady = null;
            }
            SetProgress();
        }
        void SetProgress()
        {
            progressBarField.text = progressValue + "/" + totalBarValue;
            progressBarImage.fillAmount = (float)progressValue / (float)totalBarValue;
        }
        string lastAction;
        void AnimPanel(string animName)
        {
            if (animName == lastAction) return;

            lastAction = animName;
            panel.GetComponent<Animation>().Play(animName);
        }
    }
}
