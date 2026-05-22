using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadDebug : MonoBehaviour
{
    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        // También loguear los que ya están conectados al arrancar
        foreach (var d in InputSystem.devices)
            Debug.Log($"[ARRANQUE] {d.name} | {d.displayName} | layout: {d.layout}");
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log($"[CHANGE] {change} | name: {device.name} | display: {device.displayName} | layout: {device.layout} | desc: {device.description.ToJson()}");
    }
}