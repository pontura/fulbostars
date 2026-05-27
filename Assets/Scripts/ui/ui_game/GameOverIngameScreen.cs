using System.Collections;
using System.Collections.Generic;
using Fulbo.DB;
using UnityEngine;

namespace Fulbo.UI
{
    public class GameOverIngameScreen : MonoBehaviour
    {
        [SerializeField] GameObject panel;
        [SerializeField] Animator anim; //perder, empatar, ganar
        [SerializeField] GameObject[] referies; //ids de los referies
        int referiID = 0;

        void Start()
        {
            panel.SetActive(false);
            Events.GameOver += GameOver;
        }
       
        void OnDestroy()
        {
            Events.GameOver -= GameOver;
        }
        void GameOver()
        {
            DBManager.Instance.tournamentsManager.OnSendResults(OnResultsSended); 
            panel.SetActive(true);
            AudioManager.Instance.PlaySound("music", "", true);
            AudioManager.Instance.FadeVolume("ambience", 0, 0.5f);            
            SetReferis();           
        }
        void OnResultsSended(bool ok)
        {
            print("OnResultsSended " + ok);

             if (Data.Instance.matchData.IsTutorial()) // if played a Tutorial Match
            {
                Events.OnVoiceSay("trainingendmatch", null);
                if (DB.DBManager.Instance.DbUserData.state == DB.DBUserData.userStates.FIRST_TIME)
                    Events.OnPopup(Data.Instance.texts.Get("tutorialFinish"), TutorialReady);
                else
                    Events.OnPopup(Data.Instance.texts.Get("tutorialFinish2"), TutorialReady);
            } else
                StartCoroutine( C() );
        }
         string animName = "";
        void SetReferis()
        { 
            foreach (GameObject g in referies)
                g.SetActive(false);

            

            if (Data.Instance.mode == Data.modes.PARTYMODE)
                referiID = CharactersData.Instance.GetReferi().id;
            else if (Data.Instance.matchData.levelData.referiID <= referies.Length)
                referiID = Data.Instance.matchData.levelData.referiID;

            referies[referiID-1].SetActive(true);

            
            if(Data.Instance.tournamentsData.IsTournament())
             {
                 if(Data.Instance.tournamentsData.myTeamID == 1)
                    if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                        animName = "endWin";
                    else if (Data.Instance.matchData.score.x < Data.Instance.matchData.score.y)
                        animName = "endLose";
                    else
                        animName = "endTied";
                else
                    if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                        animName = "endLose";
                    else if (Data.Instance.matchData.score.x < Data.Instance.matchData.score.y)
                        animName = "endWin";
                    else
                        animName = "endTied";
             }
             else
             {
                 if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                     animName = "endWin";
                 else if (Data.Instance.matchData.score.x < Data.Instance.matchData.score.y)
                     animName = "endLose";
                 else
                     animName = "endTied";
             }

            AudioManager.Instance.PlaySoundOneShot("ui", "ui/endScreen/" + animName);

            anim.Play(animName);
        }
        IEnumerator C()
        {
            yield return new WaitForSeconds(1.5f);

            if(Data.Instance.mode == Data.modes.PARTYMODE)
                Data.Instance.LoadLevel("GameOverPartymode");
            else
                Data.Instance.LoadLevel("GameOver");

            yield return new WaitForSeconds(1.25f);
            anim.Play(animName + "_exit");

            yield return new WaitForSeconds(1.5f);
            panel.SetActive(false);
        }
        void TutorialReady()
        {
            Data.Instance.myTeam.myTeamData.GameOver(0, null);
            Data.Instance.LoadLevel("MainMenu");
        }
    }
}
