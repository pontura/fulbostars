using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Hiscores
{
    public class HiscoresUILine : MonoBehaviour
    {
        [SerializeField] Text teamName;
        [SerializeField] Text scoreField;

        public void Init(DB.DBMatches.MatchData data)
        {
            teamName.text = data.Name();

            if (data.score_team2 < data.score_team1)
                scoreField.color = Color.red;
            else if (data.score_team2 == data.score_team1)
                scoreField.color = Color.yellow;

            int length = data.max_full_score.ToString().Length;
            int score1 = int.Parse(data.max_full_score.ToString().Substring(length - 4, 2));
            int score2 = int.Parse(data.max_full_score.ToString().Substring(length - 2, 2));

            scoreField.text = data.max_score + " (" + score1 + "-" + score2 + ")";
        }
    }
}
