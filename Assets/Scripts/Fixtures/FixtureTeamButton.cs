using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Fulbo.UI;
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
        public string teamName;

        public void Init(LevelData levelData)
        {
            anim = GetComponent<Animator>();
            chubShield.Init(levelData.clubData);
            field.text = levelData.name;
            teamName = levelData.name;;
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