using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class TutorialStepButtonUI : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] Text field;
        [SerializeField] GameObject locked;
        bool isLocked;
        int id;
        TutorialStepsUIScreen screens;

        public void Init(TutorialStepsUIScreen screens, Game.Tutorial.TutorialData.StepData sd, int id)
        {
            this.id = id;
            image.sprite = sd.image;
            string tip = "tutorial_" + id + "_title";
            field.text = (id+1) + " - " + Data.Instance.texts.Get(tip);
            this.screens = screens;
            int lockedLevel = Data.Instance.myTeam.GetTutorial();

            if (id > lockedLevel)
                Locked(true);
            else
                Locked(false);
        }
        void Locked(bool idLocked)
        {
            locked.SetActive(idLocked);
            isLocked = idLocked;
        }
        public void OnClick()
        {
            if (isLocked) return;
            screens.OnClick(id);
        }
    }
}
