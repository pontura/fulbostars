namespace Fulbo.Game.States
{
    public class StatePowerupCharging : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.POWERUP;
        }
        public override void SetActive()
        {
            base.SetActive();
            animName = "idle";
            switch (states.character.powerupsManager.GetPowerupType())
            {
                case Powerups.Powerup.types.SUPERKICK:
                    animName = "kick_init";
                    break;
                case Powerups.Powerup.types.BOMB:
                    animName = "throw";
                    break;
                case Powerups.Powerup.types.SPEED:
                    animName = "runBoost";
                    break;
            }
            states.PlayAnim(animName);
        }
        public override void LoseBall() { SetState(states.hitted); }
        public override void Unfreeze()
        {
            SetState(states.idle);
        }
        public override void Stopped() { }
        public override void Move(float speed) { }
    }
}
