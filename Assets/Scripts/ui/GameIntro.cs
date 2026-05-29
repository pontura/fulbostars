using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Fulbo.Game
{
    public class GameIntro : UI.UIMainScreen
    {
        public float speed = 2;
        int maxTeamCharacters;
        public Character character_to_instantiate;
      //  public GameObject ui;

        public CharactersManager charactersManager;
        public Transform container_team1;
        public Transform container_team2;
        [SerializeField] Text stadiumNameField;

        public override void OnSkipButtonPressed()
        {
            OnSkip();
        }
        void Start()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/game_cup_intro");
            if (Data.Instance.mode == Data.modes.PVP)
                Data.Instance.ui.SetBackButton(true, Back);
            else if (Data.Instance.mode == Data.modes.STORYMODE && DB.DBManager.Instance.DbUserData.state != DB.DBUserData.userStates.FIRST_TIME)
                Data.Instance.ui.SetBackButton(false, null);
            else
                Data.Instance.ui.SetBackButton(false, null);

            AddCharacters(1, Data.Instance.matchData.totalCharacters_team1, container_team1);
            AddCharacters(-1, Data.Instance.matchData.totalCharacters_team2, container_team2);

            if (Data.Instance.matchData.totalCharacters_team1 >= Data.Instance.matchData.totalCharacters_team2)
                maxTeamCharacters = Data.Instance.matchData.totalCharacters_team1;
            else
                maxTeamCharacters = Data.Instance.matchData.totalCharacters_team2;

            GetComponent<DialoguesManager>().Init();
            AudioManager.Instance.ChangeVolume("crowd", 0.25f);

            charactersManager.Init( true);
            charactersManager.referi.gameObject.SetActive(false);

            foreach (Character ch in charactersManager.team1)
                ch.gameObject.SetActive(false);
            foreach (Character ch in charactersManager.team2)
                ch.gameObject.SetActive(false);

            StartCoroutine(Init());

            Events.OnSkipOn(OnSkip, "skip");
            GetComponent<StadiumsManager>().Init();

            stadiumNameField.text = Fulbo.Stadiums.StadiumsData.Instance.active.name;
            
        }
        public void Back()
        {
            if (Data.Instance.mode != Data.modes.PARTYMODE)
                Data.Instance.LoadLevel("Levels");
        }
        void AddCharacters(int teamDirection, int totalCharacters, Transform container)
        {
            for (int a = 0; a < totalCharacters; a++)
            {
                Character character = Instantiate(character_to_instantiate, Vector3.zero, Quaternion.identity, container);
                character.transform.localPosition = new Vector3(0.5f * teamDirection, 0.54f, 16);
                character.GetComponent<Collider>().isTrigger = true;
                if (a == 0)
                    character.type = Character.types.GOALKEEPER;
                else
                    character.type = Character.types.MID;

            }
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
        IEnumerator Init()
        {
            yield return new WaitForEndOfFrame();

            AudioManager.Instance.FadeVolume("music", 0, 0.1f);
            //AudioManager.Instance.ChangePitch("ambience", 0.9f);
            AudioManager.Instance.ChangeVolume("music2", 0.8f);
            AudioManager.Instance.PlaySpecificSound(Fulbo.Stadiums.StadiumsData.Instance.active.opening, "music2", true);
            AudioManager.Instance.ChangeVolume("crowd", Fulbo.Stadiums.StadiumsData.Instance.active.crowd_expr_vol);

            //Events.OnIntroSound(1, null);

            yield return new WaitForSeconds(0.2f);

            Events.OnIntroSound(2, charactersManager.referi);
            charactersManager.referi.gameObject.SetActive(true);

            // float rotInitial_x = -8f;
            // float rotInitial_goalkeeper_x = -15f;

            float rotInitial_x = 0;
            float rotInitial_goalkeeper_x = 0;

            charactersManager.referi.transform.localEulerAngles = new Vector3(rotInitial_x, 0, 0);

            yield return new WaitForSeconds(1f);
            charactersManager.referi.states.PlayAnim("enter");
            Events.SetDialogue(charactersManager.referi, Data.Instance.textsData.GetRandomReferiDialogue("random"));
            float vol = 0.5f;
            AudioManager.Instance.ChangeVolume("crowd", vol);
            AudioManager.Instance.PlaySound("common", "ingame/referee/game_referee_opening", false);
            Character character = null;
            for (int id = 0; id < maxTeamCharacters; id++)
            {
                float _x = rotInitial_x;
                if (id == 0)
                    _x = rotInitial_goalkeeper_x;
                if (id <= charactersManager.team1.Count - 1)
                {
                    character = charactersManager.team1[id];
                    StartCoroutine(SetCharacterOn(character));
                    character.transform.localScale = new Vector3(-1, 1, 1);
                    character.transform.localEulerAngles = new Vector3(_x, 7, 0);
                    yield return new WaitForSeconds(1.3f);
                }

                if (id <= charactersManager.team2.Count - 1)
                {
                    character = charactersManager.team2[id];
                    StartCoroutine(SetCharacterOn(character));
                    character.transform.localScale = new Vector3(1, 1, 1);
                    character.transform.localEulerAngles = new Vector3(_x, -7, 0);
                    yield return new WaitForSeconds(1.3f);
                }
                vol -= 0.05f;
                AudioManager.Instance.ChangeVolume("crowd", vol);
            }
            yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(3);
            Ready();
        }
        IEnumerator SetCharacterOn(Character character)
        {
            InitBillboard(character);
            character.gameObject.SetActive(true);
            //character.states.Move(1);
            yield return new WaitForSeconds(2.6f);
            character.GetComponentInChildren<CharacterBillboard>().SetAnim("enter");
            //character.states.PlayAnim("enter");
            Events.OnIntroSound(0, character);
            // if(Random.Range(0,10)>3)
            Events.SetDialogue(character, Data.Instance.textsData.GetRandomDialogue("random", character.data.id, character.type == Character.types.GOALKEEPER));
            yield return new WaitForSeconds(3f);
            character.GetComponentInChildren<CharacterBillboard>().Reset();
            character.gameObject.SetActive(false);
        }
        void Update()
        {
            if (charactersManager.referi.gameObject.activeSelf)
                Move(charactersManager.referi);
            foreach (Character ch in charactersManager.team1)
                if (ch.gameObject.activeSelf)
                    Move(ch);
            foreach (Character ch in charactersManager.team2)
                if (ch.gameObject.activeSelf)
                    Move(ch);
        }
        private void Move(Character ch)
        {
            Vector3 pos = ch.transform.localPosition;
            pos.z -= speed * Time.deltaTime;
            ch.transform.localPosition = pos;
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
            if (character.teamID == 1) billboard.SetScaleX(-1);
        }
    }

}