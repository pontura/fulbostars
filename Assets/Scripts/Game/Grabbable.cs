using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Game
{
    public class Grabbable : MonoBehaviour
    {
        bool isGame;
        public types type;
        public enum types
        {
            GOLD,
            POWERUP
        }
        int prize;
        private void Start()
        {
            if (Fulbo.Game.GameManager.Instance.charactersManager is CharactersManagerTutorial)
                isGame = false;
            prize = 1;// CupsData.Instance.GetActualLevel().coinPrize;
        }
        private void OnCollisionEnter(Collision collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character == null) return;
            if (character.isBeingControlled)
            {
                Events.OnGrab(this);
                switch (type)
                {
                    case types.GOLD:
                         // to-do si hay otros objetos a futuro
                        if(prize>0)
                            Events.OnGoldScoreWin(prize, transform.localPosition);
                        AudioManager.Instance.PlaySoundOneShot("common2", "ui/game_grab_coin", false);
                        break;
                    case types.POWERUP:
                        if (Fulbo.Game.GameManager.Instance.powerupsManager.IsFilled() || 
                            Fulbo.Game.GameManager.Instance.powerupsManager.IsPowerupActive()) return;
                        Events.OnPowerupIncrease(character);
                        AudioManager.Instance.PlaySound("common2", "ingame/powerups/game_pastilla", false);
                        break;
                }
                //if (isGame)
                //    Respawn();
                //else
                    Destroy(this.gameObject);
            }
        }
        //void Respawn()
        //{

        //    float _x = Random.Range(-12, 12);
        //    float _z = Random.Range(-20, 20);
        //    transform.localPosition = new Vector3(_x, 4, _z);
        //}
    }
}