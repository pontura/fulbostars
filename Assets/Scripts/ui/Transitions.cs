using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitions : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject panel;
    System.Action OnReady;

    void Start()
    {
        Events.OnFade += OnFade;
        panel.SetActive(false);
    }
    void OnDestroy()
    {
        Events.OnFade -= OnFade;
    }
    void OnFade(bool isOn, System.Action OnReady)
    {
        StopAllCoroutines();
        this.OnReady = OnReady;
        panel.SetActive(true);
        if (isOn)
        {
            
            StartCoroutine(IsOn());
        }
    }
    IEnumerator IsOn()
    {
        AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_barrida_in");
        yield return new WaitForSecondsRealtime(0.1f);
        anim.Play("on");
        yield return new WaitForSecondsRealtime(0.85f);
        OnReady();
        yield return new WaitForSecondsRealtime(0.35f);
        AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_barrida_out");
        yield return new WaitForSecondsRealtime(0.1f);
        anim.Play("out");
        yield return new WaitForSecondsRealtime(0.8f);
        SetOff();
    }
    void SetOff()
    {
        panel.SetActive(false);
    }
}