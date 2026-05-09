using Fulbo.AssetsBundle;
using Fulbo.Stadiums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fulbo.Onboarding;

namespace Fulbo.UI
{
    public class FigusScreen : UIMainScreen
    {
        public FigusAsset[] figuAsset;
        public GameObject firstTimeBG;
        public CardsWinScreen cardsWinScreen;
        public Text field;
        public string textDone = "No players, sorry!";
        public bool isFirstTime;
        public GameObject[] container;
        public List<FigusAsset> allFigus;
        Transform figusContainer;
        [SerializeField] UIParticleSystem uiParticleSystem;


        public states state;
        public enum states
        {
            FIGUS,
            CARDS,
            READY
        }

        private void Start() {
            Events.OnFlyingPArrives += OnFlyingPArrives;
        }

        private void OnDestroy() {
            Events.OnFlyingPArrives -= OnFlyingPArrives;
        }

        public override void OnSkipButtonPressed()
        {
            if (state == states.FIGUS && newFigus != null)
                OnClicked(newFigus);
            else
                cardsWinScreen.TurnNext();

            if(cardsWinScreen.AllTurned() && state == states.CARDS)
            {
                state = states.READY;
                cardsWinScreen.OnReadyDone();
            }
        }

        System.Action OnClose;
        /// <summary>
        /// Init the figus screen with card rarity base 1 and optional callback on close
        /// </summary>
        /// <param name="rarity">rarity base 1</param>
        public void Init(int rarity, System.Action onclose=null)
        {
            OnClose = onclose;

            if (DB.DBManager.Instance.DbUserData.data.old_score >= 10000)
                Events.CheckForImportantNotifications("loading");
            gameObject.SetActive(true);
            firstTimeBG.SetActive(false);
            if (Data.Instance.onBoardingManager.IsBoardingStep(OnBoardingManager.BoardingStepStates.FIRST_TIME_GAME_LOADED))
            {
                Data.Instance.ui.SetBackButton(false);
                firstTimeBG.SetActive(true);
                
                AssetsBundleManager.Instance.InstantiateAssets();                              
                isFirstTime = true;
                Events.OnboardingCheckStep(Onboarding.OnboardingPanel.panels.intro, 1, OnboardingSkip);

            }else
                isFirstTime = false;
            
            AddFigus(rarity);
        }
        void OnboardingSkip (bool isOk)
        {
            if(allFigus.Count>0)
                allFigus[0].OnClicked(); // clickea la primera
            //OnSkipButtonPressed();
        }
        FigusAsset newFigus;
        void AddFigus(int rarity)
        {
            allFigus.Clear();
            Events.OnTrack("PlayerPackShown", null);
            cardsWinScreen = GetComponent<CardsWinScreen>();
            List<GameObject> all = GetSlots(1);
            figusContainer = all[0].transform;

            FigusAsset go = figuAsset[0];
            if (rarity>0 && (rarity-1) < figuAsset.Length)
                go = figuAsset[rarity-1];

            newFigus = Instantiate(go, figusContainer);
            newFigus.Init(this, 0);
            newFigus.transform.localScale = Vector3.one;
            newFigus.EnterEnvelope();
            newFigus.SetBack(true);
            allFigus.Add(newFigus);
        }
        private void Reset()
        {
            if (figusContainer != null)
                Utils.RemoveAllChildsIn(figusContainer);
        }
        int cardCant = 1;
        public void OnClicked(FigusAsset fa)
        {
            state = states.CARDS;            
            int id = fa.id;

            if (isFirstTime)
            {
                DB.DBManager.Instance.DbGameData.Put("tutorialStep", "" + (int)OnBoardingManager.BoardingStepStates.GOT_FIRST_PLAYER_CARDS, null);
                cardsWinScreen.SetCards(Data.Instance.myTeam.GetBestTeamDataPlayersID(6), true);
                Events.OnTrack("PlayerPackOpened", null);
            }
            else
            {

                List<DB.DBUserData.DBCharacterData> playersList = DB.DBManager.Instance.DbUserData.GetWonCharacters();
                cardCant = playersList.Count;
                cardsWinScreen.SetCards(playersList, false);
            }
            Invoke("DeleteEnvelopes", 0.5f);
        }
        void DeleteEnvelopes()
        {
            int num = allFigus.Count;
            while (num > 0)
            {
                Destroy(allFigus[num - 1].gameObject);
                num--;
            }
            allFigus.Clear();
        }
        public void Next()
        {
            if (!isFirstTime) {
                Invoke("BackToMainMenu", 1.5f);
            } else {
                OnBoardingNext();
                cardsWinScreen.Reset();
                Reset();
                gameObject.SetActive(false);
            }
            
            
        }

        void BackToMainMenu() {
            cardsWinScreen.Reset();
            Reset();
            Vector2 centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            Events.OnFlyingParticles(cardCant, FlyingParticlesUI.types.CARD, centerOfScreen, 0, 1);
            Data.Instance.ui.ShowShield(true);
            cardCant = 1;
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/ui_cardUp");
            gameObject.SetActive(false);
            if (OnClose != null)
                OnClose();
        }
        private void OnFlyingPArrives(FlyingParticlesUI.types type, float percent, float init, float final) {
            if (type != FlyingParticlesUI.types.CARD) return;
            uiParticleSystem.Play();

            int _id = (int)Mathf.Round(percent * 7);
            if (_id > 7) _id = 7;
            if (_id < 1) _id = 1;
            string soundName = "ui/cards/ui_card" + _id;

            AudioManager.Instance.PlaySound("fx", soundName, false);

            if (percent == 1)
                Invoke("CloseShield", 1);
        }

        void CloseShield() {
            Data.Instance.ui.ShowShield(false);
        }

        void OnBoardingNext()
        {
            Data.Instance.matchData.AddRealTeam(6);
            //Data.Instance.charactersPositions.LoadPositions();
            //Events.OnTutorialProgressMenu(Fulbo.Game.Tutorial.TutorialData.Instance.all.Length - 1);
            AudioManager.Instance.FadeVolume("music", 0.2f, 1f);
            Data.Instance.onBoardingManager.BoardingNextScene();
        }
        public List<GameObject> GetSlots(int total)
        {
            allFigus.Clear();
            List<GameObject> all = new List<GameObject>();
            GameObject actualByQty = container[total];
            actualByQty.SetActive(true);

            foreach (FigusAsset fa in actualByQty.GetComponentsInChildren<FigusAsset>())
            {
                if (fa.gameObject.tag == "Respawn")
                {
                    fa.SetBack(false);
                    all.Add(fa.gameObject);
                }
            }
            /*foreach (GameObject go in container)
                go.SetActive(false);*/

            container[total].SetActive(true);


            return all;
        }
    }
}