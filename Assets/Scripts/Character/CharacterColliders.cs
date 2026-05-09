using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharacterColliders : MonoBehaviour
    {
        [SerializeField] CapsuleCollider realCollider;
        [SerializeField] CapsuleCollider airCollider;
        [SerializeField] float collider_radius;
        [SerializeField] float collider_height;
        [SerializeField] float collider_radius_air;
        [SerializeField] Rigidbody rb;
        float realColliderRadius;

        public void Init(Character character)
        {
            collider_radius = character.stats.collider_radius;
            collider_height = character.stats.collider_height;
            collider_radius_air = character.stats.collider_radius_air;

            float radiusAmmountByDuel = 0;
            if (character.type == Character.types.GOALKEEPER && character.teamID == 1)
                radiusAmmountByDuel = -0.15f + (CupsData.Instance.GetActualLevel().duelStatsGK / 400);

            realCollider.radius = realColliderRadius = (collider_radius / 10) + radiusAmmountByDuel;
            realCollider.height = collider_height / 10;
            airCollider.radius = collider_radius_air / 10;
        }
        public void Reset()
        {
            ResetSetCollidersOff();
            ResetRadius();
            ResetPosition();
            ResetRigidBody();
        }
        public void SetCollidersOff(float delay)
        {
            CancelInvoke();
            realCollider.enabled = false;
            Invoke("ResetSetCollidersOff", delay);
        }
        public void ResetSetCollidersOff()
        {
            CancelInvoke();
            realCollider.enabled = true;
            realCollider.center = collidersOriginalPos;
        }
        Vector3 collidersOriginalPos = Vector3.zero;
        public void MoveCollidersTo(Vector3 pos)
        {
            CancelInvoke();
            realCollider.enabled = true;
            realCollider.center = collidersOriginalPos + pos;
            Invoke("ResetPosition", 0.5f);
        }
        void ResetPosition()
        {
            CancelInvoke();
            realCollider.enabled = true;
            realCollider.center = collidersOriginalPos;
        }
        public bool GetColliderState()
        {
            if (realCollider == null) return true;
            return realCollider.enabled;
        }
        public void ResetRigidBody()
        {
            SetVelocity(Vector3.zero);
        }
        void SetVelocity(Vector3 value)
        {
            if (rb == null) return;
            rb.velocity = value;
        }
        private void OnCollisionEnter(Collision collision)
        {
            Invoke("ResetRigidBody", 0.2f);
        }
        public void ChangeRadius(float factor, float timer)
        {
            StartCoroutine(SetRadius(factor, timer));
        }
        void ResetRadius()
        {
            realCollider.radius = realColliderRadius;
        }
        IEnumerator SetRadius(float factor, float timer)
        {
            realCollider.radius = realColliderRadius * factor;
            yield return new WaitForSeconds(timer);
            ResetRadius();
        }
    }
}