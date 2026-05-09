using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.UI
{
    public class CharactersSelector : MonoBehaviour
    {
        //public Text field;
        public Transform container;
        public CharacterButton button;
        public List<Button> tabs;

        int tabID = 0;
        List<int> myTeam;

        void Start()
        {
            Data.Instance.ui.SetBackButton(true, Ready);
            LoadCharacters();
            SetTotals();
        }
        void LoadCharacters()
        {
            Utils.RemoveAllChildsIn(container);

            List<CharactersData.CharacterData> allCharacters = new List<CharactersData.CharacterData>();

            if (tabID == 0)
            {
                allCharacters = CharactersData.Instance.all;
            }
            else if (tabID == 1)
            {
                allCharacters = CharactersData.Instance.all_goalkeepers;
            }
            int id = 1;
            foreach (CharactersData.CharacterData data in allCharacters)
            {
                if (HasThisCharacter(id))
                    AddButton(data, id);
                id++;
            }
        }
        bool HasThisCharacter(int id)
        {
            foreach (int myteamID in myTeam)
            {
                if (id == myteamID)
                    return true;
            }
            return false;
        }
        void AddButton(CharactersData.CharacterData data, int id)
        {
            CharacterButton b = Instantiate(button, container);
            b.Init(0, null);
            b.OnInitCharacterData(data);
            id++;
        }
        public void OnTabClicked(int _tabID)
        {
            if (_tabID == tabID) return;
            tabID = _tabID;
            LoadCharacters();
        }
        void OnCharacterBigClose()
        {
            Data.Instance.ui.SetBackButton(true, OnBackToScreen);
        }
        void OnBackToScreen()
        {
            Data.Instance.ui.SetBackButton(true, Ready);
        }
        void SetTotals()
        {
            int totalCharacter = DB.DBManager.Instance.DbUserData.data.players_characters.Count;
            int totalGoalkeepers = DB.DBManager.Instance.DbUserData.data.players_goalkeepers.Count;

            tabs[0].GetComponentInChildren<Text>().text = "JUGADORES (" + totalCharacter + ")";
            tabs[1].GetComponentInChildren<Text>().text = "ARQUEROS (" + totalGoalkeepers + ")";
            // tabs[2].GetComponentInChildren<Text>().text = "EQUIPO (" + (Data.Instance.myTeam.characters.Count + Data.Instance.myTeam.goalkeepers.Count) + ")";
        }
        public void Ready()
        {
            //Data.Instance.myTeam.Save();
            Data.Instance.LoadLevel("MainMenu");
        }
    }
}