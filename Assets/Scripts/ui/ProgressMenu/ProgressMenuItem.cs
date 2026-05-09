using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class ProgressMenuItem : MonoBehaviour
    {
        Image image;
        public ProgressMenu.ItemData.states state;

        public void Init(ProgressMenu.ItemData data, ProgressMenu.ItemSettings settings)
        {
            image = GetComponentInChildren<Image>();
            SetState(data.state, settings);
        }
        public void SetState(ProgressMenu.ItemData.states state, ProgressMenu.ItemSettings settings)
        {
            this.state = state;
            switch (state)
            {
                case ProgressMenu.ItemData.states.ACTIVE:
                    GetComponent<Animation>().Play("tutorialStepItemOff");
                    image.color = settings.color_active;
                    break;
                case ProgressMenu.ItemData.states.INACTIVE:
                    GetComponent<Animation>().Play("tutorialStepItemOff");
                    image.color = settings.color_inactive;
                    break;
                case ProgressMenu.ItemData.states.ON:
                    GetComponent<Animation>().Play("tutorialStepItem");
                    image.color = settings.color_on;
                    break;
            }
        }
    }
}
