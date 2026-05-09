using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Dashoard
{

    public class DashboardSignal : MonoBehaviour
    {
        public DashboardSignalContent[] content;
       // List<int> randomCharacters;

        public void Init(DashboardUI dashboardUI)
        {
          //  randomCharacters = CharactersData.Instance.GetAvailablePlayersID(false);
            foreach (DashboardSignalContent cData in content)
            {
                Add(cData);
            }
        }
        void Add(DashboardSignalContent signal)
        {
            DashboardContentData data = null;
            DashboardContentData dcData = DashboardData.Instance.GetByType(signal.type);
            if (dcData != null)
            {
                dcData.characterID = GetCharacter(2);
                dcData.characterID2 = GetCharacter(1);
                signal.Init(dcData);
            }
            else
                Destroy(this.gameObject);
        }
        int GetCharacter(int teamID = 0)
        {
            //if (teamID == 0)
            //{
            //    return randomCharacters[UnityEngine.Random.Range(0, randomCharacters.Count)];
            //}
            //else
            //{
                if (teamID == 1)
                    return GetCharacterForTeam(1);
                else// if (Data.Instance.matchData.GetTotalPlayersInMatch(2) > 0)
                    return GetCharacterForTeam(2);
                //else
                //    return randomCharacters[UnityEngine.Random.Range(0, randomCharacters.Count)];
           // }
        }
        int GetCharacterForTeam(int teamID)
        {
            if (teamID == 1)
            {
                //if(Data.Instance.matchData.team1_goals.Count>0)
                //    return Data.Instance.matchData.team1_goals[Data.Instance.matchData.team1_goals.Count - 1];
                // return Data.Instance.matchData.team1_goals[Random.Range(0, Data.Instance.matchData.team1_goals.Count)];
                return Data.Instance.matchData.GetTeam(1)[Random.Range(1, Data.Instance.matchData.GetTotalPlayersInMatch(1))];
            } else
            {
                //if (Data.Instance.matchData.team2_goals.Count > 0)
                //    return Data.Instance.matchData.team2_goals[Data.Instance.matchData.team2_goals.Count-1];
                //  return Data.Instance.matchData.team2_goals[Random.Range(0, Data.Instance.matchData.team2_goals.Count)];
                return Data.Instance.matchData.GetTeam(2)[Random.Range(1, Data.Instance.matchData.GetTotalPlayersInMatch(2))];
            }
        }
    }

}