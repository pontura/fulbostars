using System.Collections;
using UnityEngine;

namespace Fulbo.Game.Powerups
{
    public class PowerupSuperKick : Powerup
    {
        float multiplierSpeed = 2;
        float duration = 4;
        Ball ball;

        private void Start()
        {
            Events.OnGoal += OnGoal;
            Events.OnBallHitCharacter += OnBallHitCharacter;
        }
        private void OnDestroy()
        {
            Events.OnGoal -= OnGoal;
            Events.OnBallHitCharacter -= OnBallHitCharacter;
        }
        void OnGoal(int a, Character ch)
        {
            OnResetPowerup();
            AudioManager.Instance.PlaySound("common2", "", false);
        }
        void OnBallHitCharacter(Character ch)
        {
            OnResetPowerup();
            AudioManager.Instance.PlaySound("common2", "", false);
        }
        public override void OnInstanced()
        {
            AudioManager.Instance.PlaySound("common", "ingame/powerups/game_superkick", false);
            duration = Data.Instance.settings.GetSetting("powerup_SuperSpeed_duration");
            manager.Activate(OnResetPowerup, duration);

            SetColliders(false);

            character.Kick(CharacterStates.kickTypes.KICK_POWERUP);

            AttachToBall();
        }
        void AttachToBall()
        {
            ball = GameManager.Instance.ball;
            transform.SetParent(ball.transform);
            transform.localPosition = Vector3.zero;
        }
        void OnResetPowerup()
        {
            if (manager == null) return;
            SetColliders(true);
            if (ball != null) ball.ResetKick();
            manager.DestroyPowerup(this);
        }
        void SetColliders(bool isOn)
        {
            if (!isOn)
            {
                foreach (Character character in GameManager.Instance.charactersManager.team1)
                    if(character.type != Character.types.GOALKEEPER)
                        character.SetCollidersOff(duration);
                foreach (Character character in GameManager.Instance.charactersManager.team2)
                    if (character.type != Character.types.GOALKEEPER)
                        character.SetCollidersOff(duration);
            }
            else
            {
                foreach (Character character in GameManager.Instance.charactersManager.team1)
                    character.characterColliders.Reset();
                foreach (Character character in GameManager.Instance.charactersManager.team2)
                    character.characterColliders.Reset();
            }
        }
    }
}