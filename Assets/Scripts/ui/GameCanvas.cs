using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class GameCanvas : MonoBehaviour
    {
        public GameObject goalAsset;

        void Start()
        {
            if(Data.Instance.mode == Data.modes.PARTYMODE)
            { }
            else if (DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep < 2)
                Invoke("OnboardingFirstMatch", 1f);

            Events.OnGoal += OnGoal;
            goalAsset.SetActive(false);
        }
        float timescale = 1;
        void OnboardingFirstMatch()
        {
            Time.timeScale = 0;
            Events.OnboardingCheckStep(Onboarding.OnboardingPanel.panels.intro, 3, OnboardingSkip);
        }
        void OnboardingSkip(bool a)
        {
            Events.OnTrack("TutorialStarted", null);
            Time.timeScale = timescale;
        }
        void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
        }
        void OnGoal(int teamID, Character c)
        {
            StartCoroutine(DoTheGoal());
        }
        IEnumerator DoTheGoal()
        {
            yield return new WaitForSeconds(0.1f);
            goalAsset.SetActive(true);
            yield return new WaitForSeconds(5);
            goalAsset.SetActive(false);
        }
    }

}