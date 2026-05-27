using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLGamepadFix : MonoBehaviour
{
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
        if (_isReregistering) return;
        if (device.layout != "WebGLGamepad") return;

        if (change == InputDeviceChange.Added && !_connectedGamepads.Contains(device))
            _connectedGamepads.Add(device);
        else if (change == InputDeviceChange.Removed)
            _connectedGamepads.Remove(device);
        else
            return;

        _isReregistering = true;
        RebindGamepads();
        _isReregistering = false;
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