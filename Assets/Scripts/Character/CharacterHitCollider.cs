using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharacterHitCollider : MonoBehaviour
    {
        public Character character;

        private void Start()
        {
            Reset();
            float scale_x = Data.Instance.settings.GetSetting("hitScale") / 100;
            float scale_z = Data.Instance.settings.GetSetting("hitScale") / 100;
            transform.localScale = new Vector3(scale_x, 1f, scale_z);
        }
        public void Activate(bool isOn, float timer)
        {
            gameObject.SetActive(true);
            Invoke("Reset", timer);
        }
        void Reset()
        {
            gameObject.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            Character otherCharacter = other.GetComponent<Character>();
            if (otherCharacter == null) return;
            if (otherCharacter.data.id != character.data.id)
            {
                otherCharacter.states.Hitted();
            }
        }
    }
}