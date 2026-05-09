using UnityEngine;
using System.Collections;
using Fulbo.DB;
using System.Collections.Generic;

namespace Fulbo
{
    public class MyTeamData
    {
        MyTeam myTeam;
        MatchData matchData;

        public void Init(MyTeam myTeam)
        {
            this.myTeam = myTeam;
            matchData = Data.Instance.matchData;
        }

        System.Action OnDataSaved;
        public void GameOver(int xtrasScore, System.Action OnDataSaved)
        {
            Events.ScoreFreezed(true);//no updates from server; 
            Debug.Log("GAME OVER: xtrasScore:" + xtrasScore);

            this.OnDataSaved = OnDataSaved;
            LevelData levelData = CupsData.Instance.GetActualLevel();
            int myscore = (int)matchData.score.y;
            int opponentScore = (int)matchData.score.x;

            if (myscore > opponentScore)
                levelData.locked = false;

            DBMatches.MatchData dbTrackData = new DBMatches.MatchData();
            dbTrackData.cup = levelData.cupID;
            dbTrackData.tier = levelData.tier;
            dbTrackData.stadium = levelData.stadium_id;
            dbTrackData.level = Data.Instance.matchData.levelData.id;
            dbTrackData.score_team1 = opponentScore;
            dbTrackData.score_team2 = myscore;
            dbTrackData.matches = DBManager.Instance.DbUserData.data.gamesPlayed;
            MatchStats matchStats = Data.Instance.GetComponent<MatchStats>();
            dbTrackData.ball_possesion_team1 = (int)matchStats.teams[0].ball_possesion;
            dbTrackData.ball_possesion_team2 = (int)matchStats.teams[1].ball_possesion;
            dbTrackData.kicks_passes_team1 = (int)matchStats.teams[0].kicks_passes;
            dbTrackData.kicks_passes_team2 = (int)matchStats.teams[1].kicks_passes;
            dbTrackData.kicks_to_goal_team1 = (int)matchStats.teams[0].kicks_to_goal;
            dbTrackData.kicks_to_goal_team2 = (int)matchStats.teams[1].kicks_to_goal;
            dbTrackData.balls_to_referi = (int)matchStats.teams[0].balls_to_referi;
            dbTrackData.centros_team1 = (int)matchStats.teams[0].centros;
            dbTrackData.centros_team2 = (int)matchStats.teams[1].centros;
            dbTrackData.coins_grabbed = (int)matchStats.teams[1].coins_grabbed;
            dbTrackData.score = xtrasScore;

          //  DB.DBManager.Instance.DbUserData.data.AddScore(totalWonScore);

            DBManager.Instance.DbUserData.data.gamesPlayed++;
           // DBManager.Instance.DbMatches.Add(dbTrackData);


            //Add players:
            dbTrackData.stats = new DBMatches.PlayersThatPlayed();
            dbTrackData.stats.players = new List<DBMatches.PlayerData>();
            int totalPlayers = Data.Instance.matchData.GetTotalPlayersInMatch(2);
            DBGameData.DBFormationSave formation = DBManager.Instance.DbUserData.data.gameData.GetFormation(totalPlayers);
            foreach (DBGameData.DBFormationSave.DBFormationChar player in formation.formation)
            {
                DBMatches.PlayerData playerData = new DBMatches.PlayerData();
                playerData.id = player.uniqueID;
                dbTrackData.stats.players.Add(playerData);
            }

            //CheckforUnlockLevels();
            Data.Instance.matchData.cameFromMatch = true;
            /////////////////
            if(Data.Instance.mode == Data.modes.STORYMODE)
                DBEvents.Track(dbTrackData, OnTrackDone);

            //Analytics
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["cup"] = Data.Instance.matchData.levelData.cupID;
            param["tier"] = Data.Instance.matchData.levelData.tier;
            param["stadium"] = Data.Instance.matchData.levelData.stadium_id;
            param["level"] = Data.Instance.matchData.levelData.id;
            param["scorePlayer"] = (int)dbTrackData.score_team2;
            param["scoreOpponent"] = (int)dbTrackData.score_team1;
            param["teamPower"] = Data.Instance.matchData.dataOnInit.myTeamPower;

            if (myscore > opponentScore)        param["result"] = "WIN";
            else if (myscore < opponentScore)   param["result"] = "LOSE";
            else                                param["result"] = "DRAW";
            Events.OnTrack("MatchEnded", param);
            //////////////////////////////////

        }

        void OnTrackDone(bool isOK, string response)
        {
            if (!isOK)
            {
                Events.OnPopup("Bad Conection! Data will not be saved", null);
                AllDataSavedAndReloaded(); // Sigue... sin guardar nada.
            }
            else
            {
                Debug.Log("TRACK RESPONSE: " + response);
                SimpleJSON.JSONNode jsonNode = SimpleJSON.JSON.Parse(response);
                Data.Instance.matchData.SetResponse(jsonNode);
                DB.DBEvents.LoadUserData(AllDataSavedAndReloaded);
                //AllDataSavedAndReloaded();
            }
        }

        void AllDataSavedAndReloaded()
        {
            if (OnDataSaved != null)
            {
                OnDataSaved();
                OnDataSaved = null;
            }
        }
    }
}
