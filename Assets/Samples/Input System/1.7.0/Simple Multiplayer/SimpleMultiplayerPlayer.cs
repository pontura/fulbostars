using UnityEngine;
using UnityEngine.InputSystem;
using static Fulbo.Input.InputManagerGame;

public class SimpleMultiplayerPlayer : MonoBehaviour
{
    public UnityEngine.InputSystem.PlayerInput playerInput;

    private void Update()
    {
        if(playerInput.actions["move"].ReadValue<Vector2>() != Vector2.zero)
            print("Move " + gameObject.name + " value: " + playerInput.actions["move"].ReadValue<Vector2>());
    }

    public void OnTeleport()
    {
        print("OnTeleport " + gameObject.name);
    }
    public void OnShot()
    {
        print("OnShot " + gameObject.name);
    }
}
