using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Fulbo.Input.InputManagerGame;

namespace Fulbo.Input
{
    public class InputManager : MonoBehaviour
    {
        static InputManager mInstance = null;
        [SerializeField] bool onlyKeyboard;
        public static InputManager Instance {  get  {   return mInstance;  } }
        void Awake() { 
            mInstance = this; 
            DontDestroyOnLoad(this.gameObject); 
        }

        [SerializeField] List<UnityEngine.InputSystem.PlayerInput> inputs;

        public System.Action<int, int> OnButtonPressed;
        public System.Action<int, int> OnButtonReleased;

        private void Start()
        {
            //inputs[0].SwitchCurrentControlScheme("Gamepad", Keyboard.current);
            //inputs[1].SwitchCurrentControlScheme("Gamepad", Keyboard.current);
            //inputs[2].SwitchCurrentControlScheme("Gamepad", Keyboard.current);
            //inputs[3].SwitchCurrentControlScheme("Gamepad", Keyboard.current);

            if (onlyKeyboard)
            {
                inputs[0].SwitchCurrentControlScheme("Keyboard_1", Keyboard.current);
                inputs[1].SwitchCurrentControlScheme("Keyboard_2", Keyboard.current);
                inputs[2].SwitchCurrentControlScheme("Keyboard_3", Keyboard.current);
                inputs[3].SwitchCurrentControlScheme("Keyboard_4", Keyboard.current);
            }
        }

        void Update()
        {
            for (int a = 0; a < inputs.Count; a++)
            {
                if (inputs[a].actions["kick"].WasPressedThisFrame())
                    { OnButtonPressed(1, a+1); print("KICK " + a);  }
                else if (inputs[a].actions["kick"].WasReleasedThisFrame())
                    { OnButtonReleased(1, a + 1); print("KICK release " + a); }

                if (inputs[a].actions["pass"].WasPressedThisFrame())
                    { OnButtonPressed(2, a + 1); }
                else if (inputs[a].actions["pass"].WasReleasedThisFrame())
                    { OnButtonReleased(2, a + 1);  }

                if (inputs[a].actions["run"].WasPressedThisFrame())
                    { OnButtonPressed(3, a + 1);  }
                else if (inputs[a].actions["run"].WasReleasedThisFrame())
                    { OnButtonReleased(3, a + 1);  }
                if (a == 0)
                {
                    if (inputs[a].actions["VolUp"].WasPressedThisFrame())
                    { Events.SetArcadeVolUp(); }
                    else if (inputs[a].actions["VolDown"].WasReleasedThisFrame())
                    { Events.SetArcadeVolDown(); }
                    else if (inputs[a].actions["Esc"].WasReleasedThisFrame())
                    { Events.EndGame(); }
                }
            }
        }

        public Vector2 GetMovement(int playerID)
        {
            return inputs[playerID - 1].actions["move"].ReadValue<Vector2>();
        }

    }
}
