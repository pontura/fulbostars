using Fulbo.Game;
using Fulbo.Stadiums;
using Fulbo.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

namespace Fulbo.Tournamets
{
    public class TournamentSelector : MonoBehaviour
    {
        public MultipleCharactersScene multipleCharactersScene;
        public TournamentButton[] tournamentButtons;
        public CharacterCardInGame[] cards;
        public Text[] teams;
        public Text[] goles;
        public Text gamesPlayedField;

        int teamID = 0;

        void Start()
        {
            Events.OnBackActive(Data.Instance.tournamentsData.played);
            print("SPLASK");
            PlayMusicIntro();
            Data.Instance.tournamentsData.Refresh(OnDataRefreshed);
            SetResults();
            
            List<LevelData> teams = CupsData.Instance.levels.GetByState("torneo");
        
            teams[0].name = teams[0].name;
            teams[1].name = teams[1].name;

            multipleCharactersScene.TounrnamentMode();

            Events.OnRight += OnRight; 

if(tournamentButtons.Length>0)
{
            tournamentButtons[0].SetOn(false);
            tournamentButtons[1].SetOn(false);
}
            
            Invoke("Delayed", 0.1f);
        }
         public void PlayMusicIntro()
        {
            AudioManager.Instance.Play2Musics("music/intro", "music/music");
        }
         void Delayed()
        {
            cards[0].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team1[1], false), 10000);
            cards[1].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team2[1], false), 10000);
            
            Events.OnSkipOff();
        }
        void SetResults()
        {
            goles[0].text = Data.Instance.tournamentsData.goles2.ToString();
            goles[1].text = Data.Instance.tournamentsData.goles1.ToString();
            gamesPlayedField.text = Data.Instance.tournamentsData.gamesPlayed.ToString();
        }
        private void OnDataRefreshed()
        {
            SetResults();
        }

        public void OnDestroyed()
        {
            Events.OnRight -= OnRight;
        }
        void OnDestroy()
        {
            Events.OnRight -= OnRight;
        }
        bool canSelect = false;
        void OnRight(int playerID, bool isRight)
        {
            if (!canSelect)
            {
#if !UNITY_STANDALONE
                Events.OnButtonClick += OnButtonClick;
#endif
                canSelect = true;
            }
            if (isRight)
                SelectTeam(2);
            else 
                SelectTeam(1);
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
           
        }
        public void OnClicked(int teamID) // from ui
        {
            SelectTeam(teamID);
            this.teamID = teamID;
            OnButtonClick(0,0);
        }
        void OnButtonClick(int a, int b)// si no es standalone lo managerea el control, si es standalone lo maneja el input
        {
            if (teamID == 0)
                return;

            if(teamID == 1)
                Data.Instance.matchData.players[0] = 2;
            else
                Data.Instance.matchData.players[0] = 1;
                
            Events.OnSkipOff();

            AudioManager.Instance.FadeVolume("music", 0.3f);
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_play_now");
            
            Events.OnButtonClick -= OnButtonClick;
            
            Data.Instance.matchData.SetTotalPlayers(8, 8);
            CharactersData.Instance.SetRandomReferi();
            if(Data.Instance.isMobile)
                Data.Instance.LoadLevel("GameIntro");
            else{
#if UNITY_STANDALONE
            Data.Instance.LoadLevel("PlayersTeamSelector");
#else
            Data.Instance.LoadLevel("Controls");
#endif
                }
        }
    }
}
