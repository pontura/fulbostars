using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WebGLGamepadFix : MonoBehaviour
{
    public GameObject joysticksPanel;
    public Text  joysticksField;
    public int maxPlayers = 4;
    public int playersQty = 0; // cuantos joysticks hay conectados, se actualiza en tiempo real
    
    private bool _isReregistering = false;
    private readonly List<InputDevice> _connectedGamepads = new();

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
#if UNITY_WEBGL
        if (_isReregistering) return;
        if (device.layout != "WebGLGamepad") return;

        if (change == InputDeviceChange.Added && !_connectedGamepads.Contains(device))
            _connectedGamepads.Add(device);
        else if (change == InputDeviceChange.Removed)
            _connectedGamepads.Remove(device);
        else
            return;

        // actualizar cantidad expuesta de joysticks conectados
        playersQty = _connectedGamepads.Count;
        Events.TotalJoysticks(playersQty);

        if (joysticksField && playersQty>0)
        {
            CancelInvoke();
            joysticksPanel.SetActive(true);
            joysticksField.text = $"{playersQty}/{maxPlayers} jugadores";
            Invoke("CloseJoysticksPanel", 2f);
        }


        _isReregistering = true;
        RebindGamepads();
        _isReregistering = false;
#else
        if (change == InputDeviceChange.Added && !_connectedGamepads.Contains(device))
            _connectedGamepads.Add(device);
        else if (change == InputDeviceChange.Removed)
            _connectedGamepads.Remove(device);
        else
            return;

        playersQty = _connectedGamepads.Count;
        Events.TotalJoysticks(playersQty);

        if (joysticksField)
        {
            CancelInvoke();
            joysticksPanel.SetActive(true);
            joysticksField.text = $"{playersQty}/{maxPlayers} jugadores";
            Invoke("CloseJoysticksPanel", 2f);
        }
#endif
    }
    void CloseJoysticksPanel()
    {
        joysticksPanel.SetActive(false);
    }
    void RebindGamepads()
    {
        var playerInputs = FindObjectsOfType<PlayerInput>()
            .OrderBy(pi => pi.playerIndex)
            .ToArray();

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (i < _connectedGamepads.Count)
            {
                playerInputs[i].SwitchCurrentControlScheme("Gamepad", _connectedGamepads[i]);
                Debug.Log($"[FIX] Gamepad {i} → {playerInputs[i].gameObject.name}");
            }
        }
    }
}