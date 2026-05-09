using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class CharacterMovement : MonoBehaviour
    {
        public states state;
        Vector3 origin;
        Vector3 destination;
        Character character;
        float duration;
        float timer;
        private void Start()
        {
            character = GetComponent<Character>();
        }
        public enum states
        {
            IDLE,
            MOVEING
        }
        public void SetDestination(Vector3 destination, float duration)
        {
            timer = 0;
            origin = character.transform.localPosition;
            this.duration = duration;
            this.destination = destination;
            state = states.MOVEING;
        }
        float offset = 0.15f;
        void Update()
        {
            if (state == states.IDLE) return;
            timer += Time.deltaTime;
            float lerp = timer / duration;
            character.transform.localPosition = Vector3.Lerp(origin, destination, lerp);

            if (Vector3.Distance(character.transform.localPosition, destination)<0.1f)
                state = states.IDLE;
        }
    }
}
