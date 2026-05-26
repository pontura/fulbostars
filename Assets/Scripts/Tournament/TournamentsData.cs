using System;
using Fulbo.DB;
using UnityEngine;

namespace Fulbo.Tournamets
{
    public class TournamentsData : MonoBehaviour
    {
        public bool isTournament = true;
        public int myTeamID; // 1 o 2 (left o right)
        public int goles1; 
        public int goles2; 
        public int gamesPlayed;

        void Start()
        {
            myTeamID = 0;
            DBManager.Instance.tournamentsManager.GetResults("torneo1", OnLoadedResults);
        }
        System.Action OnRefreshed;
        public void Refresh(System.Action OnRefreshed)
        {
            this.OnRefreshed = OnRefreshed;
            DBManager.Instance.tournamentsManager.GetResults("torneo1", OnLoadedResults);
        }

        private void OnLoadedResults(bool success, int goles1, int goles2, int gamesPlayed)
        {
            this.goles1 = goles1;
            this.goles2 = goles2;
            this.gamesPlayed = gamesPlayed;
            if(OnRefreshed != null)
                OnRefreshed.Invoke();
        }

        public bool IsTournament()
        {
            return isTournament;
        }
        public void SetTeam(int teamID)
        {
            if(teamID>0)
            {
                isTournament = true;
                this.myTeamID = teamID;
            } else
                isTournament = false;
        }
        public void SetTournament(bool isTournament)
        {
            this.isTournament = isTournament;
        }
    }
}