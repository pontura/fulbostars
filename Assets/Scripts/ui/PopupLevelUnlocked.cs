using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PopupLevelUnlocked : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Text titleField;
        [SerializeField] Text textField;
        [SerializeField] TeamPoster teamPoster;
        [SerializeField] GameObject playerWarning;
        LevelData levelData;
        System.Action<LevelData> OnShortcutToLevelClicked;

        void Start()
        {
            Close();
            Events.OpenUnlockLevelPopup += OpenUnlockLevelPopup;
        }
        private void OnDestroy()
        {
            Events.OpenUnlockLevelPopup -= OpenUnlockLevelPopup;
        }
        void OpenUnlockLevelPopup(LevelData levelData, System.Action<LevelData> OnShortcutToLevelClicked)
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_congratulations");            

            this.OnShortcutToLevelClicked = OnShortcutToLevelClicked;
            this.levelData = levelData;
            titleField.text = Data.Instance.texts.Get("congratulations");
            textField.text = Data.Instance.texts.Get("level_unlocked");
            panel.SetActive(true);
            teamPoster.AddData(levelData);

            if (levelData.oponents.Count > Data.Instance.myTeam.GetCharacterIds(false).Count + 1)
            {
                playerWarning.SetActive(true);
                playerWarning.GetComponentInChildren<Text>().text = Data.Instance.texts.Get("levelpopup_warning_players");
            }
            else
            {
                playerWarning.SetActive(false);
            }
        }
        public void LevelClicked()
        {
            OnShortcutToLevelClicked(levelData);
            OnShortcutToLevelClicked = null;
            levelData = null;
            Close();
        }
        public void Close()
        {
            levelData = null;
            panel.SetActive(false);
        }
    }
}
