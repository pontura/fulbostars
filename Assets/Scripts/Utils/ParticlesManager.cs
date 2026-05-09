using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    public float duration;
    public bool fadeRateToZero;
    float initialRate;
    float totalDuration;
    bool initialized;

    void OnInit()
    {
        totalDuration = duration;
        if(particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (fadeRateToZero && particleSystem != null)
            initialRate = particleSystem.emission.rateOverTime.constant;
        initialized = true;
    }
    public void Init(int _duration = 0)
    {
        if (_duration > 0)
            totalDuration = _duration;
        gameObject.SetActive(true);
        if (!initialized)
            OnInit();
        duration = totalDuration;
        if (fadeRateToZero && particleSystem != null)
        {
            var emission = particleSystem.emission;
            emission.rateOverTime = initialRate;
        }
    }
    void Update()
    {
        if (particleSystem == null) return;
        duration -= Time.deltaTime;
        if (fadeRateToZero)
        {
            var emission = particleSystem.emission;
            if (duration > 0)
                emission.rateOverTime = duration * initialRate / totalDuration;
            if (duration < -1)
                SetOff();
            return;
        }
        if (duration <= 0)
            SetOff();
    }
    public void SetOff()
    {
        gameObject.SetActive(false);
    }
}
