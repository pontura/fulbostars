using Fulbo.UI.EditTeam;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class FixtureTeam : MonoBehaviour
    {
        [SerializeField] Color automataColor;
        [SerializeField] Color userColor;
        [SerializeField] Text field;
        [SerializeField] Image bg;
        [SerializeField] ClubShield chubShield;
        [SerializeField] GameObject done;
        [SerializeField] Text goals;
        public bool isOn;
        public LevelData levelData;
        public enum states
        {
            on,
            off,
            win,
            lose,
            playing
        }
        Animator anim;
        public void Init(LevelData levelData)
        {
            done.SetActive(false);
            this.levelData = levelData;
            anim = GetComponent<Animator>();
            chubShield.Init(levelData.clubData);
            field.text = levelData.name;

            if(levelData.controlledInFixtures)
                field.color = userColor;
            else
                field.color = automataColor;
        }
        public void SetState(states state)
        {
            print("SetState " + state);
            if(anim == null)
                anim = GetComponent<Animator>();
            switch(state)
            {
                case states.on:  anim.SetInteger("value", 1); break;
                case states.off:  anim.SetInteger("value", 2); done.SetActive(false);break;
                case states.win:  anim.SetInteger("value", 3); break;
                case states.lose:  anim.SetInteger("value", 4); break;
                case states.playing:  anim.SetInteger("value", 5); break;
            }
        }
         public void SetScore(int score)
        {
            field.text = levelData.clubData.name_abr;
            goals.text = score.ToString();
            done.SetActive(true);
        }
    }
}