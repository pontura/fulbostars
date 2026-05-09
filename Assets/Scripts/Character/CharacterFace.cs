using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fulbo.Game
{
    public class CharacterFace : MonoBehaviour
    {
        public GameObject idle;
        public GameObject angry;
        Character character;

        private void Start()
        {
            character = GetComponentInParent<Character>();
            Idle();
            Events.OnGoal += OnGoal;
            Events.OnGameStatusChanged += OnGameStatusChanged;
        }
        private void OnDestroy()
        {
            Events.OnGameStatusChanged -= OnGameStatusChanged;
            Events.OnGoal -= OnGoal;
        }
        void OnGameStatusChanged(Fulbo.Game.GameManager.states state)
        {
            if (state == Fulbo.Game.GameManager.states.PLAYING)
                Idle();
        }
        void OnGoal(int teamID, Character ch)
        {
            if (character == null) return;
            if (character.teamID == teamID)
                Idle();
            else
                Angry();
        }
        private void Reset()
        {
            if(idle != null)
                idle.SetActive(false);
            if (angry != null)
                angry.SetActive(false);
        }
        public void Idle()
        {
            Reset();
            if (idle != null)
                idle.SetActive(true);
        }

        void Angry()
        {
            Reset();
            if (angry != null)
                angry.SetActive(true);
        }
    }
}