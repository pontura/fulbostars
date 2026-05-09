using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AiGotoForcePosition : AIState
    {
        Vector3 dest;

        public override void Init(AI ai)
        {
            type = types.GOTO_FORCE_POSITION;
            base.Init(ai);
            color = Color.cyan;
        }
        public override void SetActive()
        {
            SetDest();
            timer = 0;
        }
        public override AIState UpdatedByAI()
        {
            timer += Time.deltaTime;
            
            int _x = 0;
            int _z = 0;
            if (Mathf.Abs(ai.transform.position.x - dest.x) > 0.2f)
            {
                if (ai.transform.position.x < dest.x) _x = 1; else _x = -1;
            }
            if (Mathf.Abs(ai.transform.position.z - dest.z) > 0.2f)
            {
                if (ai.transform.position.z < dest.z) _z = 1; else _z = -1;
            }

            ai.character.MoveTo(_x, _z);

            if (timer > 2)
                SetState(ai.aiIdle);

            return State();
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBallGK);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            SetState(ai.aiIdle);
        }
        void SetDest()
        {
            dest = ai.originalPosition;
        }
    }
}