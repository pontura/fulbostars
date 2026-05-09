using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game.Weapons
{
    public class WeaponsManager : MonoBehaviour
    {
        [SerializeField] Weapon[] weapons;
        [SerializeField] Transform weaponsContainer;

        void Start()
        {
            int stadium_id = Data.Instance.matchData.levelData.stadium_id;
            //if (stadium_id == 3) // TO-DO solo si es playa anda:
            //{
            //    Events.OnWeapon += OnWeapon;
            //    Loop();
            //}
        }
        void OnDestroy()
        {
            Events.OnWeapon -= OnWeapon;
        }
        void Loop()
        {
            Invoke("Loop", 3);
            if (Fulbo.Game.GameManager.Instance.state != Fulbo.Game.GameManager.states.PLAYING) return;
            Character characterWithBall = Fulbo.Game.GameManager.Instance.ball.character;
            if (characterWithBall == null) return;
            int teamID = characterWithBall.teamID == 1 ? 2 : 1;
            Character character = Fulbo.Game.GameManager.Instance.charactersManager.GetNearest(teamID, false, characterWithBall.transform.position, true, true);
            if (character != null)
                Events.OnWeapon(character);
        }
        void OnWeapon(Character character)
        {
            Weapon w = Instantiate(weapons[0], weaponsContainer);
            w.Init(character);
        }
    }
}
