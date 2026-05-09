using UnityEngine;

public class CharacterFXManager : MonoBehaviour
{
    public ParticlesManager superRun;
    public ParticlesManager powerUpSpeedRun;
    public ParticlesManager dash;

    private void Awake()
    {
        Reset();
    }
    public void OnSuperRun()
    {
        superRun.Init();
    }
    public void OnPowerupSuperRun(float timer)
    {
        powerUpSpeedRun.Init((int)timer);
    }
    public void OnDash(float timer)
    {
        dash.Init();
    }
    public void Reset()
    {
        superRun.SetOff();
        powerUpSpeedRun.SetOff();
        dash.SetOff();
    }
}
