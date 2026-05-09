namespace Fulbo.Game.States
{
    public class StateJueguito : StateCharacter
    {
        public override void Init(CharacterStates states)
        {
            base.Init(states);
            type = CharacterStates.types.JUEGUITO;
        }
        public override void SetActive()
        {
            base.SetActive();
            animName = "jueguito";
            states.PlayAnim(animName);
            states.character.ballCatcher.Jueguito();
        }
        public override void Move(float speed)
        {
            states.character.ballCatcher.ResetJueguito();
            SetState(states.run);
        }
        public override void LoseBall()
        {
            base.LoseBall();
            states.character.ballCatcher.ResetJueguito();
            SetState(states.cry);
        }
    }
}