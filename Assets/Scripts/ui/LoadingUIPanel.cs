using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUIPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;

    void Start()
    {
        Events.OnLoadingPanel += OnLoadingPanel;
        OnLoadingPanel(false);
    }
    void OnDestroy()
    {
        Events.OnLoadingPanel -= OnLoadingPanel;
    }

    void OnLoadingPanel(bool isOn)
    {
        panel.SetActive(isOn);
    }
}
