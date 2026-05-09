using UnityEngine;

namespace Fulbo.Game.States
{
    public class StateCry : StateCharacter
    {
        bool crying;
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.CRY;
        }
        public override void SetActive()
        {
            base.SetActive();
            float delay = states.character.stats.freeze_by_hit;
            states.character.SetCollidersOff(delay);
            animName = "cry";
            states.PlayAnim(animName, delay, OnReady, true);
            crying = true;

            if (states.character.type == Character.types.GOALKEEPER)
                Events.SetDialogue(states.character, Data.Instance.textsData.GetRandomDialogue("full", states.character.data.id, states.character.type == Character.types.GOALKEEPER));
        }
        void OnReady()
        {
            crying = false;
            if (Fulbo.Game.GameManager.Instance.state != Fulbo.Game.GameManager.states.GOAL)
                SetState(states.idle);
        }
        public override void Stopped()
        {
           // SetState(states.idle);
        }
        public override void Move(float speed)
        {
            if (crying) return;
            base.Move(speed);
        }
        //public override void OnReset()
        //{
        //    crying = false; //SetState(states.idle);
        //}
    }

}