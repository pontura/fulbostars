using UnityEngine;

namespace Fulbo.Tournamets
{
    public class TournamentsData : MonoBehaviour
    {
        public bool isTournament = true;
        public int myTeamID; // 1 o 2 (left o right)

        void Start()
        {
            myTeamID = 0;
        }
        public bool IsTournament()
        {
            return isTournament;
        }
        public void SetTeam(int teamID)
        {
            this.myTeamID = teamID;
        }
        public void SetTournament(bool isTournament)
        {
            this.isTournament = isTournament;
        }
    }
}