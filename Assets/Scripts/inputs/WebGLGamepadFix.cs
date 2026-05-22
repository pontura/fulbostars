using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLGamepadFix : MonoBehaviour
{
    private bool _isReregistering = false;

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
        if (change != InputDeviceChange.Added) return;
        if (device.layout != "WebGLGamepad") return;

        Debug.Log("[FIX] WebGLGamepad detectado, forzando pair con PlayerInput...");
        _isReregistering = true;

        var playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (var pi in playerInputs)
        {
            pi.SwitchCurrentControlScheme("Gamepad", device);
            Debug.Log($"[FIX] SwitchControlScheme aplicado a {pi.gameObject.name}");
        }

        _isReregistering = false;
    }
}