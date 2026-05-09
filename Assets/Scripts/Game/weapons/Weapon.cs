using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Weapons
{
    public class Weapon : MonoBehaviour
    {
        float offset;
        [SerializeField] Rigidbody rb;
        [SerializeField] Collider colliders;
        public float force = 1000;
        public float force_y = 10;
        public float offset_z_multiplier = 1.1f;
        public float offset_y = 1;
        Character character;
        Character target;
        public float speed = 10;
        Vector3 direction;
        float forceToCharacters = 1.5f;

        public void Init(Character character)
        {
            this.gameObject.SetActive(false);
            this.character = character;
            Invoke("Shoot", 0.1f);
            colliders.enabled = false;
            Invoke("Explote", 3);
        }
        public void Shoot()
        {
            character.states.ThrowSomething();
            Invoke("EnableCollider", 0.25f);
            target = Fulbo.Game.GameManager.Instance.ball.character;
            if (target == null)
            {
                Reset(); return;
            }
            if (target.states.currentState.type != CharacterStates.types.RUN)
            {
                Reset(); return;
            }

            this.gameObject.SetActive(true);
            Vector3 pos = character.transform.localPosition;
            pos += character.transform.forward * offset_z_multiplier;
            pos.y += offset_y;
            transform.localPosition = pos;
            //transform.localEulerAngles = character.transform.localEulerAngles;

            direction = target.transform.localPosition;
            direction.y = transform.localPosition.y;
            transform.LookAt(direction);
            direction = transform.forward;

            Vector3 dir = transform.forward * force;
            dir.y += force_y;
            rb.AddForce(dir);
        }
        void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        void EnableCollider()
        {
            colliders.enabled = true;
        }
        private void OnCollisionEnter(Collision collision)
        {
            Character ch = collision.gameObject.GetComponent<Character>();
            if (ch != null)
            {
                if (ch != character)
                {
                    OnCharacterHit(ch);
                    Reset();
                }
            }
        }
        void Explote()
        {
            Reset();
        }
        void Reset()
        {
            Destroy(this.gameObject);
        }
        public virtual void OnCharacterHit(Character characterHitted)
        {
            AudioManager.Instance.PlaySound("common2", "obstacles/boing", false);
            Vector3 pos = transform.position;
            Vector3 characterPos = characterHitted.transform.position;
            Vector3 dir = (characterPos - pos) * forceToCharacters;
            dir.y = 0;
            characterHitted.Bounce(dir);
            Events.OnFX(Fulbo.FX.FXManager.types.EXPLOTION, characterPos);
        }
    }
}