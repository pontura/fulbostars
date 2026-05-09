using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class ObstacleBouncers : Obstacle
    {
        public Vector3 AddBouncerDirection = new Vector3(0, 500, 0);
        public float force = 500;
        public float forceToCharacters = 50;
        Animation anim;

        private void Awake()
        {
            anim = GetComponent<Animation>();
        }
        public override void OnBallHit()
        {
            Ball ball = Fulbo.Game.GameManager.Instance.ball;
            ball.HitObstacle();

            Character character = null;
            if (ball.character)
                character = ball.character;

            Vector3 dir = (ball.transform.position - transform.position) * force;
            dir += AddBouncerDirection;
            Fulbo.Game.GameManager.Instance.ball.ForceLoseBall(dir);
            if (anim != null)
                anim.Play();
            AudioManager.Instance.PlaySound("common2", "obstacles/boing", false);

            if (character != null)
                OnCharacterHit(character);

        }
        public override void OnCharacterHit(Character character)
        {
            if (anim != null)
                anim.Play();
            AudioManager.Instance.PlaySound("common2", "obstacles/boing", false);

            Vector3 pos = transform.position;
            Vector3 characterPos = character.transform.position;
            Vector3 dir = (characterPos - pos) * forceToCharacters;
            dir.y = 0;
            character.Bounce(dir);
        }
    }
}
