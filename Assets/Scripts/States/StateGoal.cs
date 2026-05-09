using System.Collections;
using UnityEngine;
namespace Fulbo.Game.States
{
    public class StateGoal : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.GOAL;
        }
        public override void SetActive()
        {
            base.SetActive();

            if (states.character.teamID == 0)//referi
                animName = "start";
            else if (Data.Instance.matchData.lastGoalBy == states.character.teamID)
                animName = "goal";
            else
                animName = "cry";

            states.PlayAnim(animName);
        }
        public override void Dash() { }
        public override void Jueguito() { }
        public override void Lujito() { }
      //  public override void Hitted() { }
        public override void Jump() { }
        public override void Hit() { }

        public override void Move(float t) { }
        public override void Stopped()
        {
            SetState(states.idle);
        }
    }
}