using Fulbo.DB;
using Fulbo.Mundial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Mundial
{
    public class MundialRankingLocal : CascadeList
    {
        [SerializeField] Transform containter;
        [SerializeField] MundialRankingButton button;
        [SerializeField] EditTeam.ClubShield clubShield;
        [SerializeField] Text countryField;
        [SerializeField] Text myField;
        Animation anim;
        string country;
        MundialData.LevelData levelData;

        public void Init()
        {
            Utils.RemoveAllChildsIn(containter);
            LoadContent();
        }
        void LoadContent()
        {
            country = DB.DBManager.Instance.DbUserData.data.country;
            levelData = MundialData.Instance.GetCountryData(country);
            clubShield.Init(levelData.clubData);
            countryField.text = Data.Instance.texts.Get("ranking") + ": " +  levelData.name;
            MundialData.Instance.LoadRankingLocal(country, OnReady);
            DB.DBManager.Instance.DbMundial.LoadMyScore(OnReadyMyScore);
            
        }
        void OnReadyMyScore(DBMundial.ResultsData result)
        {
            myField.text =
                //  Data.Instance.texts.Get("your_score") + ": " +
                result.base_score.ToString();// + " | " +
               // Data.Instance.texts.Get("matches") + ": " + 
               // result.matches;
        }
        void OnReady(DBMundial.DataFromServer ranking)
        {
            InitCascade();
            Utils.RemoveAllChildsIn(containter);
            int id = 0;
            foreach (DBMundial.ResultsData data in ranking.results)
            {
                MundialRankingButton b = Instantiate(button, containter);
                b.Init(id, OnClicked);
                id++;
                b.OnInitLocal(id, data);
                AddToCascade(b);
            }
            StartCascade();
        }
        void OnClicked(int id)
        {

        }
    }
}
