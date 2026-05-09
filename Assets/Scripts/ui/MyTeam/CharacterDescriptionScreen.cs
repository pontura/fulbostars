using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.DB.DBUserData;

namespace Fulbo.UI
{
    public class CharacterDescriptionScreen : MonoBehaviour
    {
        [SerializeField] Text nameField;
        [SerializeField] Text desc;
        [SerializeField] Text rarityField;
        [SerializeField] Image rarityIcon;
        [SerializeField] MyTeamUI myTeamUI;

        private void OnEnable()
        {
            RefreshData(myTeamUI.dbCharacterData);
        }
        private void Start()
        {
            Events.RefreshData += RefreshData;
        }
        private void OnDestroy()
        {
            Events.RefreshData -= RefreshData;
        }
       
        public void RefreshData(DBUserData.DBCharacterData uData)
        {
            if (uData == null || uData.role == "") return;
            int rarity =  (int)uData.GetRarity()-1;
            Settings.RaritySetting rarityData = Data.Instance.settings.raritySetting[rarity];
            rarityIcon.sprite = rarityData.icon;
            rarityField.text = rarityData.rarity.ToString();
            CharactersData.CharacterData charData = CharactersData.Instance.GetCharacterData(uData.player_id, uData.IsGoalkeeper());
            TextsData.CharacterData textData = Data.Instance.textsData.GetCharactersData(uData.player_id, uData.IsGoalkeeper());

            nameField.text = charData.avatarName;
            desc.text = charData.text;
        }
    }
}