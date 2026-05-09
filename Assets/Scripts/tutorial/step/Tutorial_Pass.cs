using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fulbo.Game.Tutorial
{
    public class Tutorial_Pass : TutorialStep
    {
        Character character;

        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            SetProgress(5);

            Fulbo.Game.GameManager.Instance.cameraInGame.Freeze(new Vector3(0, 4.4f, -10f));

            character = charactersManagerTutorial.character;
            character.transform.localPosition = new Vector3(2, 0.55f, -2);
            Fulbo.Game.GameManager.Instance.cameraInGame.OnSetTarget(Fulbo.Game.GameManager.Instance.ball.transform);
            Vector3 pos = charactersManagerTutorial.character.transform.position;
            pos.x = -3; pos.z = 2;
            Character otherCharacter = charactersManagerTutorial.AddCharacter(pos, 2, false);
            otherCharacter.speed = charactersManagerTutorial.character.speed;

            character.ai.originalPosition = character.transform.position;
            otherCharacter.ai.originalPosition = otherCharacter.transform.position;

            SetInitCharacter(charactersManagerTutorial.character);


            SetLimitsTo(new Vector2(-7.85f, 7.85f), new Vector2(7.85f, -7.85f), character);
            SetLimitsTo(new Vector2(-7.85f, 7.85f), new Vector2(7.85f, -7.85f), otherCharacter);
        }
        public override void OnInit()
        {
            Events.BallPassedTo += BallPassedTo;
        }
        public override void OnReset()
        {
            Events.BallPassedTo -= BallPassedTo;
        }
        void BallPassedTo(Character character)
        {
            if (timer > 0.5f)
            {
                Fulbo.Game.GameManager.Instance.StopAllCoroutines();
                Fulbo.Game.GameManager.Instance.StartCoroutine(BallPassedToDelayed());
            }
        }
        IEnumerator  BallPassedToDelayed()
        {
            yield return new WaitForSeconds(0.25f);
            Events.OnTutorialProgressAdd(1);
        }
        public override void Done()
        {
            base.Done();
        }
    }
}