using Fulbo.DB;
using Fulbo.Mundial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Mundial
{
    public class CountrySelection : CascadeList
    {
        [SerializeField] CountryButton button;
        [SerializeField] Transform container;
        [SerializeField] MundialOnboardingUI ui;

        void Start()
        {
            Utils.RemoveAllChildsIn(container);
            int id = 0;
            InitCascade();
            foreach (MundialData.LevelData levelData in MundialData.Instance.content)
            {
                CountryButton b = Instantiate(button, container);
                b.Init(id, OnClick);
                b.OnInit(levelData);
                id++;
                AddToCascade(b);
            }
            StartCascade();
        }
        void OnClick(int id)
        {
            MundialData.LevelData levelData = MundialData.Instance.content[id];
            DB.DBManager.Instance.DbUserData.data.SetCountry(levelData.clubData.name_abr);
            DBEvents.SaveUserData(DB.DBManager.Instance.DbUserData.data, null);
            ui.OnClubSelected(levelData);
        }
    }
}
