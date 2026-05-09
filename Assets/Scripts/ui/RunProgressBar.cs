namespace Fulbo.UI
{
    public class RunProgressBar : ProgressBar
    {
        void Start()
        {
            SetOff();
            Events.OnRun += OnRun;
        }
        void OnDestroy()
        {
            SetOff();
            Events.OnRun -= OnRun;
        }
        void OnRun(float duration)
        {
           // AudioManager.Instance.PlaySoundOneShot("ui", "ui/game_sprint");            
            Init(duration, null);
        }
    }
}