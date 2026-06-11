using System;
using System.Collections;
using System.Collections.Generic;
using Fulbo;
using UnityEngine;

public class WEBGLMobilePanel : MonoBehaviour
{
    [SerializeField] GameObject[] showIfWebGLMobile;
    [SerializeField] GameObject[] hideIfWebGLMobile;
    void Start()
    {
        foreach(GameObject go in showIfWebGLMobile)
            go.SetActive(Data.Instance.isMobile);

        foreach(GameObject go in hideIfWebGLMobile)
            go.SetActive(!Data.Instance.isMobile);
    }
}
