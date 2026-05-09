using Fulbo;
using Fulbo.UI.EditTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CupLevelButton : ButtonCustom
    {
        [SerializeField] GameObject arrow;
        [SerializeField] TeamPoster teamPoster;
        [SerializeField] Text numField;
        [SerializeField] Text resultField;
        [SerializeField] Text scoreField;
        [SerializeField] Image stadiumBG;
        [SerializeField] GameObject chest;
        [SerializeField] Transform chestContainer;
        [SerializeField] ClubShield chubShield;

        public LevelData levelData;
        public states state;
        public enum states
        {
            ACTIVE,
            PLAYED,
            BLOCKED,
            LOST,
            DRAW
        }
        public typesButton type;
        public enum typesButton
        {
            FIRST,
            LAST,
            COMMON
        }
        public void Init(LevelData levelData, int num, typesButton type, bool won)
        {
            chubShield.Init(levelData.clubData);
            numField.text = num.ToString() + "\u00B0";
            this.levelData = levelData;
            field.text = levelData.name;
            teamPoster.AddData(levelData);
            int id = 0;
            stadiumBG.sprite = Stadiums.StadiumsData.Instance.GetStadium(levelData.stadium_id).thumbBG;
            chest.SetActive(false);

            if (type == typesButton.FIRST)
                arrow.SetActive(false);
            else if (type == typesButton.LAST)
            { 
                chest.SetActive(true);
                if (won)
                {
                    Animator animChest = chest.GetComponent<Animator>();
                    animChest.SetBool("done", true);
                    animChest.Play("finalLevelDone", 0,0);
                }
                else
                {
                    int chestID = CupsData.Instance.GetCupData(levelData.cupID, levelData.tier).chest;
                    ChestsData.ChestData d = ChestsData.Instance.GetChest(chestID);
                    GameObject go = d.GetAsset();
                    GameObject asset = null;

                    if (go != null)
                        asset = Instantiate(go, chestContainer);
                    asset.transform.localScale = Vector2.one;
                    asset.transform.localPosition = Vector2.zero;
                }
            } 
        }
        public void SetResults(int myScore, int opponentScore)
        {
            scoreField.text = myScore + "-" + opponentScore;
            if(myScore>opponentScore)
                resultField.text = Data.Instance.texts.Get("you_win");
            else if(myScore < opponentScore)
                resultField.text = Data.Instance.texts.Get("you_lose");
            else
            {
                anim.SetBool("draw", true);
                resultField.text = Data.Instance.texts.Get("you_draw");
            }
        }
        private void OnEnable()
        {
            SetState(state);
        }
        public void SetState(states state)
        {
            this.state = state;
          //  print("________________________SetState " + state);
            switch(state)
            {
                case states.ACTIVE:
                    SetInteraction(true);
                    anim.Play("Normal", 0, 0);
                    anim.SetBool("blocked", false);
                    anim.SetBool("lost", false);
                    anim.SetBool("played", false);
                    break;
                case states.DRAW:
                    SetInteraction(true);
                    anim.Play("Draw", 0, 0);
                    anim.SetBool("draw", true);
                    break;
                case states.LOST:
                    SetInteraction(true);
                    anim.Play("Lost", 0, 0);
                    anim.SetBool("lost", true);
                    break;
                case states.BLOCKED:
                    SetInteraction(false);
                    anim.Play("Disabled", 0, 0);
                    anim.SetBool("blocked", true);
                    break;
                case states.PLAYED:
                    SetInteraction(false);
                    anim.Play("Played", 0, 0);
                    anim.SetBool("played", true);
                    break;
            }
        }
    }
}
