namespace Fulbo.Game.States
{
    public class StateHit : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.SPECIAL_ACTION;
        }
        public override void SetActive()
        {
            base.SetActive();
            animName = "hit";
            states.PlayAnim(animName, 0.5f, OnReady, true);
        }
        void OnReady()
        {
            SetState(states.idle);
        }
        public override void Stopped() { }
        public override void Move(float speed) { }
    }
}
