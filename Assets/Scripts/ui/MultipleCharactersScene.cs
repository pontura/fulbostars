using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class MultipleCharactersScene : MonoBehaviour
    {
        CharacterForCamera[] characters;
        int id = 0;
        [SerializeField] int special_character = 156;
        int id_for_special_character = 0;
        int id_special_character = 0;
        void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            return;
#endif
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
            StartCoroutine(Appear());
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

           // if (d.rarity == FigusData.rarities.normal)
                return d;
           // return GetNextCharacter();

        }
        IEnumerator Appear()
        {
            foreach (CharacterForCamera character in characters)
            {
                yield return new WaitForSeconds(0.05f);
                character.gameObject.SetActive(true);
                character.SetAnim("run");
            }
        }
    }
}