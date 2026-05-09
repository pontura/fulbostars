using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Fulbo.Game;

namespace Fulbo.Input
{
    public class InputManagerGame : MonoBehaviour
    {
        public float _x;
        public float _y;
        public VariableJoystick variableJoystick;

        int totalPlayersAvailable;
        public CharactersManager charactersManager;
        float input_x_sensibilitty;
        float input_y_sensibilitty;
        GameManager game;
        float delayForLujito;

        public List<PlayerInput> playerInputs;
        [Serializable]
        public class PlayerInput
        {
            public int lastDirection;
            public float lastDirectionTime;
            public int totaldirectionChanges;
            public int lastButtonIDPressed;
        }

        public void OnButtonPressed(int buttonID, int PlayerID)
        {
            GetButtonDown(buttonID, PlayerID);
        }
        public void OnButtonReleased(int buttonID, int PlayerID)
        {
            GetButtonUp(buttonID, PlayerID);
        }


        void Start()
        {
            delayForLujito = Data.Instance.settings.GetSetting("delayForLujito");
            game = GameManager.Instance;
            totalPlayersAvailable = Data.Instance.settings.totalPlayersAvailable;

            for (int a = 0; a < totalPlayersAvailable; a++)
                playerInputs.Add(new PlayerInput());

            input_x_sensibilitty = Data.Instance.settings.GetSetting("input_x_sensibilitty");
            input_y_sensibilitty = Data.Instance.settings.GetSetting("input_y_sensibilitty");

            Events.OnRestartGame += OnRestartGame;
            InputManager.Instance.OnButtonPressed += OnButtonPressed;
            InputManager.Instance.OnButtonReleased += OnButtonReleased;
        }
        public void OnDestroyed()
        {
            Events.OnRestartGame -= OnRestartGame;

            InputManager.Instance.OnButtonPressed -= OnButtonPressed;
            InputManager.Instance.OnButtonReleased -= OnButtonReleased;
        }
        void OnRestartGame()
        {

        }
        public void Update()
        {
            if (totalPlayersAvailable <= 0) return;
            if (game.state != GameManager.states.PLAYING)
            {
                _x = 0; _y = 0;
                return;
            }


            PlayerInput playerInput;
#if UNITY_STANDALONE || UNITY_WEBGL
            int playerID = 1;
            foreach (int plays in Data.Instance.matchData.players)
            {
                playerInput = GetInputByPlayer(playerID-1);
                if (plays > 0)
                {
                    Vector2 movement = InputManager.Instance.GetMovement(playerID);
                    _x = movement.x;
                    _y = movement.y;

                    if (charactersManager != null)
                    {

                        float offset = 0.6f;
                        if (_x < -offset) _x = -1; else if (_x > offset) _x = 1; else if (_x < 0.1f && _x > -0.1f) _x = 0;
                        if (_y < -offset) _y = -1; else if (_y > offset) _y = 1; else if (_y < 0.1f && _y > -0.1f) _y = 0;
                        charactersManager.SetPosition(playerID, _x, _y);
                    }
                    if (playerInput.lastButtonIDPressed == 3 && timerButton3Down + delayForLujito < Time.time)
                    {
                        //if (game.powerupsManager != null && game.powerupsManager.IsCharging()) return;
                        playerInput.lastButtonIDPressed = 0;
                        charactersManager.Lujito(playerID);
                    }
                }
                playerID++;
            }
#else
            playerInput = GetInputByPlayer(0);
            if (playerInput.lastButtonIDPressed == 3 && timerButton3Down + delayForLujito < Time.time)
            {
                //if (game.powerupsManager != null && game.powerupsManager.IsCharging()) return;
                playerInput.lastButtonIDPressed = 0;
                charactersManager.Lujito(1);
                return;
            }
            MobileVersion();
#endif
        }
        void MobileVersion()
        {
            _x = variableJoystick.Direction.x * input_x_sensibilitty;
            _y = variableJoystick.Direction.y * input_y_sensibilitty;

            float offset = 1;
            if (_x > offset) _x = 1; else if (_x < -offset) _x = -1;
            if (_y > offset) _y = 1; else if (_y < -offset) _y = -1;

            if (charactersManager != null)
                charactersManager.SetPosition(1, _x, _y);
        }
        public void OnMobileButtonPressed(int id)
        {
            AudioManager.Instance.PlaySound("common2", "ui/ui_tap", false);
            GetButtonDown(id, 1);
        }
        public void OnMobileButtonUp(int id)
        {
           // mobile_last_button_pressed = 0;
            GetButtonUp(id, 1);
        }
        [SerializeField] bool canPressButton1 = true;
        [SerializeField] bool canPressButton2 = true;
        [SerializeField] bool canPressButton3 = true;
        [SerializeField] bool canPressButton4 = true;

        public void SetTutorial(int id)
        {
            canPressButton1 = canPressButton2 = canPressButton3 = false;
            if (id > 0) canPressButton3 = true;
            if (id > 1) canPressButton2 = true;
            if (id > 2) canPressButton1 = true;
        }

        public void SetButtonEnabled(int id) {
            canPressButton1 = canPressButton2 = canPressButton3 = false;
            if (id == 0) canPressButton3 = true;
            else if (id == 1) canPressButton2 = true;
            else if (id == 2) canPressButton1 = true;
        }

        float timerButton3Down;
        void GetButtonDown(int buttonID, int playerID)
        {
            if (game.isTutorial || game.isOnboardingGame)
            {
                if (buttonID == 1 && !canPressButton1) return;
                else if (buttonID == 2 && !canPressButton2) return;
                else if (buttonID == 3 && !canPressButton3) return;
                else if (buttonID == 4 && !canPressButton4) return;
            }

            PlayerInput playerInput = GetInputByPlayer(playerID - 1);
            int lastButtonPressedID = playerInput.lastButtonIDPressed;

            //  playerID = 2; // tODO: fuerza player izqueirdo:
            if (buttonID == 3)
                timerButton3Down = Time.time;
            if ((lastButtonPressedID == 2 && buttonID == 1) || (lastButtonPressedID == 1 && buttonID == 2))
            {
                playerInput.lastButtonIDPressed = 0;
                charactersManager.Lujito(playerID);
            }
            else if (charactersManager != null)
                charactersManager.ButtonPressed(buttonID, playerID);
            else
                Events.OnButtonClick(buttonID, playerID);

            playerInput.lastButtonIDPressed = buttonID;
        }
        void GetButtonUp(int buttonID, int playerID)
        {           

            PlayerInput playerInput = GetInputByPlayer(playerID - 1);

            if (playerInput.lastButtonIDPressed == 0) return;

            playerInput.lastButtonIDPressed = 0;

            if (charactersManager != null)
                charactersManager.ButtonUp(buttonID, playerID);
            else
                Events.OnButtonClick(buttonID, playerID);
        }
        PlayerInput GetInputByPlayer(int playerID)
        {
            //return playerInputs[0];
            return playerInputs[playerID];
        }
    }
}