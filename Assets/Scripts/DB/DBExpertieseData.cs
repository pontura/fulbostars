using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Fulbo.DB
{
    [Serializable]
    public class DBExpertieseData
    {
        public class ExpertieseData
        {
            public int playerID;
            public int value;
        }
        public List<ExpertieseData> all;

        ExpertieseData GetDataForPlayer(int playerID)
        {
            foreach (ExpertieseData data in all)
                if (data.playerID == playerID)
                    return data;

            ExpertieseData d = new ExpertieseData();
            d.playerID = playerID;
            all.Add(d);
            return d;
        }
        public void AddExpertiese(List<int> playersID, int value)
        {
            foreach(int playerID in playersID)
            {
                ExpertieseData data = GetDataForPlayer(playerID);
                data.value += value;
            }
        }
    }

}
