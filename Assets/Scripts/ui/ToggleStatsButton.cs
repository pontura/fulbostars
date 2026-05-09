using UnityEngine;

namespace Fulbo.UI
{
    public class ToggleStatsButton : MonoBehaviour
    {
        bool statsOn;

        public void ToggleStats()
        {
            statsOn = !statsOn;
            Events.ToggleStats(statsOn);
        }
    }
}
