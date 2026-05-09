using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AIHasBallGK : AIState
    {
        float areaLimits_x = 3;
        float direction;
        states state;
        float gkSpeed_sale_z;
        enum states
        {
            IDLE,
            WALKING,
            KICKED,
            DONE
        }
        public override void Init(AI ai)
        {
            type = types.HASBALL;
            base.Init(ai);
            color = Color.yellow;
            gkSpeed_sale_z = Data.Instance.settings.GetSetting("gkSpeed_sale_z");
        }
        public override void SetActive()
        {
            state = states.IDLE;
            if (ai.character.teamID == 1) direction = -1; else direction = 1;
            base.SetActive();
            timer = 0;
            ai.character.SetCollidersOff(2);
            SetLimits(areaLimits_x, gkSpeed_sale_z);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            if (character == ai.character)
                return;

            if (character.teamID != ai.character.teamID)
                SetState(ai.aiAlertGK);
            else
                SetState(ai.aiIdleGK);
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;

            if (state == states.IDLE && timer > 1)
            {
                state = states.WALKING;
                ai.character.states.Stopped();
            }
            else if (state == states.WALKING)
                UpdateWalking();
            else if (state == states.KICKED && timer > 1f)
            {
                state = states.DONE;
                SetState(ai.aiIdleGK);
            }
            return State();
        }
        void UpdateWalking()
        {
            if (IsOutsideAreaInX(_transform.position.x) || timer > 2f)
            {
                CheckSaque();
                timer = 0;
                state = states.KICKED;
            } else ai.character.MoveTo(direction, 0);
        }
        bool IsOutsideAreaInX(float _x)
        {
            if (ai.character.teamID == 1 && _transform.position.x < (ai.originalPosition.x - areaLimits_x)) return true;
            if (ai.character.teamID == 2 && _transform.position.x > (ai.originalPosition.x + areaLimits_x)) return true;
            return false;
        }
        void CheckSaque()
        {
            int rand = Random.Range(0, 9);
            if (rand < 3)
            {
                KickHard();
                return;
            }
            //Vector3 pos = _transform.position + (_transform.forward * 3);
            Character characterToPass = gameManager.charactersManager.GetNearestTo(ai.character, ai.character.teamID, false);
            if (characterToPass == null)
            {
                KickHard();
                return;
            }
            Vector3 otherPos = characterToPass.transform.position;
            ai.character.ballCatcher.LookAt(otherPos);
            ai.character.Kick(CharacterStates.kickTypes.SOFT, Utils.GetRandomFloatBetween(1.5f, 2.5f));
            SetState(ai.aiPositionGK);
        }
        void KickHard()
        {
            ai.character.Kick(CharacterStates.kickTypes.HARD, Utils.GetRandomFloatBetween(0.8f, 1));
            SetState(ai.aiPositionGK);
        }
    }
}