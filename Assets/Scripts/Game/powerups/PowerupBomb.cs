using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Powerups
{
    public class PowerupBomb : Powerup
    {
        float forceToCharacters = 1.5f;
       // [SerializeField] Animator anim;
        [SerializeField] float speed = 10;

        Character target;
        public states state;

        public enum states
        {
            IDLE,
            WALK
        }
        Ball ball;
        float _x, _z = 0;
        GameObject camera;

        public override void OnInstanced()
        {
            camera = GameManager.Instance.cameraInGame.gameObject;
            speed = Data.Instance.settings.GetSetting("powerup_bomb_speed")*100;
            character.states.ThrowSomething();
            ball = Fulbo.Game.GameManager.Instance.ball;
            Vector3 pos = new Vector3(character.transform.localPosition.x, 0, character.transform.localPosition.z);
            pos += character.GetForward() * 1.1f;
            transform.localPosition = pos;
            pos += character.GetForward() * 2f;
            pos.y = transform.position.y;
            transform.LookAt(pos);
            OnShow();
            Walk();
            AudioManager.Instance.PlaySound("common", "ingame/powerups/game_bomb", true);
        }
        private void FixedUpdate()
        {
            if (target == null)
            {
                Character characterWithBall = ball.character;
                if (characterWithBall != null && characterWithBall.teamID != character.teamID)
                    target = characterWithBall;
                else
                    target = Fulbo.Game.GameManager.Instance.charactersManager.charactersStrategy.GetOtherCharacterNear(character.teamID == 1 ? 2 : 1, transform.position, 30);
            }
            Vector3 pos = transform.position;
            Vector3 dest = target.transform.position;
            dest.y = pos.y;
            transform.LookAt(dest);
            //asset.transform.localEulerAngles = camera.transform.localEulerAngles;
            //if (transform.position.x > target.transform.position.x)
            //    asset.transform.localScale = new Vector2(-1, 1);
            //else
            //    asset.transform.localScale = new Vector2(1, 1);
            rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
        }
        void Idle()
        {
            if (state != states.IDLE)
            {
                rb.velocity = Vector3.zero;
                state = states.IDLE;
               // anim.Play("idle");
            }
        }
        void Walk()
        {
            if (state != states.WALK)
            {
                state = states.WALK;
            }
        }
        public override void OnCharacterHitted()
        {
            if (characterThatCollide != target) return;
            AudioManager.Instance.PlaySound("common", "ingame/powerups/game_bomb_explotion", false);
            GetComponent<Collider>().enabled = false;
            Vector3 pos = transform.position;
            Vector3 characterPos = characterThatCollide.transform.position;
            Vector3 dir = (characterPos - pos) * forceToCharacters;
            dir.y = 0;
            characterThatCollide.Bounce(dir);
            Events.OnFX(Fulbo.FX.FXManager.types.EXPLOTION, characterPos);
            manager.DestroyPowerup(this);
            OnHide();
        }
    }
}
