using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    // This is the Cone View of the player:

    public class CharacterViewZoneController : MonoBehaviour
    {
        List<Character> charactersInside;
        Character character;
        [SerializeField] GameObject asset;


        void Start()
        {
            charactersInside = new List<Character>();
            SetOff();

            Events.CatchBall += CatchBall;
            Events.LoseBall += LoseBall;
            Events.OnBallKicked += OnBallKicked;
        }
        void OnDestroy()
        {
            Events.CatchBall -= CatchBall;
            Events.LoseBall -= LoseBall;
            Events.OnBallKicked -= OnBallKicked;
        }
        void CatchBall(Character character)
        {
            asset.transform.SetParent(character.ballCatcher.pivot.transform);
            asset.transform.localEulerAngles = Vector3.zero;
            asset.transform.localPosition = Vector3.zero;
            this.character = character;
            SetOn();
        }
        void LoseBall(Character character)
        {
            SetOff();
        }
        void OnBallKicked(CharacterStates.kickTypes t, float s, Character character)
        {
            SetOff();
        }
        void SetOn()
        {
            charactersInside.Clear();
            asset.gameObject.SetActive(true);
        }
        void SetOff()
        {
            charactersInside.Clear();
            asset.gameObject.SetActive(false);
        }
        public Character GetNearest(int teamID)
        {
            if (charactersInside == null) return null;
            Character characterNearest = null;
            float nearestDist = 10000;
            foreach (Character characterInside in charactersInside)
            {
                if (characterInside.teamID == teamID)
                {
                    float dist = Vector3.Distance(character.transform.position, characterInside.transform.position);
                    if (characterNearest == null || dist < nearestDist)
                    {
                        characterNearest = characterInside;
                        nearestDist = dist;
                    }
                }
            }
            return characterNearest;
        }
        private void OnTriggerExit(Collider other)
        {
            if (character != null && other.tag == "Player")
            {
                Character otherCharacter = other.GetComponent<Character>();
                charactersInside.Remove(otherCharacter);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (character != null && other.tag == "Player")
            {
                Character otherCharacter = other.GetComponent<Character>();
                if (otherCharacter == null || otherCharacter == character)
                    return;
                charactersInside.Add(otherCharacter);
                if (!character.isBeingControlled)
                {
                    character.ai.OnCharacterInFront(otherCharacter);
                }
            }
        }
    }
}