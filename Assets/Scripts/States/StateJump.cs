using System.Collections;
using UnityEngine;
namespace Fulbo.Game.States
{
    public class StateJump : StateCharacter
    {
        bool isGoalkeeper;
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            isGoalkeeper = states.character.type == Character.types.GOALKEEPER;
            if (isGoalkeeper)
                type = CharacterStates.types.JUMP_ATAJA;
            else
                type = CharacterStates.types.JUMP;
        }
        public override void SetActive()
        {
            base.SetActive();
            
            float duration;
            Character character = states.character;
            float offsetColliders_y = 1.5f;
            if (isGoalkeeper)
            {
                if(Mathf.Abs(states.ball.transform.position.z-character.transform.position.z)<0.75f)
                    animName = "jump"; // SALTA PARA ARRIBA
                else
                    animName = "jump2"; // SALTA DIAGONAL
                offsetColliders_y = 0.9f;
                duration = 0.7f;
            }
            else
            {
                animName = "jump";
                character.stats.GetJumpHeight(character);
                offsetColliders_y = 0.35f;
                duration = 0.7f;
            }
            character.MoveCollidersTo(new Vector3(0, offsetColliders_y, 0));
            states.PlayAnim(animName, duration, EndAction, true);
            AudioManager.Instance.PlaySoundOneShot("shouts", "ingame/game_jump", false);
            int rand = Random.Range(0, 3);
            if (rand > 0)
                AudioManager.Instance.PlaySoundOneShot("common", "ingame/voices/game_vox_effort" + rand, false);
            else
                AudioManager.Instance.PlaySoundOneShot("common", "ingame/voices/game_vox_hey", false);
        }
        public override void Move(float speed)
        {
            if (isGoalkeeper && states.ball.character != null && states.ball.character == states.character)
                SetState(states.run);
        }
        void EndAction()
        {
            states.character.MoveCollidersTo(Vector3.zero);
            if(states.character.type == Character.types.GOALKEEPER)
            {
                bool hasBall = states.ball.character == states.character;
                if ((animName == "jump" || animName == "jump2") && !hasBall)
                {
                    SetState(states.freeze);
                    return;
                }
            }
            SetState(states.idle);
        }
        public override void Stopped()
        {
            //if (animName == "freeze")
            //    SetState(states.idle);
            //else if (states.character.type == Character.types.GOALKEEPER && animName == "jump" || animName == "jump2")
            //    SetState(states.idle);
        }
        public override void OnCatchBall()
        {
            if (isGoalkeeper)
                return;
            SetState(states.idle);
        }
        public override void Dash() { }
        public override void Jueguito() { }
        public override void Lujito() { }
        
    }
}