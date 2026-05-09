using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateFreeze : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.FREEZE;
        }
        public override void SetActive()
        {
            float delay = 1;
            if (states.character.type == Character.types.GOALKEEPER)
            {
                if (GameManager.Instance.state == GameManager.states.PLAYING)
                {
                    if(Vector3.Distance(states.ball.transform.position, states.character.transform.position)<8)
                        AudioManager.Instance.PlaySoundOneShot("common", "ingame/game_fall", false);
                    Events.OnFX(FX.FXManager.types.FUZZY, states.character.transform.position);
                }
                delay = states.character.debufSystem.GetDelayOnFloor();
                states.PlayAnim("freeze");
            } else
                delay = states.character.stats.freeze_dash;

            states.Freeze(delay);            
        }
        public override void OnCatchBall() { }
        public override void Dash() { }
        public override void Jueguito() { }
        public override void Lujito() { }
      //  public override void Hitted() { }
        public override void Jump() { }
        public override void Hit() { }

        public override void Unfreeze()
        {
            SetState(states.idle);
        }

    }
}
