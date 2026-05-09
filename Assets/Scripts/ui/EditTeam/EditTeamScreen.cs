using Fulbo.DB;
using Fulbo.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI.EditTeam
{
    public class EditTeamScreen : MonoBehaviour
    {
        public GameObject asset_for_shoes;
        public GameObject asset_for_shorts;

        [SerializeField] ButtonCustom submitButton;
        [SerializeField] EditTeamTabs tabs;
        [SerializeField] Image[] iconsColorize;
        [SerializeField] EditTeamButton button;
        [SerializeField] List<EditTeamButton> buttons;
        [SerializeField] ClubShield clubShield;
        [SerializeField] ClubShield clubShield_to_add;
        [SerializeField] Transform container;
        [SerializeField] CharacterForCamera characterForCamera;
        [SerializeField] InputField inputField;
        [SerializeField] InputField twitterField;
        [SerializeField] InputField discordField;

        [SerializeField] CustomizablePart[] allCustomizations;
        [SerializeField] CustomizableDesign[] allCustomizationsDesigns;
        [SerializeField] ButtonCustom submitNameButton;


        Animation anim;

        string teamName;
        bool isTutorial;

        private void Start()
        {
            submitButton.Init(0, Back, "NEXT");
            submitButton.gameObject.SetActive(true);
        }
        private void OnEnable()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
        private void OnDisable()
        {
            CancelInvoke();
        }
        public void Init()
        {
            anim = GetComponent<Animation>();
            anim.Play("in");

            teamName = Data.Instance.myTeam.teamName;
            tabs = GetComponent<EditTeamTabs>();

            //if (Data.Instance.myTeam.teamName != null && Data.Instance.myTeam.teamName.Length > 1)
            //{
                submitButton.gameObject.SetActive(true);
                Data.Instance.ui.SetBackButton(true, Back);
            //}
            //else
            //{
            //    Data.Instance.ui.SetBackButton(false, Back);
            //    submitButton.gameObject.SetActive(false);
            //}
            if (DBManager.Instance.DbUserData.data.twitter != null)
                twitterField.text = DBManager.Instance.DbUserData.data.twitter;
            if (DBManager.Instance.DbUserData.data.discord != null)
                discordField.text = DBManager.Instance.DbUserData.data.discord;

            tabs.Init();
            LoadCharacter();
            clubShield.Init();

            if (teamName != "")
                inputField.text = teamName;
            else if (DBManager.Instance.SocialUserName != "")
                inputField.text = DBManager.Instance.SocialUserName;

            CancelInvoke();
            // LoopForChanges();
            submitNameButton.SetInteraction(true);

            submitNameButton.Init(0, SubmitName, Data.Instance.texts.Get("save").ToUpper());
        }
        public void OpenCustomizer()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "_new/ui/click3");
        }
        void LoadCharacter()
        {
            if (CharactersData.Instance.all.Count > 0)
            {
                CharactersData.CharacterData data = CharactersData.Instance.GetCharacterData(1, false);
                characterForCamera.Init(data, "idle");
                allCustomizations = characterForCamera.GetComponentsInChildren<CustomizablePart>();
                allCustomizationsDesigns = characterForCamera.GetComponentsInChildren<CustomizableDesign>();
            }
            else
            {
                Invoke("LoadCharacter", 0.1f);
            }
        }
        public void LoadButtons(EditTeamButton.typeButton type, int total, bool addZero = false)
        {
            Utils.RemoveAllChildsIn(container);
            int id = 1;
            if (addZero)
            {
                total++;
                id = 0;
            }
            int _id = 0;
            buttons = new List<EditTeamButton>();
            for (int a = 0; a < total; a++)
            {
                ClubData clubData = NewClubData();
                bool initSelected = false;
                switch (type)
                {
                    case EditTeamButton.typeButton.COLOR1:
                        clubData.clubColor1 = id;
                        initSelected = Data.Instance.myTeam.clubData.clubColor1 == id;
                        break;
                    case EditTeamButton.typeButton.COLOR2:
                        clubData.clubColor2 = id;
                        initSelected = Data.Instance.myTeam.clubData.clubColor2 == id;
                        break;
                    case EditTeamButton.typeButton.COLOR3:
                        clubData.clubColor3 = id;
                        initSelected = Data.Instance.myTeam.clubData.clubColor3 == id;
                        break;
                    case EditTeamButton.typeButton.COLOR4:
                        clubData.clubColor4 = id;
                        initSelected = Data.Instance.myTeam.clubData.clubColor4 == id;
                        break;
                    case EditTeamButton.typeButton.PATTERNS:
                        clubData.designID = id;
                        initSelected = Data.Instance.myTeam.clubData.designID == id;
                        break;
                    case EditTeamButton.typeButton.LOGO:
                        clubData.logo = id;
                        initSelected = Data.Instance.myTeam.clubData.logo == id;
                        break;
                    case EditTeamButton.typeButton.SHAPES:
                            clubData.shieldDesignID = id;
                            initSelected = Data.Instance.myTeam.clubData.shieldDesignID == id;
                            break;
                }
                AddByType(_id, clubData, type, initSelected);
                id++;
                _id++;
            }
            RefreshColors();
        }
        void RefreshColors()
        {
            iconsColorize[0].color = Data.Instance.settings.GetColorByIndex(Data.Instance.myTeam.clubData.clubColor1);
            iconsColorize[1].color = Data.Instance.settings.GetColorByIndex(Data.Instance.myTeam.clubData.clubColor2);
            iconsColorize[2].color = Data.Instance.settings.GetColorByIndex(Data.Instance.myTeam.clubData.clubColor3);
            iconsColorize[3].color = Data.Instance.settings.GetColorByIndex(Data.Instance.myTeam.clubData.clubColor4);
        }
        ClubData NewClubData()
        {
            ClubData clubData = new ClubData();
            clubData.name_abr = Data.Instance.myTeam.clubData.name_abr;
            clubData.shieldDesignID = Data.Instance.myTeam.clubData.shieldDesignID;
            clubData.clubColor1 = Data.Instance.myTeam.clubData.clubColor1;
            clubData.clubColor2 = Data.Instance.myTeam.clubData.clubColor2;
            clubData.clubColor3 = Data.Instance.myTeam.clubData.clubColor3;
            clubData.clubColor4 = Data.Instance.myTeam.clubData.clubColor4;
            clubData.designID = Data.Instance.myTeam.clubData.designID;
            clubData.logo = Data.Instance.myTeam.clubData.logo;
            return clubData;
        }
        void AddByType(int id, ClubData clubData, EditTeamButton.typeButton type, bool initSelected)
        {
            EditTeamButton b = Instantiate(button, container);            
            b.OnInit(this, type, clubData, clubShield_to_add);
            ButtonCustom _button = b.GetComponent<ButtonCustom>();
            _button.Init(id, OnSelect);
            buttons.Add(b);
            if (initSelected)
                _button.OnSelected(true);
        }
        public void OnSelect(int id)
        {
            foreach(EditTeamButton eb in buttons)
                eb.GetComponent<ButtonCustom>().OnSelected(false);
            buttons[id].GetComponent<ButtonCustom>().OnSelected(true);

            ClubData _clubData = buttons[id].clubData;
            Data.Instance.myTeam.clubData = _clubData;
            clubShield.Init(_clubData);
            RefreshCustomizations();
        }
        void RefreshCustomizations()
        {
            foreach (CustomizablePart cp in allCustomizations)
                cp.Refresh();
            foreach (CustomizableDesign cd in allCustomizationsDesigns)
                cd.Refresh();

            RefreshColors();
        }
        public void SubmitName(int buttonID)
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "ui/click");
            if(inputField.text == "")
            {
                string text = Data.Instance.texts.Get("initial1_giveName");
                Events.OnPopup(text, null);
                return;
            }
            Data.Instance.myTeam.SetTeamName(inputField.text);
            if (DBManager.Instance.DbUserData.type == DBUserData.types.REGISTERED)
            {
                DBRegisterTeam.RData d = new DBRegisterTeam.RData();
                d.user = inputField.text;
                d.email = DBManager.Instance.Email;
                d.twitter = twitterField.text;
                d.discord = discordField.text;
                DBManager.Instance.UpdateTwitter(twitterField.text);
                DBManager.Instance.UpdateDiscord(discordField.text);
                DBEvents.OnRegisterTeam(d, OnNameSaved);
            }
            else
            {
                DBUserData.UserData d = new DBUserData.UserData();
                d.user = inputField.text;
                d.twitter = twitterField.text;
                d.discord = discordField.text;
                DBEvents.SaveUserData(d, OnNameSaved);
            }
            submitNameButton.SetInteraction(false);
        }
        void OnNameSaved(bool isOK, string response)
        {
            if(!isOK)
            {
                string text = Data.Instance.texts.Get("teamNameExists");
                Events.OnPopup(response, null);
            }
            else
            {
                Back();
            }
            submitNameButton.SetInteraction(true);
        }
        void Back(int i)
        {
            Back();
        }
        public void Back()
        {
            AudioManager.Instance.PlaySoundOneShot("ui", "_new/ui/clickPlay");
            if (Data.Instance.myTeam.teamName != null && Data.Instance.myTeam.teamName.Length > 1)
            {
                if (discordField.text != DBManager.Instance.DbUserData.data.discord ||
                    twitterField.text != DBManager.Instance.DbUserData.data.twitter ||
                    Data.Instance.myTeam.teamName != DBManager.Instance.DbUserData.data.user ||
                    DBManager.Instance.DbUserData.GetFormatedStyle() != DBManager.Instance.DbUserData.data.style)
                {
                    Debug.Log("______________Save changes on Back");
                    DB.DBUserData.UserData data = new DBUserData.UserData();

                    data.discord = discordField.text;
                    data.twitter = twitterField.text;
                    data.user = Data.Instance.myTeam.teamName;
                    data.shortName = Data.Instance.myTeam.clubData.name_abr;
                    data.style = DBManager.Instance.DbUserData.GetFormatedStyle();
                    DBEvents.SaveUserData(data, null);
                }

                //if(DB.DBManager.Instance.DbUserData.data.gameData.tutorialStep<=2)
                //{
                //    DB.DBManager.Instance.DbGameData.Put("tutorialStep", "3", null);
                //    Events.OnPopup(Data.Instance.texts.Get("tutorialWelcome"), OnTutorialDone);
                //}
                //else
                //{
                    Events.Back();
                    anim.Play("out");
                    Invoke("Reset", 0.5f);
               // }

                submitButton.gameObject.SetActive(false);
            }
            else
            {
                string text = Data.Instance.texts.Get("initial1_giveName"); 
                Events.OnPopup(text, null);
            }
        }
        void OnTutorialDone()
        {
            Data.Instance.LoadLevel("Tutorial");
        }
        //void LoopForChanges()
        //{
        //    Invoke("LoopForChanges", 0.2f);
        //    if ((inputField.text != "" && inputField.text != Data.Instance.myTeam.teamName))
        //        submitNameButton.SetInteraction(true);
        //    else
        //        submitNameButton.SetInteraction(false);
        //}
        private void Reset()
        {
            gameObject.SetActive(false);
        }
    }
}
