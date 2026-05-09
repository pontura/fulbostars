using Fulbo.DB;
using Fulbo.Game;
using Fulbo.Stadiums;
using Fulbo.UI.EditTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Fulbo.UI
{
    public class MyTeamUI : CascadeList
    {
        public Transform replaceContainer;
        public CharacterButton characterButton;
        public DBUserData.DBCharacterData dbCharacterData;
        public List<CharacterButton> cards;
        [SerializeField] CharacterCardFull characterCardFull;
        [SerializeField] MyTeamCharacterData myTeamCharacterData;
        [SerializeField] TabsManager tabs;
        [SerializeField] GameObject levelUpFX;
        [SerializeField] CharacterValue characterValue;
        [SerializeField] GameObject faq;

        Animation anim;
        int lastIdClicked;
        bool forceCharacter;
        public void Init()
        {
            Data.Instance.onBoardingManager.CheckMyTeamVisited();
            levelUpFX.SetActive(false);
            anim = GetComponent<Animation>();
            anim.Play("in");
            Events.OnSkipOff();
            Events.CheckTip(Game.Tutorial.TipsManager.Types.CHARACTERS_VIEW_STATS, null);
            LoadCharacters();
            tabs.Init();
            Invoke("BackDelayed", 0.75f);

            OnCharacterCardClicked(0);

            
            Data.Instance.onBoardingManager.ShowStatHints();

        }
        void BackDelayed()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }
        int id = 0;
        bool loaded;
        public void LoadCharacters()
        {
            List<DBUserData.DBCharacterData> all;
            if (!loaded)
            {
                loaded = true;
                Utils.RemoveAllChildsIn(replaceContainer);
                
                cards.Clear();
                InitCascade();
                id = 0;

                //Load goalkeepers + players
                //:
                all = Data.Instance.myTeam.GetCharacters(true);
                all.AddRange(Data.Instance.myTeam.GetCharacters(false));

                all = DBUserData.DBCharacterData.SortCharacters(all, DBUserData.DBCharacterData.SortOrder.Upgradeable);

                foreach (DBUserData.DBCharacterData uData in all)
                {
                    ShowCharacter(uData);
                }

                StartCascade();
            } else
            {
                bool resort = false;
                // relodea la data de los botones si ya estaban cargados:
                int id = 0;
                all = Data.Instance.myTeam.GetCharacters(true);
                all.AddRange(Data.Instance.myTeam.GetCharacters(false));

                DBUserData.DBCharacterData chD = all.Find(x => x.id == lastIdClicked);
                if (chD != null) {
                    if (chD.available_stats == 0) {
                        resort = true;
                        all = all.OrderByDescending(x => x.available_stats).ToList();
                    }
                } else {
                    resort = true;
                    all = all.OrderByDescending(x => x.available_stats).ToList();
                }

                if (resort) {
                    foreach (DBUserData.DBCharacterData uData in all) {
                        cards[id].OnInit(uData, uData.IsGoalkeeper(), true); id++;
                    }
                } else {
                    foreach (CharacterButton chB in cards) {
                        chB.OnInit(all.Find(x=>x.id== chB.dbCharacterData.id), chB.dbCharacterData.IsGoalkeeper(), true);
                    }
                }
            }

            if (forceCharacter)
                forceCharacter = false;
            else
                OnLastIdClicked(lastIdClicked);

        }

        void ShowCharacter(DBUserData.DBCharacterData uData)
        {
            CharactersData.CharacterData cData = CharactersData.Instance.GetCharacterData(uData.player_id, uData.IsGoalkeeper());
            cData.SetDataFromDB(uData);
            CharacterButton card = Instantiate(characterButton, replaceContainer);
            card.OnInit(uData, uData.IsGoalkeeper(), true);
            card.Init(id, OnCharacterCardClicked, "", true);
            card.transform.localScale = Vector2.one;
            cards.Add(card);
            AddToCascade(card.GetComponent<ButtonCascade>());
            id++;
        }

        public void UpdatePrice(DBUserData.DBCharacterData dbCharacterData) {
            characterValue.SetPrice(dbCharacterData);
        }

        public void OnSelectCharacter(DBUserData.DBCharacterData dbCharacterData)
        {
            Events.RefreshData(dbCharacterData);
            RefreshCharacterData(dbCharacterData);
            characterValue.SetPrice(dbCharacterData);
        }
        public void RefreshCharacterData(DBUserData.DBCharacterData dbCharacterData)
        {
            characterCardFull.OpenCharacterFullCard(dbCharacterData, OnBackToScreen);
            this.dbCharacterData = dbCharacterData;
            myTeamCharacterData.RefreshData(dbCharacterData);
        }
        public void OnUpgradeFX()
        {
            ResetAnim();
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_level_up");
            characterCardFull.characterForCamera.SetAnim("enter");
            levelUpFX.SetActive(true);
            CancelInvoke();
            Invoke("ResetAnim", 2);
            
        }
        void ResetAnim()
        {
            levelUpFX.SetActive(false);
            Invoke("ResetAnimInCharacter", 3);
        }
        void ResetAnimInCharacter()
        {
            characterCardFull.characterForCamera.SetAnim("run");
        }
        void OnBackToScreen()
        {
            Data.Instance.ui.SetBackButton(true, Back);
        }
        void OnLastIdClicked(int id)
        {
            int a = 0;
            foreach (CharacterButton b in cards)
            {
                if (b.dbCharacterData.id == id)
                {
                    OnCharacterCardClicked(a);
                    return;
                }
                a++;
            }
        }
        void OnCharacterCardClicked(int buttonID)
        {
            this.lastIdClicked = cards[buttonID].dbCharacterData.id;
            OnSelectCharacter(cards[buttonID].dbCharacterData);

            foreach (ButtonCustom b in cards)
                b.OnSelected(false);

            cards[buttonID].OnSelected(true);
        }
        public void Back()
        {
            loaded = false;
            CancelInvoke();
            anim.Play("out");
            Events.Back();
            Invoke("Reseted", 0.5f);
        }
        private void Reseted()
        {
            gameObject.SetActive(false);
        }

        public void ShowFAQ(bool enable) {
            faq.SetActive(enable);
        }
    }

}