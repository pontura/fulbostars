using Fulbo.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Fulbo.CharactersData;

namespace Fulbo.UI
{
    public class ReplaceSignal : MonoBehaviour
    {
        [SerializeField] CharacterButton characterButton;
        [SerializeField] Text field;
        [SerializeField] GameObject panel;
        [SerializeField] GameObject panel_replace;
        [SerializeField] GameObject panel_noReplace;
        [SerializeField] Text replaceField1;
        [SerializeField] Text replaceField2;

        void Start()
        {
            Show(false);
            field.text = Data.Instance.texts.Get("replace");
            replaceField1.text = Data.Instance.texts.Get("choose_a_player");
            replaceField2.text = Data.Instance.texts.Get("to_swap");
        }
        public void Init(DBUserData.DBCharacterData dbUserData)
        {
            int id = dbUserData.id;
            characterButton.OnInit(dbUserData,dbUserData.IsGoalkeeper());
            Show(true);
            SetState(true);
        }
        public void Show(bool showIt)
        {
            panel.SetActive(showIt);
        }
        public void SetState(bool canReplace)
        {
            panel_replace.SetActive(canReplace);
            panel_noReplace.SetActive(!canReplace);
        }
    }
}
