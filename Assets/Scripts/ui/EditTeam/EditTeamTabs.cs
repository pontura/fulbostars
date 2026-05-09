using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.EditTeam
{
    public class EditTeamTabs : MonoBehaviour
    {
        [SerializeField] EditTeamScreen editTeamScreen;
        [SerializeField] Transform container;
        [SerializeField] List<ButtonCustom> buttons;

        [SerializeField] Sprite off;
        [SerializeField] Sprite on;

        public void Awake()
        {
            editTeamScreen = GetComponent<EditTeamScreen>();
            int id = 1;
            foreach (ButtonCustom b in container.GetComponentsInChildren<ButtonCustom>())
            {
                buttons.Add(b);
                b.Init(id, OnClicked, Data.Instance.texts.Get("editTeam_button" + id));
                id++;
            }            
        }
        public void Init()
        {
            OnClicked(1);
        }
        public void OnClicked(int id)
        {
            foreach (ButtonCustom b in buttons)
                b.OnSelected(false);

            buttons[id-1].OnSelected(true);

            AudioManager.Instance.PlaySoundOneShot("ui", "_new/ui/hit");
            if (id == 1)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.SHAPES, Data.Instance.clubsData.shapes.Length);
            else if (id == 2)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.PATTERNS, Data.Instance.clubsData.patterns.Length);
            else if (id == 3)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.LOGO, Data.Instance.clubsData.logos.Length, true);
            else if (id == 4)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.COLOR1, Data.Instance.settings.colorStyles.Length);
            else if (id == 5)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.COLOR2, Data.Instance.settings.colorStyles.Length);
            else if (id == 6)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.COLOR3, Data.Instance.settings.colorStyles.Length);
            else if (id == 7)
                editTeamScreen.LoadButtons(EditTeamButton.typeButton.COLOR4, Data.Instance.settings.colorStyles.Length);
        }
    }
}
