using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fulbo.Game;
using UnityEngine.InputSystem.HID;

namespace Fulbo.Dashoard
{
    public class DashboardSignalContent : MonoBehaviour
    {
        public DashboardContentData.types type;
        public Text title;
        public Text text;
        public Image thumb1;
        public Image thumb2;
        public Image bg;
        public Image icon;
        DashboardContentData data;
        [SerializeField] GameObject copete;
        int characterID;
        List<int> randomCharacters;

        public void Init(DashboardContentData data)
        {
            characterID = data.characterID - 1;
            this.data = data;
            print("title: " + data.title);
            print("title: " + data.text);
            title.text = ParseSpecialVars(data.title);
            if (text != null)
                text.text = ParseSpecialVars(data.text);

            if (copete != null)
            {
                if (data.copete == "")
                    copete.SetActive(false);
                else
                    copete.GetComponentInChildren<Text>().text = data.copete;
            }
            if (data.characterID2 != 0 && thumb2 != null)
                thumb2.sprite = CharactersData.Instance.GetCharacterData(data.characterID2, false).thumb;

            if (thumb1 != null)
                thumb1.sprite = CharactersData.Instance.GetCharacterData(data.characterID, false).thumb;

            if (bg != null)
                bg.color = data.color;
        }
        public string ParseSpecialVars(string text)
        {
            randomCharacters = CharactersData.Instance.GetAvailablePlayersID(false);
            //randomCharacters = CharactersData.Instance.GetCharactersIDByRarity(UI.FigusData.rarities.NORMALITO, false);
            string teamLevel;
            string myTeamName;

            if (Data.Instance.isMobile)
            {
                myTeamName = Data.Instance.myTeam.teamName;
                teamLevel = CupsData.Instance.GetActualLevel().clubData.name_abr;
            }
            else
            {
                myTeamName = Data.Instance.clubsData.GetData(2).name_abr;
                teamLevel = Data.Instance.clubsData.GetData(1).name_abr;
            }
            if (text == null) return "";
            if (text.Length > 200) return "";
            if (text.Contains("[fecha]"))
                text = text.Replace("[fecha]", System.DateTime.Now.ToShortDateString());
            if (text.Contains("[1]"))
            {
                string avatarName = CharactersData.Instance.GetCharacterData(data.characterID, false).avatarName;
                text = text.Replace("[1]", avatarName);
            }
            if (text.Contains("[2]"))
            {
                string avatarName = CharactersData.Instance.GetCharacterData(data.characterID2, false).avatarName;
                text = text.Replace("[2]", avatarName);
            }

            if (text.Contains("[result]"))
                text = text.Replace("[result]", Data.Instance.matchData.score.y + " - " + Data.Instance.matchData.score.x);

            //if (text.Contains("[sobres]"))
            //    text = text.Replace("[sobres]", Data.Instance.matchData.totalFigusWon.ToString());

            if (text.Contains("[random]"))
            {
                int chID = randomCharacters[UnityEngine.Random.Range(0, randomCharacters.Count)];
                string avatarName = CharactersData.Instance.GetCharacterData(chID, false).avatarName;
                text = text.Replace("[random]", avatarName);
            }

            if (text.Contains("[team2]"))
            {
                if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                    text = text.Replace("[team2]", myTeamName);
                else
                    text = text.Replace("[team2]", teamLevel);
            }
            if (text.Contains("[team1]"))
            {
                if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                    text = text.Replace("[team1]", Data.Instance.myTeam.teamName);
                else
                    text = text.Replace("[team1]", teamLevel);
            }
            if (text.Contains("[goleador_team1]"))
            {
                if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                    text = text.Replace("[goleador_team1]", SetRealCharacter(GetGoleadorForTeam(1)));
                else
                    text = text.Replace("[goleador_team1]", SetRealCharacter(GetGoleadorForTeam(2)));
            }
            if (text.Contains("[goleador_team2]"))
            {
                if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
                    text = text.Replace("[goleador_team2]", SetRealCharacter(GetGoleadorForTeam(2)));
                else
                    text = text.Replace("[goleador_team2]", SetRealCharacter(GetGoleadorForTeam(1)));
            }
            return text;
        }
        int GetGoleadorForTeam(int teamID)
        {
            int id = 0;
            if (teamID == 1 &&  Data.Instance.matchData.team1_goals.Count > 0)
                id = Data.Instance.matchData.team1_goals[0];
            else  if(  Data.Instance.matchData.team2_goals.Count > 0)
                id = Data.Instance.matchData.team2_goals[0];
            return id;
        }
        string SetRealCharacter(int id)
        {
            characterID = id - 1;
            return CharactersData.Instance.GetCharacterData(characterID, false).avatarName;
        }
    }
}