using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Onboarding;

namespace Fulbo.UI
{
    public class CupsLifes : MonoBehaviour
    {
        [SerializeField] Text field;
        [SerializeField] Animator anim;
        float actualLife;
        int totalLife = 3;

        void Awake()
        {
            Events.ResetLifesTo += ResetLifesTo;
            Events.InitCup += InitCup;
            Events.EndCup += EndCup;
        }
        void OnDestroy()
        {
            Events.ResetLifesTo -= ResetLifesTo;
            Events.InitCup -= InitCup;
            Events.EndCup -= EndCup;
        }
        public void Init()
        {
            SetValues();
        }
        void InitCup(int cupID, int tierID)
        {
            CupsData.CupData c = CupsData.Instance.GetCupData(cupID, tierID);
            totalLife = c.life;
            gameObject.SetActive(true);
            ResetLifesTo(totalLife);
            SetValues();
        }
        void EndCup()
        {
            gameObject.SetActive(false);
        }
        void SetValues()
        {
            DB.DBCupsData cups = DB.DBManager.Instance.DbUserData.data.gameData.cups;

            if (cups == null)
                DB.DBManager.Instance.DbUserData.data.gameData.cups = new DB.DBCupsData();

            totalLife = (int)DB.DBManager.Instance.DbUserData.data.gameData.cups.totalLifes;
            float lifesBurned = DB.DBManager.Instance.DbUserData.data.gameData.cups.lifesBurned;
            actualLife = totalLife - lifesBurned;
            SetField();
        }
        void SetField()
        {
            field.text = actualLife.ToString();
        }
        public void CheckLifeLose()
        {
            // if tutorial dont lose life
            if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.SECOND_MATCH_PLAYED)) return;
            MatchData matchData = Data.Instance.matchData;
            if (matchData.score.y <= matchData.score.x) {
                actualLife--;
                Debug.Log("HasLifeLose: " + DB.DBManager.Instance.DbUserData.data.gameData.cups.hasLoseLife);
                if (!Data.Instance.onBoardingManager.FirstLifeLose((x) => { transform.SetAsFirstSibling(); PlayLifeLose(); }) )
                    PlayLifeLose();
            }            
        }

        void PlayLifeLose() {
            if (!Data.Instance.matchData.dataOnInit.hasWatchVideoForLife)
            {
                anim.Play("scoreDown", 0, 0);
                AudioManager.Instance.PlaySoundOneShot("ui", "ui/game_life_lose");
            }
            SetField();
        }

        void ResetLifesTo(float to)
        {
            actualLife = to;
            anim.Play("scoreUp", 0, 0);
            SetField();
        }

        public void SetActive(bool enable) {
            gameObject.SetActive(enable);
        }
    }
}
