using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public class PostProcessingFX : MonoBehaviour
{
    //public PostProcessProfile profile;
    //ChromaticAberration chromaticAberration;
    //Vignette vignette;

    //float value = 0;
    //float chromaticValue;
    //float chromaticSpeed = 60;
    //[SerializeField] bool setChromatic;

    void Start()
    {
        //Events.OnPostProcessingFX += OnPostProcessingFX;
      //  chromaticAberration = profile.GetSetting<ChromaticAberration>();
    }
    void OnDestroy()
    {
     //   Events.OnPostProcessingFX -= OnPostProcessingFX;
    }
    void OnPostProcessingFX(bool isOn)
    {
        //setChromatic = isOn;
        //if (isOn)
        //    chromaticValue = 0.5f;
        //else
        //    chromaticValue = 0;
        //value = chromaticValue;
        //SetValue();
    }
    public void SetOff()
    {
        //setChromatic = false;
        //chromaticValue = 0;
        //value = 0;
        //SetValue();
    }
    private void Update()
    {
        //if (!setChromatic) return;

        //if (chromaticValue>0 && value < chromaticValue)
        //{
        //    value += chromaticSpeed*Time.deltaTime;
        //} else if (chromaticValue == 0 && value > chromaticValue)
        //{
        //    value -= chromaticSpeed * Time.deltaTime;
        //    if (value < 0)
        //    {
        //        value = 0;
        //        SetOff();
        //    }
        //}
    }
    void SetValue()
    {
        //chromaticAberration.intensity.Override(value);
        //vignette.intensity.Override(value);
    }
}
