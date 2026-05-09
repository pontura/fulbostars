using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    public Text field;
    System.Action OnDone;
    public string[] allFields;
    int id;
    public bool isOn;
    public bool isSplashPopup;

    void Start()
    {
        Events.OnPopup += OnPopup;
        Events.OnPopupForceSkip += OnPopupForceSkip;
        panel.SetActive(false);
    }
    void OnDestroy()
    {
        Events.OnPopup -= OnPopup;
        Events.OnPopupForceSkip -= OnPopupForceSkip;
    }
    void OnPopup(string text, System.Action OnDone)
    {
        isOn = true;
        //print("OnPopup " + text);
        if(!isSplashPopup)
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_popup_alien");
        this.OnDone = OnDone;
        id = 0;
        allFields = text.Split("_"[0]);
        if (allFields.Length > 1)
            SetNextText();
        else
            field.text = text;
        panel.SetActive(true);
    }
    void SetNextText()
    {       
        field.text = allFields[id];
        id++;
    }
    void OnPopupForceSkip()
    {
        OnClick();
    }
    public void OnClick()
    {
        if (!isSplashPopup)
            AudioManager.Instance.PlaySound("common", "ui/click", false);
        CancelInvoke();

        if (allFields.Length > 1 && id < allFields.Length)
            Invoke("SetNextText", 0.25f);
        else
        {
            isOn = false;
            panel.GetComponent<Animator>().Play("clicked");
            Invoke("CloseDelayed", 0.25f);
            if (OnDone != null)
                OnDone();
            OnDone = null;
        }
    }
    void CloseDelayed()
    {
        panel.SetActive(false);        
    }
}
