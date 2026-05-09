using Fulbo.Game.AIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class BallNearController : MonoBehaviour
    {
        AI ai;

        void Start()
        {
            ai = GetComponent<AI>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (ai == null) return;
            if (ai.character == null) return;
            if (other.tag == "Ball")
            {
                Ball ball = other.GetComponent<Ball>();
                if (ball.kickType == CharacterStates.kickTypes.KICK_POWERUP)
                {
                    ai.character.states.Hitted();
                    return;
                }
                
                if (ai.character == ball.characterThatKicked) return;

                ball.OnEnterTrigger(ai.character);

                if (ai.character.isBeingControlled) return;

                if (ai.character.type == Character.types.GOALKEEPER && ball.transform.position.y > 1f)
                    ai.OnBallNearOnAir();
                else
                    ai.OnBallNear();
            }
        }
    }
}
