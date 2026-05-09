namespace Fulbo.Game.States
{
    public class StateKick : StateCharacter
    {
        bool isGoalKeeper;

        public override void Init(CharacterStates states)
        {
            isGoalKeeper = states.character.type == Character.types.GOALKEEPER;
            base.Init(states);
            type = CharacterStates.types.KICK;
        }
        public override void SetActive()
        {
            base.SetActive();
            animName = "kick";
            if (isGoalKeeper)
                animName = "kick_soft";
            float duration = 1;

            if (states.kickType == CharacterStates.kickTypes.CHILENA)
            {
                AudioManager.Instance.PlaySoundOneShot("shouts", "ingame/game_jump", false);
                if (states.character.teamID == 1)
                    states.LookTo(1);
                else
                    states.LookTo(-1);
                animName = "chilena";
                duration = 1f;
            }
            else if (states.kickType == CharacterStates.kickTypes.HEAD)
            {
                animName = "head";
                duration = 0.7f;
            }
            else if (states.kickType == CharacterStates.kickTypes.BALOON)
            {
                animName = "kick_soft";
                duration = .8f;
            }
            else if (states.kickType == CharacterStates.kickTypes.HARD || states.kickType == CharacterStates.kickTypes.CENTRO || states.kickType == CharacterStates.kickTypes.KICK_TO_GOAL || states.kickType == CharacterStates.kickTypes.KICK_POWERUP)
            {
                animName = "kick_power";
                duration = 1f;
            }
            else
            {
                if (isGoalKeeper)
                    animName = "kick_soft";
                duration = 1f;
            }
            states.PlayAnim(animName, duration, OnReady, true);
        }
        void OnReady()
        {
           // SetState(states.idle);
            Unfreeze();
        }
        public override void Stopped() { }
        public override void LoseBall() { SetState(states.hitted); }
        public override void Dash() { }
        public override void Jueguito() { }
        public override void Lujito() { }
        public override void Hitted() { }
        public override void Jump() { }
        public override void Hit() { }
    }
}