using Fulbo.Game;
using Fulbo.Stadiums;
using Fulbo.Tournamets;
using Fulbo.UI;
using Fulbo.UI.EditTeam;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Fixture
{
    public class FixtureTeamSelector : MonoBehaviour
    {
        [SerializeField] GameObject playingAutomaticMatch;
        [SerializeField] PlayersTeamSelector playersTeamSelector;
        public MultipleCharactersScene multipleCharactersScene;
        public TournamentButton[] tournamentButtons;
        public CharacterCardInGame[] cards;
        public ClubShield[] clubShields;
        public Text[] teams;

        int teamID = 0;

        void Start()
        {
            Events.OnBackActive(true);
            PlayMusicIntro();
            
            LevelData team2 = Data.Instance.fixtureManager.GetNextTeamData(1);
            LevelData team1 = Data.Instance.fixtureManager.GetNextTeamData(2);

            print("FixtureTeamSelector " + team1.name);

            teams[0].text = team1.name.ToUpper();
            teams[1].text = team2.name.ToUpper();

            clubShields[0].Init(team2.clubData);
            clubShields[1].Init(team1.clubData);

            tournamentButtons[0].SetOn(team1.controlledInFixtures);
            tournamentButtons[1].SetOn(team2.controlledInFixtures);

            if(!team1.controlledInFixtures && !team2.controlledInFixtures)
            {
                playingAutomaticMatch.SetActive(true);
                Events.OnSkipOn(OnSkip, "TERMINA");
                playersTeamSelector.gameObject.SetActive(false);
            }
            else
            {
                playingAutomaticMatch.SetActive(false);
                playersTeamSelector.gameObject.SetActive(true);
            } 

            playersTeamSelector.IgnoreSize(!team1.controlledInFixtures, !team2.controlledInFixtures);
            
            Invoke("Delayed", 0.1f);
        }
        void OnSkip()
        {
            Events.OnSkipOff();
            Data.Instance.LoadLevel("GameOverPartymode");
        }
         public void PlayMusicIntro()
        {
            AudioManager.Instance.Play2Musics("music/intro", "music/music");
        }
         void Delayed()
        {
            cards[0].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team1[1], false), 10000);
            cards[1].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team2[1], false), 10000);
        }
        public void SetTeamActive(int teamID)
        {            
            tournamentButtons[teamID-1].SetOn(true);
            Data.Instance.tournamentsData.SetTournament(true);
        }
        void SelectTeam(int teamID) // si no es standalone lo managerea el control, si es standalone lo maneja el input
        {
            this.teamID = teamID;
            tournamentButtons[0].SetOn(false);
            tournamentButtons[1].SetOn(false);
            tournamentButtons[teamID-1].SetOn(true);
            Data.Instance.tournamentsData.SetTeam(teamID);

            Data.Instance.matchData.team1Controlled = teamID == 2;
            Data.Instance.matchData.team2Controlled = teamID == 1;
            if(teamID == 1)
                Data.Instance.matchData.players[0] = 2;
            else
                Data.Instance.matchData.players[0] = 1;
        }
        public void OnButtonClick(int a, int b)// si no es standalone lo managerea el control, si es standalone lo maneja el input
        {
            if (teamID == 0)
                return;

            AudioManager.Instance.FadeVolume("music", 0.3f);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_play_now");
            
            Events.OnButtonClick -= OnButtonClick;
            
            Data.Instance.matchData.SetTotalPlayers(8, 8);
            CharactersData.Instance.SetRandomReferi();
            Data.Instance.LoadLevel("PlayersTeamSelector");

        }
    }
}
