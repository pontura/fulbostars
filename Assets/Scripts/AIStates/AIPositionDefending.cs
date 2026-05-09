using UnityEngine;
namespace Fulbo.Game.AIs
{
    public class AIPositionDefending : AIState
    {
        Vector3 dest;
        float delay;
        int direction;
        public override void Init(AI ai)
        {
            type = types.DEFENDING;
            delay = Data.Instance.settings.gameplay.defenseDelay/1.5f;
            dest = ai.originalPosition;
            base.Init(ai);
            color = Color.white;
            if (ai.character.teamID == 1) direction = 1; else direction = -1;
        }
        public override void SetActive()
        {
            timer = delay;
            SetDestination();
        }
        public override void OnCatchBall()
        {
            SetState(ai.aiHasBall);
        }
        public override void OnCharacterCatchBall(Character character)
        {
            if (character.teamID == ai.character.teamID)
                SetState(ai.aiPositionAttacking);            
        }
        public override AIState UpdatedByAI()
        {
            if (timer > delay)
            {
                timer = 0;
                Character characterWithBall = ai.ball.character;
                SetDestination();
                if (characterWithBall != null)
                { 
                    if (characterWithBall == ai.character.oponent)
                    {
                        SetState(ai.aiGotoBall);
                        return State();
                    } else if(characterWithBall.teamID == ai.character.teamID)
                    {
                        SetState(ai.aiPositionAttacking);
                        return State();
                    } else if(ai.character.type == Character.types.DEF)
                    {
                        // si el rival con pelota está más adelante que su marca se le va al humo:
                        //if (Vector3.Distance(ai.transform.position, characterWithBall.transform.position )<4
                        //    &&
                        //    Mathf.Abs( ai.character.oponent.transform.position.x)< Mathf.Abs(characterWithBall.transform.position.x))
                        //{
                        //    SetState(ai.aiGotoBall);
                        //}
                        return State();
                    }
                }
            }

            timer += Time.deltaTime;
           // float speed = SetSpeed();
            Move(dest, true);
            return State();
        }
        //float SetSpeed()
        //{
        //    float speed = 1;
        //    if (ai.character.type == Character.types.FOR)
        //        return speed;

        //    if (ai.character.teamID == 1 && _ballTransform.position.x + 2 < _transform.position.x
        //        || ai.character.teamID == 2 && _ballTransform.position.x - 2 > _transform.position.x)
        //            speed = 0.25f;
        //    return speed;
        //}
        public override void GotoBall()
        {
            SetState(ai.aiGotoBall);
        }
        public virtual void SetDestination()
        {
            int rand = Random.Range(0, 10);

            Vector3 opponentPos = ai.character.oponent.transform.position;

            if (ai.character.type == Character.types.DEF && rand < 8)
                ai.character.SuperRun();
            else if (ai.character.type == Character.types.MID && rand < 5)
                ai.character.SuperRun();

          // Vector3 ballPos = ai.ball.transform.position;

            Vector2 offSet_x = Vector2.zero;

            dest.z = opponentPos.z * (Random.Range(0.8f, 1f));

            if (ai.character.type == Character.types.DEF)
                offSet_x = new Vector2(2.5f, 4f);
            else if (ai.character.type == Character.types.MID)
                offSet_x = new Vector2(2f, 4f);
            else if (ai.character.type == Character.types.FOR)
            {
                dest = ai.originalPosition;
                dest.x += Random.Range(-2, 2);
                dest.z += Random.Range(-1, 1);

                if (ai.character.teamID == 1 && ai.ball.transform.position.x < 0)
                    dest = Vector3.Lerp(ai.ball.transform.position, dest, 0.5f);
                else if (ai.character.teamID == 2 && ai.ball.transform.position.x > 0)
                    dest = Vector3.Lerp(ai.ball.transform.position, dest, 0.5f);
                return;
            }

            dest.x = opponentPos.x + (Random.Range(offSet_x.x, offSet_x.y) * direction);


            if (Mathf.Abs(opponentPos.x) > Mathf.Abs(ai.transform.position.x))// oponent está adelante de la linea del defensor
            {
                ai.character.SuperRun();
            }
            else
            {
                // oponent está atras de la linea del defensor
                // si el oponente está lejos en X, solo se mueve en z
                if (
                    (ai.character.teamID == 2 && _transform.position.x < opponentPos.x && Mathf.Abs(opponentPos.x - ai.transform.position.x) > 0.5f) ||
                    (ai.character.teamID == 1 && _transform.position.x > opponentPos.x && Mathf.Abs(opponentPos.x - ai.transform.position.x) > 0.5f)
                )
                     dest.z = ai.transform.position.z;  
            }
            if (ai.character.type == Character.types.DEF)
                dest.z = (ai.originalPosition.z/1.25f) + Random.Range( -1, 1);
            else
            {
                if (ai.character.type == Character.types.FOR)
                    dest = Vector3.Lerp(ai.originalPosition, dest, 0.75f);
                else
                    dest = Vector3.Lerp(ai.originalPosition, dest, 0.95f);

                dest.z += Utils.GetRandomFloatBetween(-2, 2);
            }



        }
    }

}