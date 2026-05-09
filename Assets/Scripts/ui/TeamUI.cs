using Fulbo.Game;
using Fulbo.UI.EditTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TeamUI : MonoBehaviour
    {
        public Image bg;
        public Text scoreField;
        public Text teamNameField;
        public int teamID;
        [SerializeField] ClubShield clubShield;

        void Start()
        {
            Events.OnGoal += OnGoal;
            ClubData clubData = Data.Instance.clubsData.GetData(teamID);
            teamNameField.text = clubData.name_abr;
            bg.color = clubData.GetColor(1);
            clubShield.Init(clubData);
            SetField();
        }
        void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
        }
        void OnGoal(int _teamID, Character character)
        {
            SetField();
        }
        void SetField()
        {
            int score = 0;
            if (teamID == 1)
                score = (int)Data.Instance.matchData.score.x;
            else
                score = (int)Data.Instance.matchData.score.y;

            string text = score.ToString();

            scoreField.text = text;
        }
    }
}
