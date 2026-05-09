using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Powerups
{
    public class CharacterPowerupsManager : MonoBehaviour
    {
        Character character;
        PowerupsManager powerupsManager;
        public bool isOn;

        private void Awake()
        {
            character = GetComponent<Character>();
        }
        private void Start()
        {
            powerupsManager = Fulbo.Game.GameManager.Instance.powerupsManager;
        }
        public void Activate()
        {
            isOn = true;
            powerupsManager.SetPowerup(character, Data.Instance.matchData.team2_powerup);
        }
        public void Desactivate()
        {
            isOn = false;
        }
        public Powerup.types GetPowerupType()
        {
            return Data.Instance.matchData.team2_powerup;
            // to-do for multiplayer:
            //if (character.teamID == 1)
            //    return Data.Instance.matchData.team1_powerup;
            //else
            //    return Data.Instance.matchData.team2_powerup;
        }
    }
}