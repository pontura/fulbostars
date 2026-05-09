using UnityEngine;

namespace Fulbo.UI
{
    public class Controls : MonoBehaviour
    {
        void Start()
        {
            Events.OnSkipOn(OnSkip, "skip");
        }
        void OnSkip()
        {
            Data.Instance.LoadLevel("PlayersTeamSelector");
            Events.OnSkipOff();
        }
    }

}
