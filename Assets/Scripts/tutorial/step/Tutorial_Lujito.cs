using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fulbo.Game.Tutorial
{
    public class Tutorial_Lujito : TutorialStep
    {
        Character character;
        float timer;
        Character otherCharacter;

        public override void Setup(CharactersManagerTutorial charactersManagerTutorial, GameObject asset)
        {
            SetProgress(0);

            character = charactersManagerTutorial.character;
            character.transform.localPosition = new Vector3(-4, 0.55f, 0);

            Vector3 pos = charactersManagerTutorial.character.transform.position;
            pos.x = 7; pos.z = 0;
            otherCharacter = charactersManagerTutorial.AddCharacter(pos, 1, false);

            character.speed = 6;
            otherCharacter.speed = 8;

            character.ai.originalPosition = character.transform.position;
            otherCharacter.ai.originalPosition = otherCharacter.transform.position;

            Fulbo.Game.GameManager.Instance.ball.transform.localPosition = new Vector3(-3.25f, 0, 0);
            Fulbo.Game.GameManager.Instance.ball.Reset();
            Fulbo.Game.GameManager.Instance.cameraInGame.ForcePositionTo(Fulbo.Game.GameManager.Instance.ball.transform);
            Fulbo.Game.GameManager.Instance.cameraInGame.OnSetTarget(Fulbo.Game.GameManager.Instance.ball.transform);
            SetInitCharacter(character);

            SetLimitsTo(manager.limits_x, manager.limits_y, character);
            SetLimitsTo(manager.limits_x, manager.limits_y, otherCharacter);

            otherCharacter.characterStats.awareness = 100;
            otherCharacter.characterColliders.Reset();
            otherCharacter.ai.ResetAll();
            otherCharacter.states.Stopped();
            otherCharacter.oponent = character;
            otherCharacter.ai.SetOn(true);
        }
        public override void OnInit()
        {
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.Lujito += Lujito;
        }
        public override void OnReset()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.Lujito -= Lujito;
        }
        void Lujito()
        {
            float dist = Vector3.Distance(character.transform.position, otherCharacter.transform.position);
            if (dist < 3f)
            {
                otherCharacter.characterColliders.SetCollidersOff(2);
                otherCharacter.ai.SetOn(false);
                otherCharacter.states.Dash();
                isDone = true; Done();
            }
        }
        void CharacterCatchBall(Character character)
        {
            if (character.teamID == 1)
            {
                otherCharacter.states.Goal();
                isDone = true;
                Lose();
            }
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