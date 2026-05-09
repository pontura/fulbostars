using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private AudioClip OnHitWithBall;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Ball")
            {
                OnPlaySound();
                OnBallHit();
            }
            if (collision.gameObject.tag == "Player")
            {
                OnCharacterHit(collision.gameObject.GetComponent<Character>());
            }
        }
        public virtual void OnBallHit() { }

        public virtual void OnPlaySound()
        {
            if (OnHitWithBall != null)
                AudioManager.Instance.PlaySpecificSound(OnHitWithBall);
        }
        public virtual void OnCharacterHit(Character character) { }
    }

}