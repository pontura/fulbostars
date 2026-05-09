using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fulbo.CupsData;

namespace Fulbo
{
    public class PartyModeData : MonoBehaviour
    {
        public int teamID_1;
        public int teamID_2;

        [SerializeField] ClubData team1;
        [SerializeField] ClubData team2;

        public ClubData GetDataForPartyMode(int teamID)
        {
           switch(teamID)
            {
                case 1:
                    return team1;
                default:
                    return team2;
            }
        }
        public void SetTeamID(int teamID, int id)
        {
            if (teamID == 1)
                teamID_1 = id;
            else
                teamID_2 = id;
        }
        public void SetClubData(ClubData clubData, int teamID)
        {
            if (teamID == 1)
                team1 = clubData;
            else
                team2 = clubData;
        }
    }
}
