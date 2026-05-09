using UnityEngine;

namespace Fulbo.Input
{
    public class InputManagerUI : MonoBehaviour
    {
        [SerializeField] int[] last_x = new int[4] { 0,0,0,0 };
        int[] last_y = new int[4] { 0,0,0,0 };

        void Loop()
        {
            for (int a = 1; a < 4 + 1; a++)
            {
                Vector2 movement = InputManager.Instance.GetMovement(a);
                if (movement == null) return;
                int newX = (int)Mathf.Ceil(movement.x);
                if (last_x[a-1] != newX)
                {
                    if(newX!=0)
                        Events.OnRight(a, newX == 1);
                    last_x[a - 1] = newX;
                }
                int newY = (int)Mathf.Ceil(movement.y);
                if (last_y[a - 1] != newY)
                {
                    if (newY != 0)
                        Events.OnUp(a, newY == 1);
                    last_y[a - 1] = newY;
                }
            }
            Invoke("Loop", 0.1f);
        }
        void Start()
        {
            Loop();
            InputManager.Instance.OnButtonPressed += OnButtonPressed;
            InputManager.Instance.OnButtonReleased += OnButtonReleased;
        }
        private void OnDestroy()
        {
            InputManager.Instance.OnButtonPressed  -= OnButtonPressed;
            InputManager.Instance.OnButtonReleased -= OnButtonReleased;
        }
        public void OnDisabled() { CancelInvoke(); }
        public enum buttonTypes
        {
            BUTTON_1,
            BUTTON_2,
            BUTTON_3
        }
        //public override void OnKickPressedss(int playerID)
        //{
        //    Events.OnSkipButtonPress();
        //}
        public void OnButtonPressed(int buttonID, int playerID)
        {
            Events.OnButtonDown(buttonID, playerID);
        }
        public  void OnButtonReleased(int buttonID, int playerID)
        {
            Events.OnButtonClick(buttonID, playerID);
        }
    }

}