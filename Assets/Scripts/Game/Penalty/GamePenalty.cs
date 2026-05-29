using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Game
{
    public class GamePenalty : MonoBehaviour
    {
        public Character character_to_instantiate;

        public CharactersManager charactersManager;
        public Transform character_1;
        public Transform container_2;

        void Start()
        {

            AddCharacters(1, 0, character_1);
            AddCharacters(2, 1, container_2);

            GetComponent<DialoguesManager>().Init();
            AudioManager.Instance.ChangeVolume("crowd", 0.25f);

            charactersManager.Init( true);
            charactersManager.referi.gameObject.SetActive(false);

            foreach (Character ch in charactersManager.team1)
            {
                ch.gameObject.SetActive(true);
                InitBillboard(ch);
                ch.GetComponentInChildren<CharacterBillboard>().SetAnim("idle");
            }
            foreach (Character ch in charactersManager.team2)
            {
                ch.gameObject.SetActive(true);
                InitBillboard(ch);
                ch.GetComponentInChildren<CharacterBillboard>().SetAnim("idle");
            }

            AudioManager.Instance.PlaySound("music", "", false);

           // Events.OnSkipOn(OnSkip);
            GetComponent<StadiumsManager>().Init();
            GetComponent<Fulbo.Game.Penalty.PenaltyCharactersManager>().Init();

        }
        public void Back()
        {
            Data.Instance.LoadLevel("Levels");
        }
        void AddCharacters(int teamID, int characterID,  Transform container)
        {
            Character character = Instantiate(character_to_instantiate, Vector3.zero, Quaternion.identity, container);
            character.transform.localPosition = Vector3.zero;
            character.GetComponent<Collider>().isTrigger = true;
            if (characterID == 0)
                character.type = Character.types.GOALKEEPER;
            else
                character.type = Character.types.MID;

        }
        void OnSkip()
        {
            Ready();
        }
        void Ready()
        {
            if (!isActiveAndEnabled) return;
            StopAllCoroutines();
            Data.Instance.LoadLevel("Game");
            Events.OnSkipOff();
        }

        [SerializeField] CharacterBillboard characterBillboard;

        int bilboardID = 1;
        public void InitBillboard(Character character)
        {
            CharactersData.CharacterData characterData = CharactersData.Instance.GetCharacterData(character.data.id, character.type == Character.types.GOALKEEPER);
            CharacterBillboard billboard = Instantiate(characterBillboard, character.transform);
            billboard.transform.localEulerAngles = new Vector3(90, 180, 0);
            float _y = 0.3f;
            if (character.type == Character.types.GOALKEEPER)
                _y = 0.5f;
            billboard.transform.localPosition = new Vector3(0, _y, 0);

            float scale = 0.45f;
            float scale_x = 0.35f;
            billboard.transform.localScale = new Vector3(scale_x, scale, scale);

            bilboardID++;
            billboard.Init(characterData, bilboardID);
            if (character.teamID == 2) billboard.SetScaleX(-1);
        }
    }

}