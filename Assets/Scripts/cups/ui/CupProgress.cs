using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CupProgress : MonoBehaviour
    {
        [SerializeField] Transform cupContainer;
        [SerializeField] GameObject cupPlayingPoster;
        [SerializeField] Text title;
        [SerializeField] CupDetails cupDetails;

        public void Init()
        {
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;            
            if (cups.activeCup == 0)
            {
                SetNoCupSelected();
            } else
            SetCupData();
        }
        
        void SetNoCupSelected() {
            if(cupPlayingPoster != null) cupPlayingPoster.SetActive(false);
            if (title != null)  title.text = "";
            if (cupDetails != null)  cupDetails.gameObject.SetActive(false);
        }

        public void SetCupData()
        {
            if (cupPlayingPoster != null) cupPlayingPoster.SetActive(true);
            LevelData ld = Data.Instance.matchData.levelData;
            int cupID = ld.cupID;
            int tier = ld.tier;

            if (cupID == 0) {
                DB.DBCupsData.DBCupData cupData = DB.DBManager.Instance.DbUserData.data.gameData.cups.GetActiveCupData();
                if (cupData != null) { 
                    cupID = cupData.cupID;
                    tier = cupData.tier;
                } else {
                    SetNoCupSelected();
                    return;
                }
            }

            CupsData.CupData c = CupsData.Instance.GetCup(cupID);
            GameObject go = c.GetAssetCup();

            Utils.RemoveAllChildsIn(cupContainer);

            if (go != null)
            {
                GameObject cupNew = Instantiate(go, cupContainer);
                cupNew.transform.localPosition = Vector3.zero;
                cupNew.transform.localScale = Vector3.one;
                CupsData.Instance.AddTier(cupNew, tier);
            }

            if (cupDetails != null) {
                cupDetails.gameObject.SetActive(true);
                cupDetails.Init(cupID, tier, false);
            }
        }
    }
}
