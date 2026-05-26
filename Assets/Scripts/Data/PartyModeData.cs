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

        [SerializeField] ClubData introSettings; // guardados en el init:

        private void Start()
        {
            introSettings = team2;
        }
        public ClubData GetDefaultClubSettings()
        {
            return introSettings;
        }
        public void Reset()
        {
        }

        public ClubData GetDataForPartyMode(int teamID)
        {
            if (teamID == 0)
            {
                ClubData clubData = Data.Instance.partyModeData.GetDefaultClubSettings();
                return clubData;
            }
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
            print("PartyModeData set team id: " + id);
            if (teamID == 1)
                teamID_1 = id;
            else
                teamID_2 = id;
        }
        public void SetClubData(ClubData clubData, int teamID)
        {
            print("PartyModeData SetClubData set team id: " + teamID);
            if (teamID == 1)
                team1 = clubData;
            else
                team2 = clubData;
        }
    }
}
