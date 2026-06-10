using Fulbo.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class GameoverScene : MonoBehaviour
    {
        public Character character_to_instantiate;
        public GameObject[] allCharacters;
        public CharactersManager charactersManager;
        public List<Character> team;
        public Ball ball;
        bool isOn;
        bool win;
        [SerializeField] Camera cam;

        void Start()
        {
            isOn = true; 
            GetComponent<StadiumsManager>().Init();
            StartCoroutine(On());
        }
        Character winCharacter;
        IEnumerator On()
        {
            yield return new WaitForSeconds(0.1f);

            win = true;
            // Events.OnOutroSound(OnPita, audioID);
            int totalCharacters;
            // if (Data.Instance.matchData.score.y > Data.Instance.matchData.score.x)
            // {
                cam.GetComponent<Animation>().Play("gameOverScene");
            // }
            // else
            // {
            //     cam.GetComponent<Animation>().Play("gameOverSceneLose");
            // }

            totalCharacters = Data.Instance.matchData.totalCharacters_team2;

            Character character;
            for (int a = 0; a < totalCharacters; a++)
            {
                character = Instantiate(character_to_instantiate, Vector3.zero, Quaternion.identity, allCharacters[a].transform);
                character.transform.localPosition = Vector3.zero;


                if (a == 0)
                    character.type = Character.types.GOALKEEPER;
                else
                    character.type = Character.types.MID;

            }
            GetComponent<DialoguesManager>().Init();
            AudioManager.Instance.ChangeVolume("crowd", 0.25f);
                charactersManager.Init(true);

                int teamWon = Data.Instance.matchData.GetWinner();
                if(teamWon == 2)
                    team = charactersManager.team2;
                else     
                    team = charactersManager.team1;

            for (int id = 0; id < totalCharacters; id++)
            {
                character = team[id];
                if(win) InitBillboard(character, "goal");
                else  InitBillboard(character, "cry");

                if (character != null)
                {
                    character.gameObject.SetActive(true);

                    character.transform.localScale = Vector3.one;
                    //character.states.Goal();

                    if (id == 0)
                        winCharacter = character;
                }
            }

            SetGoalToCharacter();
            Events.OnShowDasboard(true);
            yield return new WaitForSeconds(0.2f);
            if(!Data.Instance.tournamentsData.IsTournament() && !Data.Instance.fixtureManager.isFixtureHappening)
            {
                Data.Instance.matchData.ResetAll();
            }
            yield return new WaitForSeconds(1);
            Events.OnSkipOn(OnSkip, "skip");
            yield return new WaitForSeconds(9);
            AudioManager.Instance.ChangeVolume("crowd", 0.5f);
        }
        void SetGoalToCharacter()
        {

            int characterGoalID = team[0].data.id; //VoicesManager.Instance.characterGoalID;
            int num = 0;

            Character characterToSwitch = team[0];
            Character character = team[1];
            foreach (Character ch in team)
            {
                if (ch.data.id == characterGoalID)
                {
                    if (num == 0)
                        return;
                    character = ch;
                }
                num++;
            }
            Transform containerWinner = characterToSwitch.transform.parent.transform;
            Transform containerOther = character.transform.parent.transform;

            characterToSwitch.transform.SetParent(containerOther);
            character.transform.SetParent(containerWinner);

            characterToSwitch.transform.localPosition = Vector3.zero;
            character.transform.localPosition = Vector3.zero;
            character.transform.localScale = Vector3.one;
        }
        private void OnDisable()
        {
            isOn = false;
            AudioManager.Instance.FadeVolume("music", 0.3f); 
            Events.OnShowDasboard(false);
        }


        [SerializeField] CharacterBillboard characterBillboard;

        int bilboardID = 1;
        public void InitBillboard(Character character, string action)
        {
            CharactersData.CharacterData characterData = CharactersData.Instance.GetCharacterData(character.data.id, character.type == Character.types.GOALKEEPER);
            //character.SetAsset(false);
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
            billboard.SetAnim(action);
            if (character.teamID == 2) billboard.SetScaleX(-1);
        }
        void OnSkip()
        {
            Events.OnSkipOff();
            AudioManager.Instance.FadeVolume("music", 0.5f);
            Data.Instance.OnSummaryOver();
        }
    }

}