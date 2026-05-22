using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Fulbo.Game
{
    public class MultipleCharactersScene : MonoBehaviour
    {
        [SerializeField] CharacterForCamera[] characters;
        [SerializeField] CharacterForCamera[] team1;
        [SerializeField] CharacterForCamera[] team2;
        int id = 0;
        [SerializeField] int special_character = 156;
        int id_for_special_character = 0;
        int id_special_character = 0;

        void Start()
        {
            if (!Data.Instance.tournamentsData.IsTournament())
                AllScreen();
        }
        public void TounrnamentMode()
        {
            Data.Instance.partyModeData.Reset();
            SetTeam(team1, 1);
            SetTeam(team2, 2);
        }
        void SetTeam(CharacterForCamera[] team, int _teamID )
        {
            Data.Instance.partyModeData.Reset();
            Utils.Shuffle(team);
            id = 0;
            id_for_special_character = Random.Range(0, 10);
            foreach (CharacterForCamera character in team)
            {
                int chID = 0;

                if(_teamID ==1) 
                    chID = Data.Instance.matchData.team1[id];
                else  
                    chID = Data.Instance.matchData.team2[id];

                CharactersData.CharacterData d = CharactersData.Instance.GetCharacterData(chID, id == 0);
                character.Init(d, "run");
                character.gameObject.SetActive(false);
                id++;
            }
            StartCoroutine(Appear(team));
        }
        void AllScreen()
        {
            Data.Instance.partyModeData.Reset();
            characters = GetComponentsInChildren<CharacterForCamera>();
            Utils.Shuffle(characters);
            id = Random.Range(0, CharactersData.Instance.all.Count - 2);
            id_for_special_character = Random.Range(0, 10);
            foreach (CharacterForCamera character in characters)
            {
                CharactersData.CharacterData d = GetNextCharacter();
                character.Init(d, "run");
                character.gameObject.SetActive(false);
            }
            StartCoroutine(Appear(characters));
        }
        CharactersData.CharacterData GetNextCharacter()
        {
            CharactersData.CharacterData d;
            if (special_character > 0 && id_special_character == id_for_special_character)
            {
                d = CharactersData.Instance.GetCharacterData(special_character, false);
            }
            else
            {
                id++;
                if (id > CharactersData.Instance.all.Count - 1)
                    id = 0;
                d = CharactersData.Instance.all[id];
                if(!d.IsAvailable())
                {
                    return GetNextCharacter();
                }

            }
            print("initial character id: " + id);

            id_special_character++;

            return d;

        }
        IEnumerator Appear(CharacterForCamera[] chs)
        {
            foreach (CharacterForCamera character in chs)
            {
                yield return new WaitForSeconds(0.05f);
                character.gameObject.SetActive(true);
                character.SetAnim("run");
            }
        }
    }
}