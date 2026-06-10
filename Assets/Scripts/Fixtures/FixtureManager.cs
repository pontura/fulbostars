using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Fixture
{
    public class FixtureManager : MonoBehaviour
    {
        public List<LevelData> teams;
        public List<LevelData> won;
        public List<Vector2> scores;
        public bool isFixtureHappening;

        public void Reset()
        {
            isFixtureHappening = false;
            teams.Clear();
            won.Clear();
            scores.Clear();
        }
        public void Init(List<LevelData> selectedTeams)
        {
            isFixtureHappening = true;
            this.teams = selectedTeams;

            foreach(LevelData  ld in teams)
                ld.controlledInFixtures = true;

            int totalSelected = selectedTeams.Count;
            int totalToAdd = 8-totalSelected;

            for(int a = 0; a<totalToAdd; a++)
                teams.Add(GetFreeRandomTeam());
            Utils.Shuffle(teams);

        }
        LevelData GetFreeRandomTeam()
        {
            List<LevelData> all = CupsData.Instance.levels.GetByState("on");
            LevelData ld = all[Random.Range(0, all.Count)];
            if(teams.Contains(ld))
            {
                ld.controlledInFixtures = false;
                return GetFreeRandomTeam();
            }
            return ld;
        }
        public void GameOver()
        {
            int score1 = (int)Data.Instance.matchData.score.x;
            int score2 = (int)Data.Instance.matchData.score.y;
            if(score1>score2)
                won.Add(GetNextTeamData(1));
            else
                won.Add(GetNextTeamData(2));

            scores.Add(Data.Instance.matchData.score);
            Data.Instance.matchData.Reset();
        }
        public int GetNextTeam(int teamID)
        {
            string team_tag;
            if(won.Count==4)
            {
                if(teamID == 1)
                    team_tag = won[0].team_tag;
                else
                    team_tag = won[1].team_tag;
            } else if(won.Count==5)
            {
                if(teamID == 1)
                    team_tag = won[2].team_tag;
                else
                    team_tag = won[3].team_tag;
            } else if(won.Count==6)
            {
                if(teamID == 1)
                    team_tag = won[4].team_tag;
                else
                    team_tag = won[5].team_tag;
            } else if(won.Count==7)
            {
                if(teamID == 1)
                    team_tag = won[6].team_tag;
                else
                    team_tag = won[7].team_tag;
            } else
            {
                int nextTeam = (won.Count *2)+teamID;
                team_tag = teams[nextTeam-1].team_tag;
            }
            int id = 0;

            foreach(LevelData l in CupsData.Instance.levels.content)
            {
                if(l.team_tag == team_tag)
                    return id;
                id++;
            }
            return 0;
        }
         public LevelData GetNextTeamData(int teamID)
        {
            string team_tag = "";
            if(won.Count<4)
            {
                int nextTeam = (won.Count *2)+teamID;
                team_tag = teams[nextTeam-1].team_tag;
            } else if(won.Count==4)
            {
                team_tag = won[teamID-1].team_tag;
            } else if(won.Count==5)
            {
                team_tag = won[2+teamID-1].team_tag;
            } else if(won.Count==6)
            {
                team_tag = won[4+teamID-1].team_tag;
            }

            int id = 0;
            foreach(LevelData l in CupsData.Instance.levels.content)
            {
                if(l.team_tag == team_tag)
                    return l;
                id++;
            }
            Debug.LogError("No hay next team en fixture");
            return null;
        }
    }
}
