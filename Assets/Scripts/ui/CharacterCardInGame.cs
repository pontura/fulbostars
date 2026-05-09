using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.CharactersData;

namespace Fulbo.UI
{
    public class CharacterCardInGame : MonoBehaviour
    {
        [SerializeField] Animation anim;
        [SerializeField] GameObject panel;
        [SerializeField] Image thumb;
        [SerializeField] Text nameField;
        [SerializeField] Text textField;
        [SerializeField] Transform containerTeam1;
        [SerializeField] Transform containerTeam2;
        bool isOn;
        Character character;

        void Start()
        {
            panel.SetActive(false);
            Events.OnGoal += OnGoal;
            Events.OnGameStatusChanged += OnGameStatusChanged;
        }
        void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
            Events.OnGameStatusChanged -= OnGameStatusChanged;
        }
        private void OnDisable()
        {
            CancelInvoke();
        }
        public void OnGoal(int teamID, Character character)
        {
            this.character = character;
            if (teamID == 1 && !Data.Instance.isMobile)
                panel.transform.SetParent(containerTeam1);
            else
                panel.transform.SetParent(containerTeam2);
            panel.transform.localPosition = Vector2.zero;
            Invoke("Show", 2);
        }
        public void ForceShow(CharacterData characterData, int duration)
        {

            CancelInvoke();
            isOn = true;
            anim.Play("on");
            panel.SetActive(true);

            TextsData.CharacterData data = Data.Instance.textsData.GetCharactersData(characterData.id);
            CharactersData.CharacterData cdata = CharactersData.Instance.GetCharacterData(characterData.id, characterData.isGoalkeeper);


            thumb.sprite = cdata.thumb;
            nameField.text = cdata.avatarName.ToUpper();

            Invoke("Close", duration);
        }
        void Show()
        {

            CancelInvoke();
            isOn = true;
            anim.Play("on");
            panel.SetActive(true);

            if (character == null) { Debug.LogError("Character is null"); Close(); return; }
            TextsData.CharacterData data = Data.Instance.textsData.GetCharactersData(character.data.id);
            if(data==null) { Debug.LogError("TextsData.CharacterData is null"); Close(); return; }
            CharactersData.CharacterData cdata = CharactersData.Instance.GetCharacterData(character.data.id, character.type == Character.types.GOALKEEPER);
            if (cdata == null) { Debug.LogError("CharactersData.CharacterData is null"); Close(); return; }

            thumb.sprite = cdata.thumb;
            nameField.text = cdata.avatarName.ToUpper();

            Invoke("Close", 4);
        }
        void Close()
        {
            CancelInvoke();
            isOn = false;
            anim.Play("off");
            Invoke("Reset", 2);
        }
        private void Reset()
        {
            character = null;
            panel.SetActive(false);
        }
        void OnGameStatusChanged(Fulbo.Game.GameManager.states state)
        {
            if (state == Fulbo.Game.GameManager.states.PLAYING && isOn)
                Close();
        }
    }

}