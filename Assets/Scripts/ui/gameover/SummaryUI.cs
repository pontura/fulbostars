using Fulbo.DB;
using Fulbo.Mundial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class SummaryUI : MonoBehaviour
    {
        public List<SummaryItem> all;
        [SerializeField] SummaryItem item_to_add;
        [SerializeField] Transform container;
        [SerializeField] ScoreUI scoreUI;
        [SerializeField] ScoreUI scoreByResult;
        [SerializeField] ScoreUI bonusScoreUI;
        [SerializeField] GameObject top;
        [SerializeField] Animation anim;
        [SerializeField] Text resultField;

        [SerializeField] SummaryWidgetUI widgetCoinsGrabbed;
        [SerializeField] SummaryWidgetUI widgetGoals;
        [SerializeField] SummaryWidgetUI widgetShards;
        //[SerializeField] SummaryWidgetUI widgetMundial;
        //   [SerializeField] EditTeam.ClubShield mundialClubShield;

        //public int totalScore;
       // public int firstWinReward = 0;

        float delay = 0.2f;
        float delay2 = 0.4f;
        public bool winACard;
        Vector2 initialScale;

        System.Action OnReady;

        MatchRewards rewards;

        UIGameOverMenu.states state;

        public void Init(System.Action OnReady, UIGameOverMenu.states _state)
        {
            this.state = _state;
            scoreUI.ForceScore(0, "");
            scoreUI.EmptyField();

            this.OnReady = OnReady;
            widgetGoals.SetOn(false);
            widgetCoinsGrabbed.SetOn(false);
            widgetShards.SetOn(false);

            initialScale = gameObject.transform.localScale;
            gameObject.transform.localScale = Vector2.zero;
            scoreByResult.gameObject.SetActive(false);
            bonusScoreUI.gameObject.SetActive(false);
            Utils.RemoveAllChildsIn(container);

            rewards = Data.Instance.matchData.Rewards();

            top.gameObject.SetActive(false);

            if (rewards.goalDiff < 0)
                resultField.text = Data.Instance.texts.Get("you_lose");
            else if (rewards.goalDiff == 0)
                resultField.text = Data.Instance.texts.Get("you_draw");
            else
                resultField.text = Data.Instance.texts.Get("you_win");
            

            if (Data.Instance.mode != Data.modes.PARTYMODE)
                top.SetActive(true);
            else
                top.SetActive(false);

            Add().Init("ball_possesion",  (int)rewards.team2.ball_possesion,           (int)rewards.team1.ball_possesion, SummaryItem.types.PERCENT, rewards.scoresToAdd[0]);
            Add().Init("kicks_passes", rewards.team2.kicks_passes,                      rewards.team1.kicks_passes,        SummaryItem.types.NUM, rewards.scoresToAdd[1]);
            Add().Init("kicks_to_goal", rewards.team2.kicks_to_goal,                    rewards.team1.kicks_to_goal,       SummaryItem.types.NUM, rewards.scoresToAdd[2]);
            Add().Init("balls_to_referi", rewards.team2.balls_to_referi,                rewards.team1.balls_to_referi,     SummaryItem.types.NUM, rewards.scoresToAdd[3]);
            Add().Init("centros", rewards.team2.centros,                                rewards.team1.centros,             SummaryItem.types.NUM, rewards.scoresToAdd[4]);
            Add().Init("tackles", rewards.team2.tackles,                                rewards.team1.tackles,             SummaryItem.types.NUM, rewards.scoresToAdd[5]);
            Add().Init("saves", rewards.team2.saves,                                    rewards.team1.saves,               SummaryItem.types.NUM, rewards.scoresToAdd[6]);

            int id = 0;
            foreach (SummaryItem s in all)
            {
                id++;
                if (id % 2 == 0)
                    s.SetBg();
            }


            StartCoroutine(Appear());
        }
        int suma;
        IEnumerator Appear()
        {
            if (Data.Instance.mode != Data.modes.PARTYMODE)
                top.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            anim.Play("on");
            gameObject.transform.localScale = initialScale;
            foreach (SummaryItem s in all)
            {
                AudioManager.Instance.PlaySound("common", "ui/estadistica", false);
                s.gameObject.SetActive(true);

                yield return new WaitForSeconds(delay);
            }
            if (Data.Instance.mode == Data.modes.PARTYMODE || Data.Instance.energySystem.IsAFreeGame())
            {
                if (OnReady != null) OnReady();
            } else  {
                suma = rewards.score - rewards.bonusTotal;
                AddScore();
                scoreByResult.gameObject.SetActive(true);
                scoreByResult.EmptyField();
                scoreByResult.ForceScore(rewards.score - rewards.bonusTotal, "scoreForsummary");
                AudioManager.Instance.PlaySound("common", "ui/coin_reward", false);

                yield return new WaitForSeconds(delay2);
                foreach (SummaryItem s in all)
                {                    
                    if (s.winScore > 0)
                    {
                        suma += (int)Mathf.Round(s.winScore);
                        AddScore();
                        s.SetWin();
                        yield return new WaitForSeconds(delay2);
                    }
                }
                if (rewards.coinsReward > 0)
                {
                    widgetCoinsGrabbed.Init(Data.Instance.texts.Get("coins_grabbed"), rewards.coinsReward);
                    suma += rewards.coinsReward;
                    AddScore();
                    yield return new WaitForSeconds(delay2);
                }
                if (rewards.goalDiffValue > 0)
                {
                    widgetGoals.Init(Data.Instance.texts.Get("goals_difference"), rewards.goalDiffValue);
                    suma += rewards.goalDiffValue;
                    AddScore();
                    yield return new WaitForSeconds(delay2);
                }
                if (state == UIGameOverMenu.states.WIN_CUP)
                {
                    int shardsWon = Data.Instance.matchData.response.shardsWon;
                    widgetShards.Init(Data.Instance.texts.Get("shards_summary"), shardsWon);

                    //onboarding: // solo primera vez que se gana la primer copa
                    DBGameData.Content gameData = DBManager.Instance.DbUserData.data.gameData;
                    int timesWon = gameData.cups.GetTimesWon(10, 1); // primer cup

                    print("gameData.cups.cupsPlayed.Count "  + gameData.cups.cupsPlayed.Count + " timesWon" + timesWon);

                    if (Data.Instance.matchData.levelData.cupID == 10 &&
                        Data.Instance.matchData.levelData.tier == 1 && timesWon == 1) 
                        Data.Instance.onBoardingManager.OpenShards();
                    //

                    yield return new WaitForSeconds(delay2);
                }
                if (rewards.bonusTotal > 0) {
                    suma += (int)Mathf.Round(rewards.bonusTotal);
                    AddScore();
                    bonusScoreUI.gameObject.SetActive(true);
                    bonusScoreUI.EmptyField();
                    bonusScoreUI.ForceScore(rewards.bonusTotal, "scoreForsummary");
                }
            }
            OnReady();
        }
        int lastScoreAdded = 0;
        void AddScore()
        {
            scoreUI.ForceScore(suma, "scoreForsummary");
            Events.AddScore(suma - lastScoreAdded);
           // print("_____________________AddScore: " + suma + "             lastScoreAdded:" + lastScoreAdded);
            lastScoreAdded = suma; 
            AudioManager.Instance.PlaySound("common", "ui/coin_reward", false);
        }
        SummaryItem Add()
        {
            SummaryItem s = Instantiate(item_to_add, container);
            s.transform.localScale = Vector2.one;
            all.Add(s);
            s.gameObject.SetActive(false);
            return s;
        }
        public void Reset()
        {
            StopAllCoroutines();
        }

        //void CheckMundial()
        //{
        //    if (Data.Instance.mode != Data.modes.PARTYMODE && !Data.Instance.energySystem.IsAFreeGame())
        //    {
        //        string country = DB.DBManager.Instance.DbUserData.data.country;
        //        if (country != "")
        //        {
        //            widgetMundial.SetOn(true);

        //            //MundialData.LevelData levelData = MundialData.Instance.GetCountryData(country);
        //            //mundialClubShield.Init(levelData.clubData);

        //          //  widgetMundial.InitStrings(Data.Instance.texts.Get("score_sended"), levelData.name);
        //        }
        //    }
        //}
    }
}
