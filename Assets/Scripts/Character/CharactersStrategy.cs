using UnityEngine;
using System.Collections;

namespace Fulbo.Game
{
    public class CharactersStrategy
    {
        CharactersManager charactersManager;
        Character characterReceiver;

        public receiverStates receiverState;
        public enum receiverStates
        {
            NONE,
            VOLEA,
            PARED
        }

        public void Init(CharactersManager cm) {  this.charactersManager = cm;  }

        public Character GetCharacterEnPase(Character character)
        {
            if (character == null) return null;
            Character other = charactersManager.characterViewZoneController.GetNearest(character.teamID);
            if (other == null) return null;
            if (!other.GetColliderState()) return null;
            if (other == null || other.control_id == character.control_id) return null;
            return other;
        }
        public Character GetOtherCharacterNear(int teamID, Vector3 pos, float MaxDistance, bool ignoreControlls = false, bool ifHasControlGetSecond = false, bool DontGetGoalKeeper = false)
        {
            Character ch = charactersManager.GetNearest(teamID, false, pos, ifHasControlGetSecond, DontGetGoalKeeper, ignoreControlls);
            if (ch == null) return null;
            float distance = Vector3.Distance(pos, ch.transform.position);
            if (distance < MaxDistance)
                return ch;
            return null;
        }
        public void SetCharacterReceiver(Character character)
        {
            characterReceiver = character;
        }
        public Character GetReceiver()
        {
            return characterReceiver;
        }
        public bool CheckForStrategy(Character character)
        {
            if (character == GetReceiver())
            {
                if (receiverState != receiverStates.NONE)
                    charactersManager.ball.KickByStrategy(character, receiverState);
                else
                    GameManager.Instance.ball.CharacterCatchBall(character);

                ResetStates();
                return true;
            }
            ResetStates();
            return false;
        }
        public void ResetStates()
        {
            SetReceiverState(receiverStates.NONE);
            SetCharacterReceiver(null);
        }
        public void SetReceiverState(receiverStates newState)
        {
            receiverState = newState;
        }
    }
}
