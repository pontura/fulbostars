using Fulbo.UI.EditTeam;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class FixtureTeamButton : MonoBehaviour
    {
        Animator anim;
        [SerializeField] Text field;
        [SerializeField] TeamPoster teamPoster;
        [SerializeField] ClubShield chubShield;
        public bool isOn;
        public GameObject selectedGO;
        public LevelData levelData;

        public void Init(LevelData levelData)
        {
            this.levelData = levelData;
            anim = GetComponent<Animator>();
            chubShield.Init(levelData.clubData);
            field.text = levelData.name;
            teamPoster.AddData(levelData);
        }
        public void OnClicked()
        {
            isOn = !isOn;
            SetOn(isOn);
        }
        public void SetOn(bool isOn)
        {
            this.isOn = isOn;     
            selectedGO.SetActive(isOn);
            
            if(isOn)
                anim.Play("Disabled");
            else
                anim.Play("Normal");
        }
        public void RollOver()
        {
            anim.Play("Selected");
        }
        public void RollOut()
        {
          if(isOn)
                anim.Play("Disabled");
            else
                anim.Play("Normal");
        }
    }
}