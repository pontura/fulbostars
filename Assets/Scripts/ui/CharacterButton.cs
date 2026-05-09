using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo;
using Fulbo.DB;
using System;


namespace Fulbo.UI
{
    public class CharacterButton : ButtonCascade
    {
        public DBUserData.DBCharacterData dbCharacterData;
        [SerializeField] Text levelField;
        [SerializeField] Text description;
        public int id;
        public Image thumb;
        [SerializeField] Transform container;
       // public CharactersData.CharacterData data;
        [HideInInspector] public CharacterCardFull fullCard;
        [SerializeField] GameObject consumables;
        [SerializeField] Text upgradableField;
        [SerializeField] GameObject upgradableIcon;
        [SerializeField] Text textField;
        [SerializeField] Text positionfield;

        [SerializeField] Sprite[] rarityCards = new Sprite[0];
        [SerializeField] Color[] rarityColorsGradient;
        [SerializeField] Image[] rarityColorImages;
        [SerializeField] Image[] rarityGradients;

        [SerializeField] GameObject[] rarityBackgrounds;
        [SerializeField] Image rarityCardBG;

        [SerializeField] Sprite[] levelRarityBGs;
        [SerializeField] Sprite[] iconRarities;
        [SerializeField] Sprite[] formationIcons;

        [SerializeField] Image levelRarityBG;
        [SerializeField] Image iconRarity;
        [SerializeField] Image formationIcon;

        [SerializeField] Sprite[] tierIcons;
        [SerializeField] Image tierIcon;

        [SerializeField] readonly bool showFullPositionName = false;

        private void OnDestroy()
        {
            Events.ToggleStats -= ToggleStats;
        }
        void ToggleStats(bool isOn)
        {
           // panelStats.SetActive(isOn);
           // panelData.SetActive(!isOn);
        }
        MyTeamUI myTeam;
        CharacterStats stats;
        public void OnInit(DBUserData.DBCharacterData dbCharacterData, bool isGoalKeeper, bool showFullPositionName = false)
        {
            myTeam = GetComponent<MyTeamUI>();
            this.dbCharacterData = dbCharacterData;
            CharactersData.CharacterData cData = CharactersData.Instance.GetCharacterData(dbCharacterData.player_id, isGoalKeeper);
            cData.uniqueID = dbCharacterData.id;
            stats = new CharacterStats();
            stats.ForceStats(dbCharacterData);

            if(upgradableIcon != null)
            {
                if (upgradableField != null && dbCharacterData.available_stats > 0)
                    upgradableField.text = dbCharacterData.available_stats.ToString();
                upgradableIcon.SetActive(dbCharacterData.available_stats > 0);
                if (dbCharacterData.available_stats > 0)
                    Data.Instance.onBoardingManager.CheckStatUpgradable();
            }

        //    OnInit(cData);
        //}
        //public void OnInit(CharactersData.CharacterData data)
        //{
            if (consumables != null)
                consumables.SetActive(false);
          //  this.data = data;

            if (description != null)
                description.text = cData.text;

            OnInitCharacterData(cData);
         

            if (positionfield != null)
                positionfield.text = Data.Instance.textsData.GetPositionName(dbCharacterData.IsGoalkeeper(), dbCharacterData.id, showFullPositionName);

            AddStats();
            SetRarity();
            if (formationIcons != null && formationIcons.Length > 0) {
                int position = dbCharacterData.position + 1 < formationIcons.Length ? dbCharacterData.position + 1 : 0;
                if (dbCharacterData.IsGoalkeeper())
                    formationIcon.sprite = formationIcons[0];
                else
                    formationIcon.sprite = formationIcons[position];
            }

            if (levelField != null)
                levelField.text = stats.GetTotal(false).ToString();

            //if (anim != null)
            //    anim.Play("on");
            if (textField != null)
            {
                textField.text = cData.text;
            }

            if (tierIcon != null && dbCharacterData.tier>0 && dbCharacterData.tier < tierIcons.Length)
                tierIcon.sprite = tierIcons[dbCharacterData.tier - 1];
        }
        public void OnInitCharacterData(CharactersData.CharacterData cData)
        {
            if(thumb)
                thumb.sprite = cData.thumb;
            TextsData.CharacterData textData = Data.Instance.textsData.GetCharactersData(cData.id, cData.isGoalkeeper);
            if (field != null && textData != null)
                field.text = cData.avatarName;
        }
        void SetRarity()
        {
            int i = (int)dbCharacterData.GetRarity()-1;

            if(levelRarityBGs != null && levelRarityBGs.Length > 0)
                levelRarityBG.sprite = levelRarityBGs[i];
            if (iconRarities != null && iconRarities.Length > 0)
                iconRarity.sprite = iconRarities[i];

            if (rarityBackgrounds != null && rarityBackgrounds.Length > 0)
            {
                foreach (GameObject go in rarityBackgrounds)
                    go.SetActive(false);

                rarityBackgrounds[i].SetActive(true);
            }

            if (rarityCards == null || rarityCards.Length <= 0) return;

            rarityCardBG.sprite = rarityCards[i];
            foreach (Image rarityColorBG in rarityColorImages)
                rarityColorBG.color = Data.Instance.settings.GetRaritySettingFor(i).color;
            //Estos números hardcodeados abajo son la diferencia de valores para la gradiente más oscura de los colores originales
            foreach (Image rarityGradient in rarityGradients)
                rarityGradient.color = rarityColorsGradient[i];

            
            
        }

