using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fulbo
{
    public class ClubsData : MonoBehaviour
    {
        [Serializable]
        public class ClubShape
        {
            public GameObject asset;
        }
        [Serializable]
        public class ClubPattern
        {
            public GameObject asset;
        }
        public Sprite[] logos;
        public ClubShape[] shapes;
        public ClubPattern[] patterns;

        public ClubData GetData(int teamID)
        {
            if (Data.Instance.mode == Data.modes.PARTYMODE)
                return Data.Instance.partyModeData.GetDataForPartyMode(teamID);

            if (teamID == 2 || teamID == 0)
            {
                ClubData clubData = Data.Instance.myTeam.clubData;
                if (clubData.name_abr == "")
                    clubData.name_abr = Data.Instance.myTeam.SetTeamName(DB.DBManager.Instance.DbUserData.data.user);
                return clubData;
            }
            else if(Data.Instance.mode == Data.modes.PVP)
            {
                return Data.Instance.pvpData.clubData;
            } else
            {
                LevelData levelData = CupsData.Instance.GetActualLevel();
                if (levelData == null)
                    return new ClubData();
                ClubData clubData = levelData.clubData;
                if (clubData == null)
                    return new ClubData();
                return clubData;
            }
        }
    }

}