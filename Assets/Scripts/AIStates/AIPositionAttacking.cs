using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AIPositionAttacking : AIState
    {
        public Vector3 dest;
        public bool isHelper; // lo sigue al qeu tiene la pelota
        public bool yourGoalkeeperHasBall;
        Vector3 ballPos;
        float delay;

        public override void Init(AI ai)
        {
            type = types.ATTACKING;
            delay = Data.Instance.settings.gameplay.attackDelay;
            _transform = ai.transform;
            dest = ai.originalPosition;
            base.Init(ai);
            color = Color.grey;
            timer = 0;
        }
        public override void SetActive()
        {
            isHelper = false;
            timer = 0;
            SetDestination();
            if (ai.ball.character != null && ai.ball.character.type == Character.types.GOALKEEPER && ai.ball.character.teamID == ai.character.teamID)
                yourGoalkeeperHasBall = true;
            else
                yourGoalkeeperHasBall = false;
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBall);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            isHelper = false;
            if (character.teamID != ai.character.teamID)
                SetState(ai.aiPositionDefending);
        }
        public override AIState UpdatedByAI()
        {
            if (timer > delay)
                SetDestination();

            timer += Time.deltaTime;

            Move(dest, true);
            return State();
        }
        public override void GotoBall()
        {
            SetState(ai.aiGotoBall);
        }

        public virtual void SetDestination()
        {
            if (gameManager.isTutorial)
            {
                dest = ai.originalPosition;
                return;
            }
            timer = 0;

            ballPos = ai.ball.transform.position;

            //por si no detectó que tiene la pelota:
            if (ai.ball.character != null && ai.ball.character == ai.character)
                SetState(ai.aiHasBall);
            else
                UpdateAttackPosition();
        }
        void UpdateAttackPosition()
        {
            Character characterWithBall = ai.ball.character;
            if (ai.character.type == Character.types.FOR && Random.Range(0, 10) < 3)
                ai.character.SuperRun();

            float lerpValue = 0.5f;
            dest = ai.originalPosition;

            dest.z = Mathf.Lerp(dest.z, _ballTransform.position.z, lerpValue / 8);
            dest.z += Utils.GetRandomFloatBetween(-2, 2);

            float randomX;
            Vector3 opponentPos = ai.character.oponent.transform.position;
            switch (ai.character.type)
            {
                case Character.types.DEF:

                    dest.z += Utils.GetRandomFloatBetween(-2, 2);
                    if (ai.character.teamID == 1 && opponentPos.x > ai.transform.position.x)
                    { 
                        dest.x = opponentPos.x + Random.Range(2, 4);
                        return;
                    }
                    else if (ai.character.teamID == 2 && opponentPos.x < ai.transform.position.x)
                    { 
                        dest.x = opponentPos.x - Random.Range(2, 4);
                        return;
                    }
                    else
                    {
                        randomX = Utils.GetRandomFloatBetween(-1, 1);
                        if (ai.character.teamID == 1)
                            dest.x -= ai.stadiumDataSizeX / 8;
                        else
                            dest.x += ai.stadiumDataSizeX / 8;
                        lerpValue = Utils.GetRandomFloatBetween(0.6f, 0.8f); break;
                    }
                case Character.types.MID:
                    randomX = Utils.GetRandomFloatBetween(-3, 3);
                    if (ai.character.teamID == 1) dest.x -= ai.stadiumDataSizeX / 4;
                    else dest.x += ai.stadiumDataSizeX / 3;
                    lerpValue = Utils.GetRandomFloatBetween(0.5f, 0.7f); break;
                default:
                    randomX = Utils.GetRandomFloatBetween(-2, 2);
                    lerpValue = Utils.GetRandomFloatBetween(0.3f, 0.5f);
                    if(ai.character.teamID == 1) dest.x -= ai.stadiumDataSizeX / 2;
                    else dest.x += ai.stadiumDataSizeX / 2;

                    if (ai.ball.transform.position.x > Mathf.Abs(ai.stadiumDataSizeX / 2 - 7))
                    {
                        dest.z = Random.Range(-2, 2);
                    }
                    else if (ai.ball.transform.position.x > Mathf.Abs(ai.stadiumDataSizeX / 2 - 10))
                    {
                        dest.z = ai.originalPosition.z / 3f;
                    }                    
                    else
                    {
                        dest.z = Mathf.Lerp(dest.z, _ballTransform.position.z, lerpValue / 8);
                        dest.z += Utils.GetRandomFloatBetween(-2, 2);
                    }
                    break;
            }

            int rand = Random.Range(0, 10);
            if (ai.character.type == Character.types.FOR && characterWithBall != null && Mathf.Abs(characterWithBall.transform.position.x) > Mathf.Abs(ai.transform.position.x) && rand < 8)
                ai.character.SuperRun();
            else if (ai.character.type == Character.types.MID && rand < 3)
                ai.character.SuperRun();

            dest.x = Mathf.Lerp(dest.x, _ballTransform.position.x, lerpValue);
            dest.x += randomX;           

            if (yourGoalkeeperHasBall)
            {
                if(ai.character.type == Character.types.DEF) // avanza un poco a los defensores:
                    dest.x *= 1.15f;
            }

            if (Vector3.Distance(dest, ballPos) < 4)
                Opposite_Z();
        }
        void Opposite_Z()
        {
            if ((ballPos.z > 0 && dest.z > 0) || (ballPos.z < 0 && dest.z < 0))
                dest.z *= -0.95f;
        }
        
    }

}