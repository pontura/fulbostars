using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharactersManagerGameover : CharactersManager
    {
        public int scoreWinned;

        public override void Init(bool isBillboard = false)
        {
            AudioManager.Instance.PlaySpecificSound(Fulbo.Stadiums.StadiumsData.Instance.active.ambience_end_loop, "ambience", true);
            SetScore();

            CharactersConstructor cc = GetComponent<CharactersConstructor>();
            if (cc != null) cc.AddCharacters();

            int teamWon = Data.Instance.matchData.GetWinner();
            if(teamWon == 2)
                teamWon = 2;
            else     
                teamWon = 1;   

            ball = Fulbo.Game.GameManager.Instance.ball;
            referi.InitReferi(this, CharactersData.Instance.GetReferi().asset);
            print("CharactersManagerGameover teamWon: "+ teamWon);
            SetCharactersToTeam(teamWon, false, true, true);
        }
        void SetScore()
        {
            scoreWinned = 5;
            if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
            {
                scoreWinned += (int)Data.Instance.matchData.score.x - (int)Data.Instance.matchData.score.y;
                scoreWinned *= 10;
            }
            Events.OnGoldScoreWin(scoreWinned, Vector3.zero);
        }
    }

}