using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class ObstacleMuro : Obstacle
    {
        int timesHitted;
        [SerializeField] private int maxTimeHitted = 6;

        public override void OnBallHit()
        {
            Ball ball = Fulbo.Game.GameManager.Instance.ball;
            ball.HitObstacle();
            if (ball.character != null)
            {
                ball.ForceLoseBall(ball.transform.forward * -1);
                // ball.character.Kick(CharacterStates.kickTypes.SOFT);
            }

            if (timesHitted > maxTimeHitted) return;
            if (Random.Range(0, 10) < 5)
            {
                timesHitted++;
            }

        }
    }
}
