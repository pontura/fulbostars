using Fulbo.Game;
using UnityEngine;

namespace Fulbo.UI
{
    public class PrensaScreen : MonoBehaviour
    {
        [SerializeField] CharacterForCamera character;
        [SerializeField]  CharacterDialogueUI dialogueUI;
        string[] frases;
        bool team2_win = true;
        void Start()
        {
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
            
            Data.Instance.matchData.ResetAll();
            Loop();
            dialogueUI.gameObject.SetActive(false);
            Data.Instance.tournamentsData.SetTournament(false);
        }
        void OnSkip()
        {
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
            Invoke("Loop", Random.Range(4, 6));
            dialogueUI.Init(null, frases[Random.Range(0, frases.Length - 1)]);
        }
        
    }

}