        void AddStats()
        {
            if (container == null) return;
            int id = 0;
            foreach  (CharacterStatLine newStatLine in container.GetComponentsInChildren< CharacterStatLine>())
            {
                string name = stats.GetStatName(id);
                int value = stats.GetStatByName(id);
                int upgrades = 0;   

                newStatLine.Init(name, value, upgrades, id, Data.Instance.settings.statsSettings[id]);
                newStatLine.gameObject.SetActive(true);
                newStatLine.SetUpgradeButton(id, dbCharacterData, OnUpgrade);
                id++;
            }
        }
        public void SetConsumables(int total)
        {
            if (consumables != null && total > 0)
                consumables.SetActive(true);
        }
        void OnUpgrade(int id) {
            if (dbCharacterData.available_stats <= 0) {
                UpdateUpgradeData(dbCharacterData);
                myTeam.OnSelectCharacter(dbCharacterData);
                return;
            }

            string stat = ((Settings.stat)id).ToString();
            var statInfo = dbCharacterData.GetType().GetField(stat);
            if (statInfo != null) {
                int val = (int)statInfo.GetValue(dbCharacterData);
                statInfo.SetValue(dbCharacterData, val + 1);
            }

            DBEvents.UpgradeStat(dbCharacterData.id, (Settings.stat)id, OnStatSaved);

            dbCharacterData.upgraded_stats++;
            dbCharacterData.total_stats++;
            dbCharacterData.available_stats--;
            if (dbCharacterData.available_stats <= 0)
                foreach (CharacterStatLine statLine in container.GetComponentsInChildren<CharacterStatLine>())
                    statLine.UpdateUpgradableState(false);

            if (myTeam == null)
                myTeam = GetComponent<MyTeamUI>();

            UpdateUpgradeData(dbCharacterData);

            //Analytics
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["role"] = dbCharacterData.role;
            param["rarity"] = dbCharacterData.rarity;
            param["characterName"] = dbCharacterData.AvatarName(); //Más intuitivo de leer que pasar el número del ID
            param["stat"] = ((Settings.stat)id).ToString();

            Events.OnTrack("LevelUp", param);

            //Events.OnLoadingPanel(true);

            myTeam.OnUpgradeFX();
            myTeam.OnSelectCharacter(dbCharacterData);
        }

        void UpdateUpgradeData(DBUserData.DBCharacterData chData) {
            CharacterButton cb = myTeam.cards.Find(x => x.dbCharacterData.id == chData.id);
            cb.upgradableField.text = "" + chData.available_stats;
            cb.upgradableIcon.SetActive(chData.available_stats > 0);
            cb.levelField.text = "" + chData.total_stats;
        }

        void OnStatSaved(bool isOk, string result)
        {
            if (isOk)
            {
                print("On Stat Saved: User Data Reload");
                DB.DBManager.Instance.DbUserData.LoadUserData(null);
            }
            else
            {
                Events.OnPopup(result, null);
            }
        }
        void OnUserLoaded()
        {
            if (myTeam == null)
                myTeam = GetComponent<MyTeamUI>();
            //Events.OnLoadingPanel(false);
            print("On User Loaded");
            
            if(myTeam != null && dbCharacterData!=null)
            {
                int id_ = dbCharacterData.id;
                dbCharacterData = DBManager.Instance.DbUserData.data.GetPlayerByID(dbCharacterData.id);
                if(dbCharacterData!=null)
                    myTeam.UpdatePrice(dbCharacterData);
                else Debug.LogError("DbCharacterData null on GetPlayerByID for id: "+id_);

            } else Debug.LogError("myTeam or dbCharacterData are null");
        }
    }
}