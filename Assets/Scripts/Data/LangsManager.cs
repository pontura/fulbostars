using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangsManager : MonoBehaviour
{
    [SerializeField] string lang = "es";

    public void Init()
    {
       // lang = PlayerPrefs.GetString("lang", "es");
    }
    public string GetLang()
    {
        return lang.ToString().ToLower();
    }
    public void SetLang(string _lang)
    {
        PlayerPrefs.SetString("lang", _lang);
        lang = _lang;
    }
}
