using Fulbo.Mundial;
using Fulbo.UI.EditTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Mundial
{
    public class CountryButton : ButtonCascade
    {
        [SerializeField] ClubShield clubShield;

        public void OnInit(Fulbo.Mundial.MundialData.LevelData levelData)
        {
            field.text = levelData.name;
            clubShield.Init(levelData.clubData);
        }
    }
}
