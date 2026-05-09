using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.Marketplace
{
    public class NewCharacterUI : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] CharacterButton characterButton;
        [SerializeField] ButtonCustom closeButton;
        [SerializeField] GameObject panel;
        [SerializeField] CharacterForCamera characterForCamera;
        int player_id;
        bool isGoalkeeper;
        private void Start()
        {
            Close(0);
        }
        public void Init(DB.DBUserData.DBCharacterData uData)
        {
            Events.OnLoadingPanel(false);
            isGoalkeeper = uData.IsGoalkeeper();
            player_id = uData.player_id;
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_congratulations");
            title.text = Data.Instance.texts.Get("new_player");
            closeButton.Init(0, Close);
            panel.SetActive(true);
            characterButton.OnInit(uData, uData.IsGoalkeeper());

            CharactersData.CharacterData cData = CharactersData.Instance.GetCharacterData(uData.player_id, uData.IsGoalkeeper());
            characterForCamera.Init(cData.asset, "run");
            characterForCamera.SetCamera(true);
            Invoke("SayDelayed", 1f);
        }
        void SayDelayed()
        {
            Events.SayCharacterName(player_id, isGoalkeeper);
        }
        public void Close(int id)
        {
            panel.SetActive(false);
        }
    }
}
