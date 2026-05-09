using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CardAsset : MonoBehaviour
    {
        [SerializeField] Image background;
        [SerializeField] Sprite[] spritesRarity;
        [SerializeField] Text levelField;
        [SerializeField] Text positionField;
        [SerializeField] Text field;
        [SerializeField] Image thumb;

        [SerializeField] GameObject position;
        [SerializeField] GameObject nameGO;
        [SerializeField] GameObject levelGO;
        [SerializeField] Image nameBG;
        [SerializeField] RawImage rawImage;
        CharactersData.CharacterData cData;
        [SerializeField] public CharacterForCamera characterForCamera;

        public void Init(CharactersData.CharacterData cData, int totalStats =-1)
        {
            isAnimated = false;
            this.cData = cData;
            if (totalStats == -1)
                levelGO.SetActive(false);
            else
                levelField.text = totalStats.ToString();

            int RANDOM = Random.Range(0,3);
            background.sprite = spritesRarity[(int)(cData.rarity-1)];

            if(nameBG != null)
                nameBG.color = Data.Instance.settings.GetRaritySettingFor((cData.rarity-1)).color;

            positionField.text = Data.Instance.textsData.GetPositionName(cData, true);
            field.text = cData.avatarName.ToUpper();

            thumb.sprite = cData.thumb;
            if (rawImage != null)
                SetRawImage(false);
        }

        void SetRawImage(bool isOn)
        {
            thumb.enabled = !isOn;
            if (isOn) {
                characterForCamera.Init(cData);
            } else
                characterForCamera.DestroyCharacter();

            rawImage.enabled = isOn;
        }

        public void OnClose() {
            SetRawImage(false);
        }

        public void HidePanels(bool hidePos, bool hideName, bool hideLevel)
        {
            position.SetActive(hidePos);
            nameGO.SetActive(hideName);
            levelGO.SetActive(hideLevel);
        }
        bool isAnimated;
        public void ToggleImage()
        {
            if (!isAnimated)
                Events.OnButtonPressed(ButtonCustom.types.UI_GENERIC);
            else
                Events.OnButtonPressed(ButtonCustom.types.KEY_PRESSED);

            isAnimated = !isAnimated;
            SetRawImage(isAnimated);
        }
    }
}
