using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CharacterDialogueUI : MonoBehaviour
    {
        public Text field;
        public Character character;

        public void Init(Character character, string text)
        {
            this.gameObject.SetActive(true);
            CancelInvoke();
            this.character = character;
            this.enabled = true;
            field.text = text;
            Invoke("Reset", Data.Instance.settings.GetSetting("dialoguesDuration"));
        }
        void Update()
        {
            transform.position = character.transform.position;
        }
        void Reset()
        {
            this.gameObject.SetActive(false);
        }
    }
}
