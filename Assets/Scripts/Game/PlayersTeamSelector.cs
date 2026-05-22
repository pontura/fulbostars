using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PlayersTeamSelector : MonoBehaviour
    {
        public PlayerTeamSignalUI[] all;
        public Text field;

        void Start()
        {
            all = GetComponentsInChildren<PlayerTeamSignalUI>();
            int id = 1;
            foreach (PlayerTeamSignalUI p in all)
            {
                p.Init(id);
                id++;
            }
            Events.OnRight += OnRight;
            Events.OnSkipOn(Go, "skip");

            int team1 = (int)Data.Instance.settings.selectedTeams[0];
            int team2 = (int)Data.Instance.settings.selectedTeams[1];
        }
        private void OnDestroy()
        {
            Events.OnRight -= OnRight;
        }
        void OnRight(int playerID, bool isRight)
        {
            print("Right playerID: " + playerID + " isRight: " + isRight);
            int id = playerID - 1;
            all[id].MoveRight(isRight);
            SetTotalPlayers();
        }
        void Go()
        {
            Events.OnSkipOff();
            if (AnyActive())
            {
                Data.Instance.LoadLevel("TeamSelector");
            }
            else
            {
                Events.OnSkipOn(Go, "continue");
            }
        }
        bool AnyActive()
        {
            bool oneActive = false;
            int id = 0;
            foreach (PlayerTeamSignalUI p in all)
            {
                if (p.teamID != 0)
                    oneActive = true;
                int teamID = p.teamID;
                if (teamID == 1) teamID = 2; else if (teamID == 2) teamID = 1;
                Data.Instance.matchData.AddPlayer(id + 1, teamID);
                id++;
            }
            if (oneActive)
                return true;
            return false;
        }
        void SetTotalPlayers()
        {
            int total = 0;
            foreach (PlayerTeamSignalUI p in all)
            {
                if (p.teamID != 0)
                    total++;
            }
            field.text = total + " JUGADOR/ES";
        }
    }
}
