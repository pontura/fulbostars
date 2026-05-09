using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fulbo.Stadiums;
using Fulbo.Onboarding;

namespace Fulbo.UI
{
    public class CardsWinScreen : MonoBehaviour
    {
        public Text characterField;
        public CardWinAsset cardAsset;
        public int id;
        public List<CardWinAsset> all;
     //   public List<int> initial_characters;// = new List<int>() { 1,2,3,4,5,6,7,8,9,10,11 };
      //  public List<int> initial_goalkeepers;// = new List<int>() { 1,2 };
        public FigusScreen figusScreen;
        List<CharacterRolData> allCharactersToAdd;
        [SerializeField] Text title;
        bool firstTime;

        void OpenPackAutomatically(bool isOk)
        {
            TurnNext();
        }

        public void EnterNext() {
            foreach (CardWinAsset card in all) { 
                if (!card.isDone) {
                    card.CardEnter();
                    return;
                }
            }
        }

        public void TurnNext()
        {
            foreach (CardWinAsset card in all)
                if (!card.isDone)
                {
                    card.Turn();
                    return;
                }
        }
        public bool AllTurned()
        {
            foreach (CardWinAsset card in all)
                if (!card.isDone)
                    return false;
            return true;
        }
        public void SetCards(List<DB.DBUserData.DBCharacterData> all, bool firstTime)
        {
            this.firstTime = firstTime;
            allCharactersToAdd = new List<CharacterRolData>();

            //Utils.Shuffle(initial_goalkeepers);
            //Utils.Shuffle(initial_characters);
            int id = 0;
            foreach (DB.DBUserData.DBCharacterData uData in all)
            {
                id++;

                CharacterRolData characterRolData = new CharacterRolData(uData.player_id, uData.IsGoalkeeper());
                allCharactersToAdd.Add(characterRolData);
                characterRolData.positionText = uData.GetPositionText();
                characterRolData.totalStats = uData.GetTotalStats();
                characterRolData.uniqueID = uData.id;
                //for (int a = 0; a < StoryModeData.Instance.GetLevelActual().myTeamQty; a++)
                //{
                //    characterRolData = new CharacterRolData(initial_characters[a], false);
                //    allCharactersToAdd.Add(characterRolData);
                //}
            }
            AddContent();
           // Data.Instance.myTeam.Save();
            CardsAdded();
        }
        void CardsAdded()
        {
            title.text = ""; // Data.Instance.texts.Get("tap_card");
        }
        public void SetOneSingleCard()
        {
            //CardsAdded();
            //singleCard = true;
            //allCharactersToAdd = new List<CharacterRolData>();

            //Utils.Shuffle(initial_goalkeepers);
            //Utils.Shuffle(initial_characters);

            //CharacterRolData characterRolData;

            //characterRolData = new CharacterRolData(initial_characters[0], false);
            //allCharactersToAdd.Add(characterRolData);

            //AddContent();
            //Data.Instance.myTeam.Save();
        }
        void AddCharacter(CharacterRolData characterRolData)
        {
            allCharactersToAdd.Add(characterRolData);
        }
        void AddContent()
        {
            title.text = Data.Instance.texts.Get("open_envelope");

            int total = allCharactersToAdd.Count;
            List<GameObject> allSlots = figusScreen.GetSlots(total);
            int num = 0;

            foreach (CharacterRolData cData in allCharactersToAdd)
            {
                CardWinAsset card = Instantiate(cardAsset, transform);
                card.transform.localPosition = Vector2.zero;
                card.transform.SetParent(allSlots[num].transform);
                card.transform.localScale = Vector3.one;
                all.Add(card);
                card.InitPlayer(this, cData.characterID, cData.isGoalkeeper, cData.totalStats, cData.uniqueID);
                card.transform.localEulerAngles = Vector3.zero;
                card.CardEnter();
                num++;
            }
            StartCoroutine(TurnCardsAutomatically());
        }
        IEnumerator TurnCardsAutomatically()
        {
            foreach (CardWinAsset card in all)
            {
                yield return new WaitForSeconds(1);
                card.TurnAutomatically();
            }
            yield return new WaitForSeconds(1);
        }
        bool isReady;
        public void CheckReady()
        {
            if (isReady) return;
            title.text = "";
            bool isDone = true;
            foreach (CardWinAsset cwa in all)
                if (!cwa.isDone)
                    isDone = false;
            if (isDone)
            {
                isReady = true;
                if (!firstTime) {
                    OnReadyDone();
                } else
                    OnNext();
            }           
        }
        void OnNext()
        {
            print("NEXT" + all.Count);
           // Data.Instance.myTeam.ForceCharactersToSelectedTeam();
            Data.Instance.matchData.AddRealTeam(11);

            if (all.Count < 2)
            {
                OnReadyDone();
            }
            else
            {
                int biggestStats = 0;
                int smallestStats = 1000;
                int avatarBiggestStatsID= all[1].characterID;
                int avatarSmallestStatsID = all[2].characterID;

                foreach ( CharacterRolData d in allCharactersToAdd)
                {
                    if (d.totalStats > biggestStats)
                    {
                        avatarBiggestStatsID = d.characterID;
                        biggestStats = d.totalStats;
                    }
                    else if(d.totalStats < smallestStats)
                    {
                        avatarSmallestStatsID = d.characterID;
                        smallestStats = d.totalStats;
                    }
                }
                print("Cards ready");
                if (!Data.Instance.onBoardingManager.IsBoardingStepDone(OnBoardingManager.BoardingStepStates.FIRST_MATCH_PLAYED))
                    Events.OnboardingCheckStep(Onboarding.OnboardingPanel.panels.intro, 2, OnOnBoardingDone);

                //string avatar1 = Data.Instance.textsData.GetCharactersData(avatarSmallestStatsID, false).avatarName;
                //string avatar2 = Data.Instance.textsData.GetCharactersData(avatarBiggestStatsID, false).avatarName;
                //string text = Data.Instance.texts.Get("initial1");
                //text = Data.Instance.texts.ReplaceTextsByCharacter(text, 1, avatar1);
                //text = Data.Instance.texts.ReplaceTextsByCharacter(text, 2, avatar2);
                //Events.OnPopup(text, OnReadyDone);
            }
        }
        void OnOnBoardingDone(bool isOk)
        {
            OnReadyDone();
        }
        public void OnReadyDone()
        {
            figusScreen.Next();
        }
        public void Reset()
        {
            foreach(CardWinAsset c in all)
            {
                Destroy(c.gameObject);
            }
            all.Clear();

            isReady = false;
            title.text = Data.Instance.texts.Get("open_envelope");
        }
        public void BackToFigus()
        {
            figusScreen.Init(0);
        }
    }
}