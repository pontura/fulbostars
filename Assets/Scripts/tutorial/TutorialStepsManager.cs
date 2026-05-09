using UnityEngine;
using System.Collections;
using Fulbo.UI;
using Fulbo.Stadiums;
using System.Collections.Generic;
using System;

namespace Fulbo.Game.Tutorial
{
    public class TutorialStepsManager : MonoBehaviour
    {

        public Vector2 limits_x;
        public Vector2 limits_y;

        public int id;
        TutorialStep tutorialStep;
        [SerializeField] states state;
        [SerializeField] Transform items;

        CharactersManagerTutorial charactersManagerTutorial;

        public int GetTotalSteps()
        {
            return TutorialData.Instance.all.Length - 2;
        }
        public enum states
        {
            WAITING,
            PLAYING,
            DONE
        }
        private void Awake()
        {
            AudioManager.Instance.FadeVolume("music", 0.2f);


            StadiumsData.Instance.SetActiveStadium(-1, "medium");
            Data.Instance.matchData.AddRealTeam(3);
            List<int> opponents = new List<int>();
            opponents.Add(2);
            opponents.Add(43);
            opponents.Add(83);
            Data.Instance.matchData.AddOponentTeam(opponents);

            Data.Instance.matchData.totalCharacters_team1 = 3;
            Data.Instance.matchData.totalCharacters_team2 = 5;
            //StadiumsData.Instance.SetActiveStadium(0, "medium");

            float _limits_x = StadiumsData.Instance.active.GetAssetBySelectedSize().size_x / 2;
            float _limits_y = StadiumsData.Instance.active.GetAssetBySelectedSize().size_y / 2;

            limits_y = new Vector2(_limits_y, -_limits_y);
            limits_x = new Vector2(-_limits_x, _limits_x);

            id = 0;

            Events.OnSkipTutorial += OnSkipTutorial;
            Events.OnSkipAllTutorialSteps += OnSkipAllTutorialSteps;
        }
        private void Start()
        {
            List<ProgressMenu.ItemData> itemsData = new List<ProgressMenu.ItemData>();
            int this_id = 0;
            foreach(TutorialData.StepData sData in TutorialData.Instance.all)
            {
                ProgressMenu.ItemData iData = new ProgressMenu.ItemData();
                if (id == this_id) iData.state = ProgressMenu.ItemData.states.ON;
                else iData.state = ProgressMenu.ItemData.states.INACTIVE;
                itemsData.Add(iData);
                this_id++;
            }
            Events.ShowTutorialProgressMenu(itemsData, true);
        }
        private void OnDestroy()
        {
            Events.OnSkipTutorial -= OnSkipTutorial;
            Events.OnSkipAllTutorialSteps -= OnSkipAllTutorialSteps;
        }
        void OnSkipAllTutorialSteps()
        {
            //charactersManagerTutorial.tutorialPopupUI.GotoMatch();
            Data.Instance.LoadLevel("MainMenu");
            //id = 7;
            //Win();
        }
        void OnSkipTutorial()
        {
            OnSetDone(false);
        }
        private void Update()
        {
            if (state == states.PLAYING)
                UpdatePlaying();
        }
        System.Action OnInitDone;
        public void Init(System.Action OnInitDone)
        {
            AudioManager.Instance.FadeVolume("music", 0, 0.5f);
            AudioManager.Instance.ChangeVolume("ambience", 0f);
            AudioManager.Instance.PlaySound("ambience", "_new/ambience/crowd_loop_1", true);
            AudioManager.Instance.FadeVolume("ambience", 0.1f, 0.5f);
            //id = Data.Instance.myTeam.GetTutorial();
            charactersManagerTutorial = Fulbo.Game.GameManager.Instance.GetComponent<CharactersManagerTutorial>();
            Utils.RemoveAllChildsIn(items);
            state = states.WAITING;
            Character otherCharacter;
            Vector3 pos = Vector3.zero;
            GameObject asset;
            switch (id)
            {
                case 0:
                    tutorialStep = new Tutorial_Run();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                case 1:
                    tutorialStep = new Tutorial_RunVS();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                case 2:
                    tutorialStep = new Tutorial_Pass();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                case 3:
                    tutorialStep = new Tutorial_Goal();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                case 4:
                    tutorialStep = new Tutorial_Goal();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    (tutorialStep as Tutorial_Goal).AddGoalKeeper();
                    break;
                case 5:
                    tutorialStep = new Tutorial_StoleBall();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                case 6:
                    tutorialStep = new Tutorial_Lujito();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                case 7:
                    tutorialStep = new Tutorial_Center();
                    tutorialStep.Init(this);
                    asset = Instantiate(TutorialData.Instance.GetStepData(id).asset_to_add, items);
                    tutorialStep.Setup(charactersManagerTutorial, asset);
                    break;
                default: tutorialStep = new Tutorial_Run(); break;
            }

            this.OnInitDone = OnInitDone;
            Events.OnTutorialPopup(id, Data.Instance.texts.Get("tutorial_" + id), OnSetInit, false, false);

            Events.OnTutorialProgressMenu(id);
        }
        public void OnSetInit()
        {
            Events.OnSkipTutorialShow(true);
            state = states.PLAYING;
            OnInitDone();
            tutorialStep.PassBallToCharacter();
        }
        public void OnLose()
        {
            OnSetDone(false);
        }
        public void OnSetDone(bool win, int falseID = 0)
        {
            if (tutorialStep != null)
                tutorialStep.OnReset();
            if (state != states.PLAYING) return;
            AudioManager.Instance.FadeVolume("music", 1, 1);

            Events.OnSkipTutorialShow(false);
            SetControllToMainCharacter();
            //AudioManager.Instance.PlaySound("crowd", "_new/ambience/crowd_gol", false);
            AudioManager.Instance.PlayCrowd(Fulbo.Game.GameManager.Instance.stadiumData.active.crowd_gol);
            charactersManagerTutorial.Kick();
            Events.OnGameStatusChanged(Fulbo.Game.GameManager.states.GOAL);
            Events.OntutorialStepDone(win);
            if (win) Win(); else Lose(falseID);
        }
        public void SetControllToMainCharacter()
        {
            if (charactersManagerTutorial == null) return;
            Character character = charactersManagerTutorial.character;
            if (!character.isBeingControlled)
            {
                foreach (Character ch in charactersManagerTutorial.team2)
                {
                    if (ch.isBeingControlled)
                    {
                        charactersManagerTutorial.SwapTo(ch, character);
                        return;
                    }
                }
            }
        }
        void Win()
        {
            Events.OnVoiceSay("trainingok", null);
            charactersManagerTutorial.Happy();
            Events.OnTutorialPopup(id, Data.Instance.texts.Get("tutorial_" + id + "_done"), OnDoneClicked, true, true);
        }
        void Lose(int falseID = 0)
        {
            charactersManagerTutorial.Cry();
            string tip = "tutorial_" + id + "_lose";
            if (falseID > 0) tip = "tutorial_" + id + "_lose" + falseID;
            Events.OnTutorialPopup(id, Data.Instance.texts.Get(tip), OnDoneClicked, true, false);
        }
        public void OnDoneClicked()
        {
            state = states.DONE;
        }
        void UpdatePlaying()
        {
            tutorialStep.OnUpdate();
        }
        public Vector3 GetOriginalPos()
        {
            return tutorialStep.GetOriginalPos();
        }
        public void Next()
        {
            id++;
            Data.Instance.myTeam.OnTutorialStepDone(id);
        }
        private void OnDisable()
        {
            if(tutorialStep != null)
                tutorialStep.OnReset();
        }
    }
}