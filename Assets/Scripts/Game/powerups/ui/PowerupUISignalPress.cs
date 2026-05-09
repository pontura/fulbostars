using Fulbo.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PowerupUISignalPress : MonoBehaviour
    {
        [SerializeField] Image powerupImage;

        public void OnPowerupActive(bool isOn, Character character)
        {
            this.gameObject.SetActive(isOn);
            if(isOn)
                powerupImage.sprite = Fulbo.Game.GameManager.Instance.powerupsManager.GetPowerupData(character.powerupsManager.GetPowerupType()).image;
        }
    }
}
