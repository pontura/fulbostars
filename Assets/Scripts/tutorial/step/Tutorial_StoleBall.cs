using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fulbo.Game.Tutorial
{
    public class Tutorial_StoleBall : TutorialStep
    {
        Character character;
        float timer;

        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            SetProgress(0);

            character = charactersManagerTutorial.character;
            character.transform.localPosition = new Vector3(-3, 0.55f, -2);

            Vector3 pos = charactersManagerTutorial.character.transform.position;
            pos.x = 5; pos.z = 0;
            Character otherCharacter = charactersManagerTutorial.AddCharacter(pos, 1, false);
            
            character.speed = 4;
            otherCharacter.speed = charactersManagerTutorial.character.speed-2;

            character.ai.originalPosition = character.transform.position;
            otherCharacter.ai.originalPosition = otherCharacter.transform.position;
            otherCharacter.stats.random_jump_a_dash = 0;

            Fulbo.Game.GameManager.Instance.ball.transform.localPosition = new Vector3(4, 2, 0);
            Fulbo.Game.GameManager.Instance.ball.rb.velocity = Vector3.zero;
            Fulbo.Game.GameManager.Instance.cameraInGame.ForcePositionTo(Fulbo.Game.GameManager.Instance.ball.transform);
            Fulbo.Game.GameManager.Instance.cameraInGame.OnSetTarget(Fulbo.Game.GameManager.Instance.ball.transform);
            SetInitCharacter(otherCharacter);

            otherCharacter.characterStats.awareness = 0;
            otherCharacter.ai.SetOn(true);

            SetLimitsTo(manager.limits_x, manager.limits_y, character);
            SetLimitsTo(manager.limits_x, manager.limits_y, otherCharacter);

            otherCharacter.characterColliders.Reset();
        }
        public override void OnInit()
        {
            Events.CharacterCatchBall += CharacterCatchBall;
        }
        public override void OnReset()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
        }
        void CharacterCatchBall(Character character)
        {
            if (character.teamID == 2)
            {
                isDone = true;
                Done();
            }
        }
        IEnumerator BallPassedToDelayed()
        {
            yield return new WaitForSeconds(0.25f);
            Events.OnTutorialProgressAdd(1);
        }
        public override void Done()
        {
            Fulbo.Game.GameManager.Instance.StopAllCoroutines();
            Fulbo.Game.GameManager.Instance.StartCoroutine(DoneC());
            Fulbo.Game.GameManager.Instance.cameraInGame.OnSetTarget(character.transform);
        }
        IEnumerator DoneC()
        {
            yield return new WaitForSeconds(1);
            base.Done();
        }
        bool isDone;
        public override void OnUpdate()
        {
            if (isDone) return;
            base.OnUpdate();
            timer += Time.deltaTime;
            if (timer > 5)
            {
                Lose();
                isDone = true;
            }
        }
    }
}