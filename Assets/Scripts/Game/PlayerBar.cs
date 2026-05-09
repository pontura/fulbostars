using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Fulbo.Game
{
    public class PlayerBar : MonoBehaviour
    {
        public Text playerName;
        Character character;
        //  public GameObject progressBar;

        void Awake()
        {
            Events.CharacterCatchBall += CharacterCatchBall;
            Events.OnBallKicked += OnBallKicked;
            Events.OnGoal += OnGoal;
        }
        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            Events.CharacterCatchBall -= CharacterCatchBall;
            Events.OnBallKicked -= OnBallKicked;
            Events.OnGoal -= OnGoal;
        }
        void OnGoal(int id, Character ch)
        {
            gameObject.SetActive(false);
        }
        private void Update()
        {
            if (character == null)
                return;
            transform.position = Fulbo.Game.GameManager.Instance.cameraInGame.cam.WorldToScreenPoint(character.transform.position);
        }

        void OnBallKicked(CharacterStates.kickTypes kickType, float forceForce, Character character)
        {
            character = null;
            Invoke("Reset", 1);
        }
        void CharacterCatchBall(Character character)
        {
            gameObject.SetActive(true);
            //if (!character.isBeingControlled)
            //    progressBar.SetActive(false);
            //else
            //    progressBar.SetActive(true);

            this.character = character;
            CancelInvoke();
            playerName.text = character.avatarName.ToUpper();
        }
        void Reset()
        {
            playerName.text = "";
            if (character == null)
                gameObject.SetActive(false);
        }
    }

}