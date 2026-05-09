using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.UI
{
    public class TutorialStepsUIScreen : MonoBehaviour
    {
        [SerializeField] TutorialStepButtonUI button;
        [SerializeField] Transform container;
        TutorialProgressMenu tutorialProgressMenu;

        public void Init(TutorialProgressMenu tutorialProgressMenu)
        {
            this.tutorialProgressMenu = tutorialProgressMenu;
            Utils.RemoveAllChildsIn(container);
            int id = 0;
            foreach(Game.Tutorial.TutorialData.StepData sd in Game.Tutorial.TutorialData.Instance.all)
            {
                TutorialStepButtonUI b = Instantiate(button, container);
                b.Init(this, sd, id);
                id++;
            }
        }
        public void OnClick(int id)
        {
            tutorialProgressMenu.Open(id);
        }

    }
}
