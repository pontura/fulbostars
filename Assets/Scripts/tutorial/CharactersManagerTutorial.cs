using Fulbo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharactersManagerTutorial : CharactersManager
    {
        [SerializeField] int totalCharactersTeam1 = 0;
        [SerializeField] int totalCharactersTeam2 = 1;
        public Character character;
        public List<Character> others;
        public Fulbo.Game.Tutorial.TutorialStepsManager tutorialsManager;
        public Fulbo.UI.TutorialPopupUI tutorialPopupUI;


        public override void ResetAll()
        {
            Fulbo.Game.GameManager.Instance.isTutorial = true;
            List<Character> team = team1;
            SetCharacters(team1, totalCharactersTeam1, false);
            SetCharacters(team2, totalCharactersTeam2, false);
            Saca(team2);
            SetPlayerOff(referi);
        }
        public override Character GetSacaCharacter(List<Character> team, int who = 0)
        {
            return character;
        }
        void SetCharacters(List<Character> team, int totalCharacters, bool showGoalkeeper)
        {
            int id = 0;
            foreach (Character c in team)
            {
                c.ai.ResetPosition();
                c.states.LookAtBall();
                c.states.Stopped();
                c.gameObject.SetActive(false);

                if (!showGoalkeeper && c.type != Character.types.GOALKEEPER)
                {
                    if (id < totalCharacters)
                    {
                        c.gameObject.SetActive(true);
                        character = c;
                    }
                    else
                    {
                        SetPlayerOff(c);
                    }
                    id++;
                }
            }
        }
        public Character AddCharacter(Vector3 pos, int teamID, bool isGoalkeeper)
        {
            List<Character> team;
            if (teamID == 2) team = team2; else team = team1;

            Character characterToAdd;
            if (isGoalkeeper) characterToAdd = team[0]; else characterToAdd = team[2];

            characterToAdd.transform.position = pos;
            characterToAdd.gameObject.SetActive(true);
            others.Add(characterToAdd);
            return characterToAdd;
        }
        void SetPlayerOff(Character character)
        {
            character.transform.localPosition = new Vector3(1000, character.transform.localPosition.y, 0);
            character.gameObject.SetActive(false);
        }
        public void InitTutorial(System.Action OnDone)
        {
            ResetOthers();
            GetComponent<Fulbo.Input.InputManagerGame>().SetTutorial(tutorialsManager.id);
            ball.transform.position = new Vector3(0, 2, 0);
            tutorialsManager.Init(OnDone);
        }
        void ResetOthers()
        {
            foreach (Character ch in others)
            {
                PathInCharacter p = ch.GetComponent<PathInCharacter>();
                if (p != null)
                    Destroy(p);
                SetPlayerOff(ch);
            }
        }
        public void Kick()
        {
            Character character = Fulbo.Game.GameManager.Instance.ball.character;
            if (character != null)
                character.Kick(CharacterStates.kickTypes.HARD);
        }
        public void Happy()
        {
            Data.Instance.matchData.lastGoalBy = 1;//gana team 1
            character.states.Goal();
            foreach (Character ch in others)
                ch.states.Cry();
        }
        public void Cry()
        {
            Data.Instance.matchData.lastGoalBy = 2;//gana team 2
            character.states.Cry();
            foreach (Character ch in others)
                ch.states.Goal();
        }
        public bool CheckNext()
        {
            if (tutorialsManager.id >= tutorialsManager.GetTotalSteps())
                return false;
            tutorialsManager.Next();
            return true;
        }
    }

}