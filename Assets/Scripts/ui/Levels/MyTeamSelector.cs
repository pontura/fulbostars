using Fulbo.Game;
using Fulbo.Stadiums;
using Fulbo.UI.EditTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.CharactersData;

namespace Fulbo.UI
{
    public class MyTeamSelector : CascadeList
    {
        public CharacterCardInGame team1;
        public CharacterCardInGame team2;

        public PositionsUIManager positionsUIManager_team1;
        public PositionsUIManager positionsUIManager_team2;
        public int myTeamPositionID;
        public ClubShield[] clubShields;
        [SerializeField] Animation anim;
        [SerializeField] Image backgroundImage;
        [SerializeField] GameObject oponentScreen;
        [SerializeField] Transform stadiumContainer;
        public Text[] teamNames;
        LevelData levelData;
        int totalPos;
        int posID_team_1;
        int posID_team_2;
        public void Start()
        {
            StadiumsData.Instance.SetRandomStadium();
            Events.OnSkipOn(OnSkip, "skip");
            Data.Instance.matchData.SetTotalPlayers(8, 8);

            CupsData.Instance.levels.InitMultiplayer();

            totalPos = Data.Instance.charactersPositions.team1.all.Length;

            posID_team_1 = UnityEngine.Random.Range(0, totalPos-1);
            posID_team_2 = UnityEngine.Random.Range(0, totalPos-1);

            InitTeam(1, posID_team_1);
            InitTeam(2, posID_team_2);

            InitStadium();

            if (Data.Instance.partyModeData.teamID_1 == Data.Instance.partyModeData.teamID_2)
                OnRight(1, true);//para evitar los 2 teams iguales:

            Events.OnRight += OnRight;
            Events.OnUp += OnUp; 
            Events.SetArcadeVolUp += SetArcadeVolUp;
            Events.SetArcadeVolDown += SetArcadeVolDown;
        }
        private void OnDestroy()
        {
            Events.OnRight -= OnRight;
            Events.OnUp -= OnUp;
            Events.SetArcadeVolUp -= SetArcadeVolUp;
            Events.SetArcadeVolDown -= SetArcadeVolDown;
        }
        void SetArcadeVolUp()
        {
            int team_ID = 1;
            CupsData.Instance.levels.ChangeMultiplayerTeam(team_ID, true);

            InitTeam(team_ID, posID_team_1);

            if (Data.Instance.partyModeData.teamID_1 == Data.Instance.partyModeData.teamID_2)
                OnRight(team_ID, true);
        }
        void SetArcadeVolDown()
        {
            int team_ID = 1;
            CupsData.Instance.levels.ChangeMultiplayerTeam(team_ID, false);

            InitTeam(team_ID, posID_team_1);

            if (Data.Instance.partyModeData.teamID_1 == Data.Instance.partyModeData.teamID_2)
                OnRight(team_ID, false);
        }
        private void OnUp(int playerID, bool up)
        {
            int team_ID = Data.Instance.matchData.players[playerID - 1];
            if (team_ID == 1)
            {
                if (up) posID_team_1++; else posID_team_1--;
                if (posID_team_1 < 0) posID_team_1 = totalPos - 1; else if (posID_team_1 > totalPos - 1) posID_team_1 = 0;
                InitTeam(team_ID, posID_team_1);
            }
            else
            {
                if (up) posID_team_2++; else posID_team_2--;
                if (posID_team_2 < 0) posID_team_2 = totalPos - 1; else if (posID_team_2 > totalPos - 1) posID_team_2 = 0;
                InitTeam(team_ID, posID_team_2);
            }
        }

        private void OnRight(int playerID, bool right)
        {
            int team_ID = Data.Instance.matchData.players[playerID - 1];           
            CupsData.Instance.levels.ChangeMultiplayerTeam(team_ID, right);          

            if (team_ID == 1)
                InitTeam(1, posID_team_1);
            else
                InitTeam(2, posID_team_2);

            if (Data.Instance.partyModeData.teamID_1 == Data.Instance.partyModeData.teamID_2)
                OnRight(playerID, right);
        }

        void InitTeam(int teamID, int posID)
        {
            StartTeam(teamID);
            CharactersPositions.All posData;
            List<int> team = Data.Instance.matchData.GetTeam(teamID);
            if (teamID == 1)
            {
                posData = Data.Instance.charactersPositions.team2;
                positionsUIManager_team1.Init(posData.all[posID], team, teamID);
            }
            else
            {
                posData = Data.Instance.charactersPositions.team1;
                positionsUIManager_team2.Init(posData.all[posID], team, teamID);
            }
            if(teamID == 1 || teamID == 2)
                Data.Instance.matchData.charactersPositions[teamID - 1] = posID;
            SetTeamNames();
        }
        public void SetTeamNames()
        {
            LevelData team_1_Data = CupsData.Instance.levels.GetByState("on")[Data.Instance.partyModeData.teamID_1];
            LevelData team_2_Data = CupsData.Instance.levels.GetByState("on")[Data.Instance.partyModeData.teamID_2];

            teamNames[0].text = team_1_Data.name;
            teamNames[1].text = team_2_Data.name;

            clubShields[0].Init(team_1_Data.clubData);
            clubShields[1].Init(team_2_Data.clubData);
        }
        void GotoGameIntro() {
            Data.Instance.matchData.SetMyPosition(myTeamPositionID);
            Data.Instance.LoadLevel("GameIntro");
        }
        void InitStadium()
        {
            Utils.RemoveAllChildsIn(stadiumContainer);
            StadiumsData stadiumsData = StadiumsData.Instance;
            GameObject asset = stadiumsData.active.GetAssetBySelectedSize().asset;
            Instantiate(asset, stadiumContainer);
        }
        void OnSkip()
        {
            Data.Instance.LoadLevel("GameIntro");
            Events.OnSkipOff();
        }
        Coroutine team1C;
        Coroutine team2C;

        void StartTeam(int teamID)
        {
            int characterID;
            if (teamID == 1)
            {
                
                if (team1C != null) StopCoroutine(team1C);
                team1C = StartCoroutine(Team1());
            }
            else
            {
                if (team2C != null) StopCoroutine(team2C);
                team2C = StartCoroutine(Team2());
            }

        }
        int timer = 2;
        IEnumerator Team1()
        {
            int id = 0;
            foreach (int chID in Data.Instance.matchData.team1)
            {
                id++;
                CharacterData character = CharactersData.Instance.GetCharacterData(chID, id == 1);
                team1.ForceShow(character, timer);
                yield return new WaitForSeconds(timer + 1);
            }
        }
        IEnumerator Team2()
        {
            int id = 0;
            foreach (int chID in Data.Instance.matchData.team2)
            {
                id++;
                CharacterData character = CharactersData.Instance.GetCharacterData(chID, id == 1);
                team2.ForceShow(character, timer);
                yield return new WaitForSeconds(timer + 1);
            }
        }
    }
}