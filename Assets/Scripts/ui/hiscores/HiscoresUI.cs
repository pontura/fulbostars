using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Hiscores
{
    public class HiscoresUI : MonoBehaviour
    {
        [SerializeField] HiscoresUILine hiscoresUILine;
        [SerializeField] Transform container;
        [SerializeField] Text titleField;

        public void OnEnable()
        {
            //Utils.RemoveAllChildsIn(container);
            //titleField.text = Data.Instance.texts.Get("hiscores");
            //int stadiumID = Data.Instance.matchData.levelData.stadium_id;
            //int levelID = Data.Instance.matchData.levelData.id;
            //DB.DBEvents.LoadMatchesPerLevel(stadiumID, levelID, OnLoaded);
        }
        void OnLoaded()
        {
            foreach(DB.DBMatches.MatchData data in DB.DBManager.Instance.DbMatchesPerLevel.data.results)
            {
                HiscoresUILine line = Instantiate(hiscoresUILine, container);
                line.Init(data);
            }
        }
        public void ViewAll()
        {
            int levelID = Data.Instance.matchData.levelData.id;
            Application.OpenURL(DBManager.Instance.ranking_url + CupsData.Instance.GetActualLevel().stadium_id + "/" + levelID);
        }
    }
}
