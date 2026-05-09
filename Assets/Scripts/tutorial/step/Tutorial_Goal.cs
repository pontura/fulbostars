using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fulbo.Game.Tutorial
{ 
    public class Tutorial_Goal : TutorialStep
    {
        Character character;
        CharactersManagerTutorial charactersManagerTutorial;
        bool isDone;

        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            this.charactersManagerTutorial = charactersManagerTutorial;
            SetProgress(0);

            character = charactersManagerTutorial.character;
            character.transform.localPosition = new Vector3(18, 0.55f, 0);
            character.states.Stopped();

            Fulbo.Game.GameManager.Instance.cameraInGame.Freeze(new Vector3(23,4.4f,-10.3f));

            character.ai.originalPosition = character.transform.position;

            Fulbo.Game.GameManager.Instance.ball.transform.localPosition = new Vector3(19, 2, 0);

            if (Fulbo.Game.GameManager.Instance.ball.character != null)
                Fulbo.Game.GameManager.Instance.ball.ForceLoseBall(Vector3.right);

            SetInitCharacter(character);
            character.states.LookTo(1);
            SetLimitsTo(new Vector2(17.99f, manager.limits_x.y), manager.limits_y, character);
        }

        public void AddGoalKeeper()
        {
            Vector3 pos = charactersManagerTutorial.character.transform.position;
            pos.x = 29; pos.z = 0;
            Character otherCharacter = charactersManagerTutorial.AddCharacter(pos, 1, true);
            otherCharacter.ai.originalPosition = otherCharacter.transform.position;
            otherCharacter.limits_y = new Vector2(-0.5f, 0.5f);
        }
        public override void OnInit()
        {
            Events.CatchBall += CatchBall;
            Events.OnBallKicked += OnBallKicked;
            Events.OnGoal += OnGoal;
        }
        public override void OnReset()
        {
            Events.CatchBall -= CatchBall;
            Events.OnGoal -= OnGoal;
            Events.OnBallKicked -= OnBallKicked;
        }
        CharacterStates.kickTypes type;
        void OnBallKicked(CharacterStates.kickTypes type, float f, Character ch)
        {
            this.type = type;
            charactersManagerTutorial.StartCoroutine(CheckIfFailed());
        }
        IEnumerator CheckIfFailed()
        {
            if (type == CharacterStates.kickTypes.HARD)
            {
                yield return new WaitForSeconds(1.6f);
                if (isDone)
                    yield return null;
                else
                {
                    isDone = true;
                    Lose();
                }
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
                if (isDone)
                    yield return null;
                else
                {
                    isDone = true;
                    Lose(1);
                }
            }
            Fulbo.Game.GameManager.Instance.ball.ForceLoseBall(Vector3.left);
        }
        void CatchBall(Character character)
        {
            if (character.type == Character.types.GOALKEEPER)
            {
                if (isDone) return; isDone = true;
                Lose();
            } else
                character.states.LookTo(1);
        }
        void OnGoal(int a, Character ch)
        {
            if (isDone) return; isDone = true;
            Done();
        }
    }
}