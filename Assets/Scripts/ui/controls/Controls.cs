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
            if(Data.Instance.tournamentsData.IsTournament())
                Data.Instance.LoadLevel("GameIntro");
            else
                Data.Instance.LoadLevel("PlayersTeamSelector");

            Events.OnSkipOff();
        }
    }

}
