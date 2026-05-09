using Fulbo.DB;
using Fulbo.Mundial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Pvp
{
    public class PvpRankingLine : ButtonCascade
    {
        [SerializeField] Text numField;
        [SerializeField] Text textField;
        [SerializeField] Text scoreField;
        [SerializeField] GameObject isMe;

        [SerializeField] EditTeam.ClubShield clubShield;

        public void OnInit(int num, DBMundial.ResultsData data)
        {
            numField.text = '#' + num.ToString();

            scoreField.text = Utils.FormatNumbers(data.base_score, false);
            string name_abr = data.country;
            textField.text = "";
            MundialData.LevelData l = MundialData.Instance.GetCountryData(name_abr);
            if (l != null)
            {
                clubShield.Init(l.clubData);
               // countryField.text = l.name;
            }
            if (isMe != null)
            {
                if (name_abr == DB.DBManager.Instance.DbUserData.data.country)
                    isMe.SetActive(true);
                else
                    isMe.SetActive(false);
            }
        }
        public void OnInitLocal(int num, DBMundial.ResultsData data)
        {
            numField.text = '#' + num.ToString();
            scoreField.text = Utils.FormatNumbers(data.base_score, false);
         //   playedField.text = data.matches.ToString();
            if (isMe != null)
            {
                if (data.user == DB.DBManager.Instance.DbUserData.data.user)
                    isMe.SetActive(true);
                else
                    isMe.SetActive(false);
            }
        }
    }
}
