using System.Collections.Generic;
using Fulbo.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class PrensaScreen : MonoBehaviour
    {
        public CharacterCardInGame[] cards;
        public Text[] teams;
        public Text[] goles;
        public Text[] golesAdded;
        [SerializeField] CharacterForCamera character;
        [SerializeField]  CharacterDialogueUI dialogueUI;
        string[] frases;
        bool team2_win = true;

        void Start()
        {
            List<LevelData> teams = CupsData.Instance.levels.GetByState("torneo");
            
            SetResults();
            
            team2_win = Data.Instance.tournamentsData.lastMatchGoles1 < Data.Instance.tournamentsData.lastMatchGoles2;

            if(team2_win)
                frases = Data.Instance.tournamentsData.GetRandomFrases(0, 1);
            else
                frases = Data.Instance.tournamentsData.GetRandomFrases(0, 2);

            Events.OnSkipOn(OnSkip, "skip");

            int chID = Data.Instance.matchData.team1[1];

            if(team2_win)
                chID = Data.Instance.matchData.team2[1];

            CharactersData.CharacterData d = CharactersData.Instance.GetCharacterData(chID, false);
            character.Init(d, "idle");  
            
            Loop();
            dialogueUI.gameObject.SetActive(false);
            Data.Instance.tournamentsData.SetTournament(false);

            Invoke("OnSkip", 20);
            Invoke("Delayed", 0.1f);
        }
        void Delayed()
        {
            cards[0].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team1[1], false), 10000);
            cards[1].ForceShow(CharactersData.Instance.GetCharacterData(Data.Instance.matchData.team2[1], false), 10000);
            
            Data.Instance.matchData.ResetAll();
        }
         void SetResults()
        {
            goles[0].text = (Data.Instance.tournamentsData.goles2 + Data.Instance.tournamentsData.lastMatchGoles2).ToString();
            goles[1].text = (Data.Instance.tournamentsData.goles1 + Data.Instance.tournamentsData.lastMatchGoles1).ToString();
            golesAdded[0].text = "+" + (Data.Instance.tournamentsData.lastMatchGoles2).ToString();
            golesAdded[1].text = "+" + (Data.Instance.tournamentsData.lastMatchGoles1).ToString();
        }
        void OnSkip()
        {
            CancelInvoke();
            Events.OnSkipOff();
            Data.Instance.LoadLevel("SplashOptions");
        }
        string anim = "idle";
        void Loop()
        {
            if(anim == "idle")
                anim = "goal";
            else
                anim = "idle";   

            character.SetAnim(anim);
            Invoke("Loop", Random.Range(3, 6));
            dialogueUI.Init(null, frases[Random.Range(0, frases.Length - 1)]);
        }
        
    }

}