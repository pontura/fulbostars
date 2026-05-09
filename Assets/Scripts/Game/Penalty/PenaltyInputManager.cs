using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Fulbo.Game;

namespace Fulbo.Input
    {
        public class PenaltyInputManager : InputManager
        {
            public float _x;
            public float _y;
            public VariableJoystick variableJoystick;

            int totalPlayersAvailable;
            float time_to_palancazo = 0.25f;
            float input_x_sensibilitty;
            float input_y_sensibilitty;

            //public override void OnButton1Pressed_p1(InputAction.CallbackContext context)
            //{
            //    GetButtonDown(1, 2);
            //}
            //public override void OnButton1Released_p1(InputAction.CallbackContext context)
            //{
            //    GetButtonUp(1, 2);
            //}
            //public override void OnButton2Pressed_p1(InputAction.CallbackContext context)
            //{
            //    GetButtonDown(2, 2);
            //}
            //public override void OnButton2Released_p1(InputAction.CallbackContext context)
            //{
            //    GetButtonUp(2, 2);
            //}
            //public override void OnButton3Pressed_p1(InputAction.CallbackContext context)
            //{
            //    GetButtonDown(3, 2);
            //}
            //public override void OnButton3Released_p1(InputAction.CallbackContext context)
            //{
            //    GetButtonUp(3, 2);
            //}
            //public override void OnButton4Pressed_p1(InputAction.CallbackContext context)
            //{
            //    // GetButtonDown(4, 2);
            //    Data.Instance.LoadLevel("Penalty");
            //    //ResetPlayer();
            //}           
           
            public void Init()
            {
                input_x_sensibilitty = Data.Instance.settings.GetSetting("input_x_sensibilitty");
                input_y_sensibilitty = Data.Instance.settings.GetSetting("input_y_sensibilitty");
            }
          
            void MobileVersion()
            {
                _x = variableJoystick.Direction.x * input_x_sensibilitty;
                _y = variableJoystick.Direction.y * input_y_sensibilitty;

                float offset = 1;
                if (_x > offset) _x = 1; else if (_x < -offset) _x = -1;
                if (_y > offset) _y = 1; else if (_y < -offset) _y = -1;
            }          
            void GetButtonDown(int buttonID, int playerID)
            {
            }
            void GetButtonUp(int buttonID, int playerID)
            {
                Events.OnButtonClick(buttonID, playerID);
            }
          
        }
    }