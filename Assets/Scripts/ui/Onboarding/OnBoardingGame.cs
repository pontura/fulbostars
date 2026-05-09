using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Onboarding
{

    public class OnBoardingGame : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] VariableJoystick variableJoystick;
        [SerializeField] GameObject joystick;
        [SerializeField] GameObject[] buttons;
        [SerializeField] GameObject[] buttonsSignals;
        [SerializeField] GameObject dialogue;
        [SerializeField] Text field;
        bool kickDone;
        bool opponentHasBall;
        bool stoleOpponentBall;
        public int step;
        float timeScale = 1;

        //UnityEngine.InputSystem.InputAction userInputAction;

        void Start()
        {
#if UNITY_STANDALONE
            anim.gameObject.SetActive(false);
            Destroy(this);
            return;
#endif
            if (Data.Instance.matchData.ShowQuickIngameTutorial())
            {
                Events.CharacterCatchBall += CharacterCatchBall;
                //Events.OnBallKicked += OnBallKicked;
                GameManager.Instance.isOnboardingGame = true;
                HideDialogue();
                Invoke("Init", 0.1f);
            }
            else
            {
                anim.gameObject.SetActive(false);
                Destroy(this);
            }
        }
        void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;

            //Events.OnBallKicked -= OnBallKicked;
            /*if(userInputAction!=null)
                userInputAction.started -= OnTutorialStepDone;*/
        }
        int buttonClickedId = -1;
        public void ButtonClicked(int id) // lo llama directo el boton de la ui
        {
            if (id == buttonClickedId)
                ResetActionDialog();
            //HideDialogue();
        }        
        void Init()
        {
            joystick.SetActive(false);
            foreach (GameObject button in buttons)
                button.SetActive(false);
            ResetSignals();
        }
        void ResetSignals()
        {
            foreach (GameObject go in buttonsSignals)
                go.SetActive(false);
        }
        void NextStep()
        {
            HideDialogue();
            ResetSignals();
            StopAllCoroutines();
            switch (step)
            {
                case 0:
                    Events.OnTrack("TutorialPass", null);
                    joystick.SetActive(true);
                    OnStepReady = () => SetCursor(1);
                    SetDialogue(0, 1);
                    buttonClickedId = 2;
                    GameManager.Instance.inputManagerGame.SetButtonEnabled(1);
                    ResetActionListener();
                    //userInputAction = Data.Instance.fulboInputs.Player1.Pass;
                    //userInputAction.started += OnTutorialStepDone;
                    break;
                case 1:
                    Events.OnTrack("TutorialSprint", null);
                    OnStepReady = ()=>SetCursor(0);
                    SetDialogue(1, 0.3f);
                    buttonClickedId = 3;
                    GameManager.Instance.inputManagerGame.SetButtonEnabled(0);
                    ResetActionListener();
                    //userInputAction = Data.Instance.fulboInputs.Player1.Run;
                    //userInputAction.started += OnTutorialStepDone;
                    break;
            }
            step++;
        }

        System.Action OnStepReady;
        bool stepDoneReady;
        void WaitToRead() {
            Debug.Log("WaitToRead");
            stepDoneReady = true;
            if (OnStepReady != null) {
                OnStepReady();
                OnStepReady = null;
            }
        }

        void OnTutorialStepDone(UnityEngine.InputSystem.InputAction.CallbackContext context) {
            if (stepDoneReady) {
                Debug.Log("OnTutorialStepDone");
                ResetActionDialog();
            }            
            //Invoke("HideDialogue", 1f);
        }

        void ResetActionDialog() {
            ResetActionListener();
            stepDoneReady = false;
            GameManager.Instance.inputManagerGame.SetTutorial(3);
            HideDialogue();
        }

        void ResetActionListener() {
            /*if (userInputAction != null) {
                userInputAction.started -= OnTutorialStepDone;
                userInputAction = null;
            }*/
        }
        
        void CharacterCatchBall(Character character)
        {
            if (step == 0 || step == 1)
                NextStep();
            else if(step>1 && !opponentHasBall && character.type != Character.types.GOALKEEPER && character.teamID == 1)
            {
                opponentHasBall = true;

                OnStepReady = () => SetCursor(3);
                SetDialogue(3, 0.1f);
                ResetActionListener();
                buttonClickedId = 1;
                //userInputAction = Data.Instance.fulboInputs.Player1.Kick;
                //userInputAction.started += OnTutorialStepDone;
            }
        }
        void SetDialogue(int _step, float delayForPopup, float delayForControls=0.5f)
        {
            StopAllCoroutines();
            anim.Play("on");
            CancelInvoke();
            dialogue.SetActive(true);
            string text = Data.Instance.texts.Get("ingame_onboarding_" + _step);

            if(_step == 2)
                Events.OnTrack("TutorialShot", null);

            field.text = text;
            StartCoroutine(StopGame(delayForPopup, delayForControls));
        }
        IEnumerator StopGame( float delay, float delayForControls)
        {
            yield return new WaitForSeconds(delay);
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(delayForControls);
            WaitToRead();

            //retoma automáticamente el timescale:
            yield return new WaitForSecondsRealtime(10f);
            ResetActionDialog();
            //if (Time.timeScale == 0) HideDialogue();
        }
        void HideDialogue()
        {
            Debug.Log("HideDialogue");
            Time.timeScale = timeScale;
            dialogue.SetActive(false);            
            ResetSignals();
        }
        void SetCursor(int id)
        {
            buttons[id].gameObject.SetActive(true);
            foreach (GameObject go in buttonsSignals)
                go.SetActive(false);
            buttonsSignals[id].SetActive(true);
        }
        private void Update()
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE) return;
           /* if (step == 2)
            {
                float _x = variableJoystick.Direction.x;
                float _y = variableJoystick.Direction.y;
                if (_x > 0 || _y > 0)
                    NextStep();
            } else */if(!kickDone && step > 1)
            {
                float ball_x = GameManager.Instance.ball.transform.position.x;
                if(ball_x>8)
                {
                    kickDone = true;

                    OnStepReady = () => SetCursor(2);
                    SetDialogue(2, 0.1f);
                    buttons[0].gameObject.SetActive(true);
                    GameManager.Instance.inputManagerGame.SetButtonEnabled(2);
                    ResetActionListener();
                    buttonClickedId = 1;
                    //userInputAction = Data.Instance.fulboInputs.Player1.Kick;
                    //userInputAction.started += OnTutorialStepDone;
                    
                }
            }
        }
        /*void OnBallKicked(CharacterStates.kickTypes type, float a, Character ch)
        {
            if (type == CharacterStates.kickTypes.HARD)
                HideDialogue();
        }*/
    }
}
