using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fulbo.Game.Tutorial
{ 
    public class Tutorial_Center : TutorialStep
    {
        Character character;
        CharactersManagerTutorial charactersManagerTutorial;

        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            this.charactersManagerTutorial = charactersManagerTutorial;
            SetProgress(0);

            character = charactersManagerTutorial.character;
            character.transform.localPosition = new Vector3(25, 0.55f, -13);
            character.states.Stopped();

            // Fulbo.Game.GameManager.Instance.cameraInFulbo.Game.GameManager.ForcePositionTo(Fulbo.Game.GameManager.Instance.ball.transform);
            GameManager.Instance.cameraInGame.Unfreeze();

            character.ai.originalPosition = character.transform.position;
            Vector3 ballPos = character.transform.localPosition;
            ballPos.z += 1;
            Fulbo.Game.GameManager.Instance.ball.transform.localPosition = ballPos;

            if (Fulbo.Game.GameManager.Instance.ball.character != null)
                Fulbo.Game.GameManager.Instance.ball.ForceLoseBall(Vector3.right);

            character.states.LookTo(1);

            Vector3 pos = character.transform.position;
            pos.x = 25; pos.z = 3;
            Character otherCharacter = charactersManagerTutorial.AddCharacter(pos, 2, false);
            otherCharacter.ai.originalPosition = otherCharacter.transform.position;
            SetInitCharacter(character);            

            SetLimitsTo(new Vector2(17.99f, manager.limits_x.y), manager.limits_y, character);
            SetLimitsTo(new Vector2(17.99f, manager.limits_x.y), manager.limits_y, otherCharacter);
        }
        public void AddGoalKeeper()
        {
            Vector3 pos = charactersManagerTutorial.character.transform.position;
            pos.x = 29; pos.z = 0;
            Character otherCharacter = charactersManagerTutorial.AddCharacter(pos, 1, true);
            otherCharacter.ai.originalPosition = otherCharacter.transform.position;
        }
        public override void OnInit()
        {
            Events.OnGoal += OnGoal;
        }
        public override void OnReset()
        {
            Events.OnGoal -= OnGoal;
        }
        void OnGoal(int a, Character ch)
        {
            Done();
        }
    }
}